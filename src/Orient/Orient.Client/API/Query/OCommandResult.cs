using System.Collections.Generic;
using System.Linq;
using Orient.Client.Protocol;

namespace Orient.Client.Sql
{
    public class OCommandResult
    {
        private ODataObject _dataObject;

        public string Sql { get; internal set; }

        internal OCommandResult(ODataObject dataObject)
        {
            Sql = "";
            _dataObject = dataObject;
        }

        internal OCommandResult(string sql, ODataObject dataObject)
        {
            Sql = sql;
            _dataObject = dataObject;
        }

        public ODataObject ToDataObject()
        {
            return _dataObject;
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
