using System.Collections.Generic;
using System.Linq;
using Orient.Client.Protocol;

namespace Orient.Client
{
    public class OCommandResult
    {
        private ODataObject _dataObject;

        internal OCommandResult(ODataObject dataObject)
        {
            _dataObject = dataObject;
        }

        public ORecord ToSingle()
        {
            ORecord record = null;

            switch (_dataObject.Get<PayloadStatus>("PayloadStatus"))
            {
                case PayloadStatus.SingleRecord:
                    record = _dataObject.Get<ORecord>("Content");
                    break;
                case PayloadStatus.RecordCollection:
                    record = _dataObject.Get<List<ORecord>>("Content").FirstOrDefault();
                    break;
                default:
                    break;
            }

            return record;
        }

        public List<ORecord> ToList()
        {
            return _dataObject.Get<List<ORecord>>("Content");
        }
    }
}
