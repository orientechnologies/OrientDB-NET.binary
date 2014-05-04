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
