using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class CreateRecord : IOperation
    {
        private readonly ODocument _document;
        private readonly ODatabase _database;

        public CreateRecord(ODocument document, ODatabase database)
        {
            _document = document;
            _database = database;
        }

        public Request Request(int sessionID)
        {
            Request request = new Request();

            if (_document.ORID != null)
                throw new InvalidOperationException();

            CorrectClassName();

            var clusterId = _database.GetClusterIdFor(_document.OClassName);
            _document.ORID = new ORID(clusterId, -1);

            // standard request fields
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)OperationType.RECORD_CREATE) });
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(sessionID) });
            if (OClient.ProtocolVersion < 24)
            {
                request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray((int)-1) });  // data segment id
            }
            request.DataItems.Add(new RequestDataItem() { Type = "short", Data = BinarySerializer.ToArray((short)-1) });
            request.DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(_document.Serialize()) });
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)'d') });
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)0) });

            return request;
        }

        private void CorrectClassName()
        {
            if (_document.OClassName == "OVertex")
                _document.OClassName = "V";
            if (_document.OClassName == "OEdge")
                _document.OClassName = "E";
        }

        public ODocument Response(Response response)
        {
            ODocument responseDocument = _document;


            if (response == null)
            {
                return responseDocument;
            }

            var reader = response.Reader;

            _document.ORID.ClusterPosition = reader.ReadInt64EndianAware();
            if (OClient.ProtocolVersion >= 11)
            {
                _document.OVersion = reader.ReadInt32EndianAware();
            }

            if (OClient.ProtocolVersion >= 20)
            {
                try//if (reader.BaseStream.CanRead && reader.PeekChar() != -1)
                {
                    int collectionChangesCount = reader.ReadInt32EndianAware();
                    for (var i = 0; i < collectionChangesCount; i++)
                    {
                        //    throw new NotImplementedException("don't understand what to do with this yet");
                        var mostSigBits = reader.ReadInt64EndianAware();
                        var leastSigBits = reader.ReadInt64EndianAware();
                        var updatedFileId = reader.ReadInt64EndianAware();
                        var updatedPageIndex = reader.ReadInt64EndianAware();
                        var updatedPageOffset = reader.ReadInt32EndianAware();
                    }
                }
                catch (Exception ex) 
                { 
                }
            }
            return responseDocument;


        }
    }
}
