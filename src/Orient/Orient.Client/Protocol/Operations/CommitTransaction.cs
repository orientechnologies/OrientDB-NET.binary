using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
   

    class CommitTransaction : IOperation
    {
        private readonly ODatabase _database;
        private List<TransactionRecord> _records;

        public CommitTransaction(List<TransactionRecord> records, ODatabase database)
        {
            _records = records;
            _database = database;
        }

        public Request Request(int sessionID)
        {
            Request request = new Request();

            //if (_document.ORID != null)
            //    throw new InvalidOperationException();

            //CorrectClassName();

            //string className = _document.OClassName.ToLower();
            //var clusterId = _database.GetClusters().First(x => x.Name == className).Id;
            //_document.ORID = new ORID(clusterId, -1);

            // standard request fields
            int transactionId = 1;

            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)OperationType.TX_COMMIT) });
            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(sessionID) });

            request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(transactionId) });
            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)0) }); // use log 0 = no, 1 = yes

            foreach (var item in _records)
            {

                item.AddToRequest(request);
            }

            request.DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray((byte)0) }); // zero terminated

            request.AddDataItem((int) 0);

            //request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(-1) });  // data segment id
            //request.DataItems.Add(new RequestDataItem() { Type = "short", Data = BinarySerializer.ToArray((short)-1) });
            //request.DataItems.Add(new RequestDataItem() {Type = "string", Data = BinarySerializer.ToArray(_document.Serialize())});
            //request.DataItems.Add(new RequestDataItem() {Type = "byte", Data = BinarySerializer.ToArray((byte) 'd')});
            //request.DataItems.Add(new RequestDataItem() {Type = "byte", Data = BinarySerializer.ToArray((byte) 0)});


            return request;
        }



        public ODocument Response(Response response)
        {
            ODocument responseDocument = new ODocument();

            var reader = response.Reader;

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

            //int collectionChangesCount = reader.ReadInt32EndianAware();
            //if (collectionChangesCount > 0)
            //    throw new NotImplementedException("don't understand what to do with this yet");

            return responseDocument;
        }

        private ORID ReadORID(BinaryReader reader)
        {
            ORID result =new ORID();
            result.ClusterId = reader.ReadInt16EndianAware();
            result.ClusterPosition = reader.ReadInt64EndianAware();
            return result;
        }
    }
}
