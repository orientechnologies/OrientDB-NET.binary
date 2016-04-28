using System;
using System.Collections.Generic;
using System.Linq;
using Orient.Client.Protocol;

namespace Orient.Client
{
    public class OCommandResult
    {
        private ODocument _document;

        internal OCommandResult(ODocument document)
        {
            _document = document;
        }

        public int GetModifiedCount()
        {
            switch (_document.GetField<PayloadStatus>("PayloadStatus"))
            {
                case PayloadStatus.SingleRecord:
                    return 1;
                case PayloadStatus.RecordCollection:
                    return _document.GetField<List<ODocument>>("Content").Count;
                case PayloadStatus.SerializedResult:
                    return Convert.ToInt32(_document.GetField<object>("Content"));
            }

            return 0;
        }

        public ODocument ToSingle()
        {
            ODocument document = null;

            switch (_document.GetField<PayloadStatus>("PayloadStatus"))
            {
                case PayloadStatus.SingleRecord:
                    document = _document.GetField<ODocument>("Content");
                    break;
                case PayloadStatus.RecordCollection:
                    document = _document.GetField<List<ODocument>>("Content").FirstOrDefault();
                    break;
                case PayloadStatus.SerializedResult:
                    document = new ODocument();
                    document.SetField<object>("value",_document.GetField<object>("Content"));
                    break;
                default:
                    break;
            }

            return document;
        }

        public List<ODocument> ToList()
        {
            return _document.GetField<List<ODocument>>("Content");
        }

        public ODocument ToDocument()
        {
            return _document;
        }
    }
}
