using System.Collections.Generic;
using System.IO;
using System.Linq;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class Command : IOperation
    {
        internal OperationMode OperationMode { get; set; }
        internal CommandClassType ClassType { get; set; }
        internal CommandPayload CommandPayload { get; set; }

        public Request Request(int sessionId)
        {
            Request request = new Request();
            
            // standard request fields
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)OperationType.COMMAND) });
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(sessionId) });
            // operation specific fields
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)OperationMode) });

            // class name field
            string className = "x";
            switch (ClassType)
            {
                // idempotent command (e.g. select)
                case CommandClassType.Idempotent:
                    className = "com.orientechnologies.orient.core.sql.query.OSQLSynchQuery";
                    break;
                // non-idempotent command (e.g. insert)
                case CommandClassType.NonIdempotent:
                    if (!string.IsNullOrEmpty(CommandPayload.Language) && CommandPayload.Language.Equals("gremlin"))
                    {
                        className = "com.orientechnologies.orient.graph.gremlin.OCommandGremlin";
                    }
                    else
                    {
                        className = "com.orientechnologies.orient.core.sql.OCommandSQL";
                    }
                    break;
                // script command
                case CommandClassType.Script:
                    className = "com.orientechnologies.orient.core.command.script.OCommandScript";
                    break;
                default:
                    break;
            }

            // TODO: sql script case length
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(
                //4 + // this int
                4 + // class name int length
                BinarySerializer.Length(className) + 
                4 + // limit int length
                4 + // text int length
                BinarySerializer.Length(CommandPayload.Text) + 
                4 + // fetch plant int length
                BinarySerializer.Length(CommandPayload.FetchPlan) +
                4 // serialized params int (disable)
            ) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(className) });

            if (CommandPayload.Type == CommandPayloadType.SqlScript)
            {
                request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(CommandPayload.Language) });
            }

            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(CommandPayload.Text) });
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(CommandPayload.NonTextLimit) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(CommandPayload.FetchPlan) });
            //request.DataItems.Add(new RequestDataItem() { Type = "bytes", Data = CommandPayload.SerializedParams });
            // HACK: 0:int means disable
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(0) });
            
            return request;
        }

        public ODocument Response(Response response)
        {
            ODocument responseDocument = new ODocument();
            
            if (response == null)
            {
                return responseDocument;
            }

            var reader = response.Reader;

            // operation specific fields
            PayloadStatus payloadStatus = (PayloadStatus)reader.ReadByte();

            responseDocument.SetField("PayloadStatus", payloadStatus);

            if (OperationMode == OperationMode.Asynchronous)
            {
                List<ODocument> documents = new List<ODocument>();

                while (payloadStatus != PayloadStatus.NoRemainingRecords)
                {
                    ODocument document = ParseDocument(reader);

                    switch (payloadStatus)
                    {
                        case PayloadStatus.ResultSet:
                            documents.Add(document);
                            break;
                        case PayloadStatus.PreFetched:
                            //client cache
                            response.Connection.Database.ClientCache[document.ORID] = document;
                            break;
                        default:
                            break;
                    }

                    payloadStatus = (PayloadStatus)reader.ReadByte();
                }

                responseDocument.SetField("Content", documents);
            }
            else
            {
                int contentLength;

                switch (payloadStatus)
                {
                    case PayloadStatus.NullResult: // 'n'
                        // nothing to do
                        break;
                    case PayloadStatus.SingleRecord: // 'r'
                        ODocument document = ParseDocument(reader);
                        responseDocument.SetField("Content", document);
                        break;
                    case PayloadStatus.SerializedResult: // 'a'
                        // TODO: how to parse result - string?
                        contentLength = reader.ReadInt32EndianAware();
                        string serialized = System.Text.Encoding.Default.GetString(reader.ReadBytes(contentLength));
                        responseDocument.SetField("Content", serialized);
                        break;
                    case PayloadStatus.RecordCollection: // 'l'
                        List<ODocument> documents = new List<ODocument>();

                        int recordsCount = reader.ReadInt32EndianAware();

                        for (int i = 0; i < recordsCount; i++)
                        {
                            documents.Add(ParseDocument(reader));
                        }

                        responseDocument.SetField("Content", documents);
                        break;
                    default:
                        break;
                }

                if (OClient.ProtocolVersion >= 17)
                {
                    //Load the fetched records in cache
                    while ((payloadStatus = (PayloadStatus)reader.ReadByte()) != PayloadStatus.NoRemainingRecords)
                    {
                        ODocument document = ParseDocument(reader);
                        if (document != null && payloadStatus == PayloadStatus.PreFetched)
                        {
                            //Put in the client local cache
                            response.Connection.Database.ClientCache[document.ORID] = document;
                        }
                    }
                }
            }

            return responseDocument;
        }

        private ODocument ParseDocument(BinaryReader reader)
        {
            ODocument document = null;

            short classId = reader.ReadInt16EndianAware();

            if (classId == -2) // NULL
            {
            }
            else if (classId == -3) // record id
            {
                ORID orid = new ORID();
                orid.ClusterId = reader.ReadInt16EndianAware();
                orid.ClusterPosition = reader.ReadInt64EndianAware();

                document = new ODocument();
                document.ORID = orid;
                document.OClassId = classId;
            }
            else
            {
                ORecordType type = (ORecordType)reader.ReadByte();

                ORID orid = new ORID();
                orid.ClusterId = reader.ReadInt16EndianAware();
                orid.ClusterPosition = reader.ReadInt64EndianAware();
                int version = reader.ReadInt32EndianAware();
                int recordLength = reader.ReadInt32EndianAware();
                byte[] rawRecord = reader.ReadBytes(recordLength);
                document = RecordSerializer.Deserialize(orid, version, type, classId, rawRecord);
            }

            return document;
        }
    }
}
