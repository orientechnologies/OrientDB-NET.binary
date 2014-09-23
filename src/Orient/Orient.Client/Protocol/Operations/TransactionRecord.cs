using System;

namespace Orient.Client.Protocol.Operations
{
    internal enum RecordType
    {
        Update = 1,
        Delete = 2,
        Create = 3,
    }

    class TransactionRecord
    {
        public TransactionRecord(RecordType recordType, ODocument document = null)
        {
            RecordType = recordType;
            Document = document;
        }

        public ODocument Document { get; set; }
        public RecordType RecordType { get; private set; }
        public IBaseRecord Object { get; set; }

        public ORID ORID
        {
            get
            {
                if (Document != null)
                    return Document.ORID;

                return Object.ORID;
            }
            set
            {
                if (Document != null)
                    Document.ORID = value;
                if (Object != null)
                    Object.ORID = value;
            }
        }


        public void AddToRequest(Request request)
        {
            request.AddDataItem((byte)RecordType);
            request.AddDataItem(ORID.ClusterId);
            request.AddDataItem(ORID.ClusterPosition);
            request.AddDataItem((byte)ORecordType.Document);

            switch (RecordType)
            {
                case RecordType.Create:
                    request.AddDataItem(GetDocument().Serialize());
                    break;
                case RecordType.Delete:
                    request.AddDataItem(GetVersion());
                    break;
                case RecordType.Update:
                    request.AddDataItem(GetVersion());
                    request.AddDataItem((byte)1);
                    request.AddDataItem(GetDocument().Serialize());
                    break;

                default:
                    throw new InvalidOperationException();
            }

        }

        private int GetVersion()
        {
            if (Document != null)
                return Document.OVersion;

            return Object.OVersion;
        }

        private ODocument GetDocument()
        {
            return Document ?? (Document = ODocument.ToDocument(Object));
        }
    }
}