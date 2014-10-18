using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Orient.Client.API.Types;
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
            request.AddDataItem((byte)OperationType.RECORD_CREATE);
            request.AddDataItem(sessionID);

            if (OClient.ProtocolVersion < 24)
            {
                request.AddDataItem((int)-1);  // data segment id
            }

            request.AddDataItem((short)clusterId);
            request.AddDataItem(_document.Serialize());
            request.AddDataItem((byte)ORecordType.Document);
            request.AddDataItem((byte)0);

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

            if (_database.ProtocolVersion > 21)
            {
                int collectionChangesCount = reader.ReadInt32EndianAware();
                for (var i = 0; i < collectionChangesCount; i++)
                {
                    throw new NotImplementedException("Collection changes not yet handled - failing rather than ignoring potentially significant information");
                    //var mostSigBits = reader.ReadInt64EndianAware();
                    //var leastSigBits = reader.ReadInt64EndianAware();
                    //var updatedFileId = reader.ReadInt64EndianAware();
                    //var updatedPageIndex = reader.ReadInt64EndianAware();
                    //var updatedPageOffset = reader.ReadInt32EndianAware();
                }
            }
            return responseDocument;


        }
    }
}
