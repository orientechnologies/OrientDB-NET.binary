using System.Collections.Generic;
using System.IO;
using System.Linq;
using Orient.Client.API.Types;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class Command : IOperation
    {
        internal OperationMode OperationMode { get; set; }
        internal CommandPayloadBase CommandPayload { get; set; }

        public Request Request(Request request)
        {
            // standard request fields
            request.AddDataItem((byte)OperationType.COMMAND);
            request.AddDataItem(request.SessionId);
            // operation specific fields
            request.AddDataItem((byte)OperationMode);

            // idempotent command (e.g. select)
            var queryPayload = CommandPayload as CommandPayloadQuery;
            if (queryPayload != null)
            {
                // Write command payload length
                request.AddDataItem(queryPayload.PayLoadLength);
                request.AddDataItem(queryPayload.ClassName);
                //(text:string)(non-text-limit:int)[(fetch-plan:string)](serialized-params:bytes[])
                request.AddDataItem(queryPayload.Text);
                request.AddDataItem(queryPayload.NonTextLimit);
                request.AddDataItem(queryPayload.FetchPlan);

                // TODO: Implement Serialized Params for Idempotent query
                // HACK: 0:int means disable
                request.AddDataItem((int)0);
                return request;
            }
            // non-idempotent command (e.g. insert)
            var scriptPayload = CommandPayload as CommandPayloadScript;
            if (scriptPayload != null)
            {
                // Write command payload length
                request.AddDataItem(scriptPayload.PayLoadLength);
                request.AddDataItem(scriptPayload.ClassName);
                if (scriptPayload.Language != "gremlin")
                    request.AddDataItem(scriptPayload.Language);
                request.AddDataItem(scriptPayload.Text);
                request.AddDataItem((byte)0);
                request.AddDataItem((byte)0);

                return request;
            }
            var commandPayload = CommandPayload as CommandPayloadCommand;
            if (commandPayload != null)
            {
                // Write command payload length
                request.AddDataItem(commandPayload.PayLoadLength);
                request.AddDataItem(commandPayload.ClassName);
                // (text:string)(has-simple-parameters:boolean)(simple-paremeters:bytes[])(has-complex-parameters:boolean)(complex-parameters:bytes[])
                request.AddDataItem(commandPayload.Text);
                // has-simple-parameters boolean
                request.AddDataItem((byte)0); // 0 - false, 1 - true
                //request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(0) });
                // has-complex-parameters
                request.AddDataItem((byte)0); // 0 - false, 1 - true
                //request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(0) });
                return request;
            }
            throw new OException(OExceptionType.Operation, "Invalid payload");
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

                document = new ODocument { ORID = orid, OVersion = version, OType = ORecordType.Document, OClassId = classId };

                if (OClient.SerializationImpl == ORecordFormat.ORecordDocument2csv.ToString())
                    document = RecordSerializerFactory.GetSerializer(OClient.Serializer).Deserialize(rawRecord, document);
                else
                {
                    document = RecordSerializerFactory.GetSerializer(OClient.Serializer).Deserialize(rawRecord, document);
                }

            }

            return document;
        }

    }
}
