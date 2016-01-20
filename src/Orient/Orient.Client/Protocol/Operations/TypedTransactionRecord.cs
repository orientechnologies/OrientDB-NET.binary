namespace Orient.Client.Protocol.Operations
{
    class TypedTransactionRecord<T> : TransactionRecord where T : IBaseRecord
    {
        public TypedTransactionRecord(RecordType recordType, T typedObject)
            : base(recordType)
        {
            this.Object = typedObject;

            var doc = typedObject as ODocument;
            if (doc != null)
            {
                this.Document = doc;
            }
        }
    }
}