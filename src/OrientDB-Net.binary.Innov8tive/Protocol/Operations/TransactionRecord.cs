using System;
using Orient.Client.Protocol.Serializers;

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

        public int Version
        {
            get
            {
                if (Document != null)
                    return Document.OVersion;

                return Object.OVersion;
            }
            set
            {
                if (Document != null)
                    Document.OVersion = value;
                if (Object != null)
                    Object.OVersion = value;
            }
        }

        public string OClassName
        {
            get
            {
                if (Document != null)
                    return Document.OClassName;

                return Object.OClassName;
            }
        }


        public void AddToRequest(Request request)
        {
            request.AddDataItem((byte)1); // undocumented but the java code does this
            request.AddDataItem((byte)RecordType);
            request.AddDataItem(ORID.ClusterId);
            request.AddDataItem(ORID.ClusterPosition);
            request.AddDataItem((byte)ORecordType.Document);

            var serializedDocument = RecordSerializerFactory.GetSerializer(request.Connection.Database).Serialize(GetDocument());
            switch (RecordType)
            {
                case RecordType.Create:
                    request.AddDataItem(serializedDocument);
                    break;
                case RecordType.Delete:
                    request.AddDataItem(Version);
                    break;
                case RecordType.Update:
                    request.AddDataItem(Version);
                    //request.AddDataItem((byte)1);
                    request.AddDataItem(serializedDocument);
                    if (OClient.ProtocolVersion >= 23)
                    {
                        request.AddDataItem((byte)1); // updateContent flag 
                    }
                    break;

                default:
                    throw new InvalidOperationException();
            }

        }



        private ODocument GetDocument()
        {
            return Document ?? (Document = ODocument.ToDocument(Object));
        }
    }
}