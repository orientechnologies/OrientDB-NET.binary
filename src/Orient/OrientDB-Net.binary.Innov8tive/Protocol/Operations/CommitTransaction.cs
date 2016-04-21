using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{


    class CommitTransaction : BaseOperation
    {
        private List<TransactionRecord> _records;

        public CommitTransaction(List<TransactionRecord> records, ODatabase database)
            : base(database)
        {
            _records = records;
            _operationType = OperationType.TX_COMMIT;
        }

        public override Request Request(Request request)
        {
            //if (_document.ORID != null)
            //    throw new InvalidOperationException();

            //CorrectClassName();

            //string className = _document.OClassName.ToLower();
            //var clusterId = _database.GetClusters().First(x => x.Name == className).Id;
            //_document.ORID = new ORID(clusterId, -1);

            base.Request(request);
            int transactionId = 1;

            request.AddDataItem(transactionId);
            request.AddDataItem((byte)(UseTransactionLog ? 1 : 0)); // use log 0 = no, 1 = yes

            foreach (var item in _records)
            {

                item.AddToRequest(request);
            }

            request.AddDataItem((byte)0); // zero terminated

            request.AddDataItem((int)0);

            return request;
        }

        public override ODocument Response(Response response)
        {
            ODocument responseDocument = new ODocument();

            var reader = response.Reader;
            if (response.Connection.ProtocolVersion > 26 && response.Connection.UseTokenBasedSession)
                ReadToken(reader);

            var createdRecordMapping = new Dictionary<ORID, ORID>();
            int recordCount = reader.ReadInt32EndianAware();
            for (int i = 0; i < recordCount; i++)
            {
                var tempORID = ReadORID(reader);
                var realORID = ReadORID(reader);
                createdRecordMapping.Add(tempORID, realORID);
            }
            responseDocument.SetField("CreatedRecordMapping", createdRecordMapping);

            int updatedCount = reader.ReadInt32EndianAware();
            var updateRecordVersions = new Dictionary<ORID, int>();
            for (int i = 0; i < updatedCount; i++)
            {
                var orid = ReadORID(reader);
                var newVersion = reader.ReadInt32EndianAware();
                updateRecordVersions.Add(orid, newVersion);
            }
            responseDocument.SetField("UpdatedRecordVersions", updateRecordVersions);

            // Work around differents in storage type < version 2.0
            if (_database.ProtocolVersion >= 28 || (_database.ProtocolVersion >= 20 && _database.ProtocolVersion <= 27 && !EndOfStream(reader)))
            {
                int collectionChanges = reader.ReadInt32EndianAware();
                if (collectionChanges > 0)
                    throw new NotSupportedException("Processing of collection changes is not implemented. Failing rather than ignoring potentially significant data");

                //for (int i = 0; i < collectionChanges; i++)
                //{
                //    long mBitsOfId = reader.ReadInt64EndianAware();
                //    long lBitsOfId = reader.ReadInt64EndianAware();
                //    var updatedFileId = reader.ReadInt64EndianAware();
                //    var updatedPageIndex = reader.ReadInt64EndianAware();
                //    var updatedPageOffset = reader.ReadInt32EndianAware();
                //}
            }


            return responseDocument;
        }

        private ORID ReadORID(BinaryReader reader)
        {
            ORID result = new ORID();
            result.ClusterId = reader.ReadInt16EndianAware();
            result.ClusterPosition = reader.ReadInt64EndianAware();
            return result;
        }


        public bool UseTransactionLog { get; set; }
    }
}
