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

        public ODocument ToDocument()
        {
            return _document;
        }

        public ORecord ToSingle()
        {
            ORecord record = null;

            switch (_document.GetField<PayloadStatus>("PayloadStatus"))
            {
                case PayloadStatus.SingleRecord:
                    record = _document.GetField<ORecord>("Content");
                    break;
                case PayloadStatus.RecordCollection:
                    record = _document.GetField<List<ORecord>>("Content").FirstOrDefault();
                    break;
                default:
                    break;
            }

            return record;
        }

        public List<ORecord> ToList()
        {
            return _document.GetField<List<ORecord>>("Content");
        }
    }
}
