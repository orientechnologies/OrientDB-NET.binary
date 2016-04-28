using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;
using Orient.Client.Transactions;

namespace Orient.Client.API
{


    public class OTransaction
    {
        private readonly Connection _connection;

        internal OTransaction(Connection connection)
        {
            _connection = connection;
            _tempClusterId = -1;
            _tempObjectId = -1;

            /*
             * using-tx-log tells if the server must use the Transaction Log to recover the transaction. 
             * 1 = true, 0 = false. 
             * Use always 1 (true) by default to assure consistency. 
             * NOTE: Disabling the log could speed up execution of transaction, 
             * but can't be rollbacked in case of error. 
             * This could bring also at inconsistency in indexes as well, 
             * because in case of duplicated keys the rollback is not called to restore the index status.
             */
            UseTransactionLog = true;
        }

        private readonly Dictionary<ORID, TransactionRecord> _records = new Dictionary<ORID, TransactionRecord>();

        internal bool UseTransactionLog { get; set; }

        private readonly short _tempClusterId;

        private long _tempObjectId;

        public void Commit()
        {
            CommitTransaction ct = new CommitTransaction(_records.Values.ToList(), _connection.Database);
            ct.UseTransactionLog = UseTransactionLog;
            var result = _connection.ExecuteOperation(ct);
            Dictionary<ORID, ORID> mapping = result.GetField<Dictionary<ORID, ORID>>("CreatedRecordMapping");

            var survivingRecords = _records.Values.Where(x => x.RecordType != RecordType.Delete).ToList();

            foreach (var kvp in mapping)
            {
                var record = _records[kvp.Key];
                record.ORID = kvp.Value;
                _records.Add(record.ORID, record);
                if (record.Document != null)
                    _connection.Database.ClientCache.Add(kvp.Value, record.Document);
            }

            var versions = result.GetField<Dictionary<ORID, int>>("UpdatedRecordVersions");
            foreach (var kvp in versions)
            {
                var record = _records[kvp.Key];
                record.Version = kvp.Value;
            }

            foreach (var record in survivingRecords)
            {
                if (record.Object != null)
                {
                    ORIDUpdaterBase.GetInstanceFor(record.Object.GetType()).UpdateORIDs(record.Object, mapping);
                }
                else
                {
                    ORIDUpdaterBase.GetInstanceFor(record.Document.GetType()).UpdateORIDs(record.Document, mapping);
                }


            }

            Reset();
        }

        public void Reset()
        {
            _records.Clear();
        }

        public void Add<T>(T typedObject) where T : IBaseRecord
        {
            var record = new TypedTransactionRecord<T>(RecordType.Create, typedObject);
            Insert(record);
        }

        public void AddEdge(OEdge edge, OVertex from, OVertex to)
        {
            this.Add(edge);
            edge.SetField("out", from.ORID);
            edge.SetField("in", to.ORID);

            appendOridToField(from, "out_" + edge.OClassName, edge.ORID);
            appendOridToField(to, "in_" + edge.OClassName, edge.ORID);

            if (!_records.ContainsKey(from.ORID))
                Update(from);

            if (!_records.ContainsKey(to.ORID))
                Update(to);
        }

        private void appendOridToField(ODocument document, string field, ORID orid)
        {
            if (document.HasField(field))
            {
                document.GetField<HashSet<ORID>>(field).Add(orid);
            }
            else
            {
                var orids = new HashSet<ORID>();
                orids.Add(orid);
                document.SetField(field, orids);
            }
        }

        public void Update<T>(T typedObject) where T : IBaseRecord
        {
            var record = new TypedTransactionRecord<T>(RecordType.Update, typedObject);
            Insert(record);
        }

        public void Delete<T>(T typedObject) where T : IBaseRecord
        {
            var record = new TypedTransactionRecord<T>(RecordType.Delete, typedObject);
            Insert(record);
        }

        private void Insert(TransactionRecord record)
        {
            bool hasOrid = record.ORID != null;
            bool needsOrid = record.RecordType != RecordType.Create;

            if (hasOrid && !needsOrid)
                throw new InvalidOperationException("Objects to be added via a transaction must not already be in the database");

            if (needsOrid && !hasOrid)
                throw new InvalidOperationException("Objects to be updated or deleted via a transaction must already be in the database");

            if (!hasOrid)
            {
                record.ORID = CreateTempORID();
                record.ORID.ClusterId = _connection.Database.GetClusterIdFor(record.OClassName);
            }

            if (_records.ContainsKey(record.ORID))
            {
                if (record.RecordType != _records[record.ORID].RecordType)
                    throw new InvalidOperationException("Same object already part of transaction with a different CRUD intent");
                _records[record.ORID] = record;
            }
            else
            {
                _records.Add(record.ORID, record);
            }
        }

        private ORID CreateTempORID()
        {
            return new ORID(_tempClusterId, --_tempObjectId);
        }

        public T GetPendingObject<T>(ORID orid) where T : IBaseRecord
        {
            TransactionRecord record;
            if (_records.TryGetValue(orid, out record))
            {
                return (T)record.Object;
            }
            return default(T);
        }

        public void AddOrUpdate<T>(T target) where T : IBaseRecord
        {
            if (target.ORID == null)
                Add(target);
            else if (!_records.ContainsKey(target.ORID))
                Update(target);
        }
    }
}
