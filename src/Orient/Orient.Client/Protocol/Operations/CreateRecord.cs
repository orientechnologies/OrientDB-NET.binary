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
            request.DataItems.Add(new RequestDataItem() {Type = "byte", Data = BinarySerializer.ToArray((byte) OperationType.RECORD_CREATE)});
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(sessionID) });
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(-1) });  // data segment id
            request.DataItems.Add(new RequestDataItem() { Type = "short", Data = BinarySerializer.ToArray((short)-1) });
            request.DataItems.Add(new RequestDataItem() {Type = "string", Data = BinarySerializer.ToArray(_document.Serialize())});
            request.DataItems.Add(new RequestDataItem() {Type = "byte", Data = BinarySerializer.ToArray((byte) 'd')});
            request.DataItems.Add(new RequestDataItem() {Type = "byte", Data = BinarySerializer.ToArray((byte) 0)});

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
            _document.OVersion = reader.ReadInt32EndianAware();

            if (_database.ProtocolVersion > 21)
            {
                int collectionChanges = reader.ReadInt32EndianAware();
                if (collectionChanges != 0)
                    throw new NotSupportedException();
            }

            return responseDocument;


        }
    }
}
