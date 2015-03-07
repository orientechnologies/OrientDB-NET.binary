using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol.Operations
{
    internal class RecordUpdate : BaseOperation
    {
        private readonly ODocument _document;
        internal OperationMode OperationMode { get; set; }

        public RecordUpdate(ODocument document, ODatabase database)
            : base(database)
        {
            _document = document;
            _operationType = OperationType.RECORD_UPDATE;
        }

        public override Request Request(Request request)
        {
            base.Request(request);

            if (_document.ORID != null)
                throw new InvalidOperationException();

            CorrectClassName();

            request.AddDataItem(_document.ORID);
            if (OClient.ProtocolVersion >= 23)
            {
                request.AddDataItem((int)1);  // update content  1 - true , 0 - false
            }

            request.AddDataItem(Serializer.Serialize(_document));
            request.AddDataItem(_document.OVersion);
            request.AddDataItem((byte)ORecordType.Document);
            request.AddDataItem((byte)((OperationMode == OperationMode.Synchronous) ? 0 : 1));

            return request;
        }

        public override ODocument Response(Response response)
        {
            ODocument responseDocument = _document;


            if (response == null)
            {
                return responseDocument;
            }

            var reader = response.Reader;
            if (response.Connection.ProtocolVersion > 26 && response.Connection.UseTokenBasedSession)
                ReadToken(reader);

            _document.OVersion = reader.ReadInt32EndianAware();

            // Work around differents in storage type < version 2.0
            if (_database.ProtocolVersion >= 28 || (_database.ProtocolVersion >= 20 && _database.ProtocolVersion <= 27 && !EndOfStream(reader)))
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

        private void CorrectClassName()
        {
            if (_document.OClassName == "OVertex")
                _document.OClassName = "V";
            if (_document.OClassName == "OEdge")
                _document.OClassName = "E";
        }
    }
}
