using System.Collections.Generic;
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

            //request.DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(-1) });  // data segment id
            //request.DataItems.Add(new RequestDataItem() { Type = "short", Data = BinarySerializer.ToArray((short)-1) });
            //request.DataItems.Add(new RequestDataItem() {Type = "string", Data = BinarySerializer.ToArray(_document.Serialize())});
            //request.DataItems.Add(new RequestDataItem() {Type = "byte", Data = BinarySerializer.ToArray((byte) 'd')});
            //request.DataItems.Add(new RequestDataItem() {Type = "byte", Data = BinarySerializer.ToArray((byte) 0)});


            return request;
        }



        public ODocument Response(Response response)
        {
            ODocument responseDocument = null;



            return responseDocument;


        }
    }
}
