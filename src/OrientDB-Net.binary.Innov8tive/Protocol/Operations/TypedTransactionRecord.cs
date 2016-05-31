namespace Orient.Client.Protocol.Operations
{
    class TypedTransactionRecord<T> : TransactionRecord where T : IBaseRecord
    {
        public TypedTransactionRecord(RecordType recordType, T typedObject)
            : base(recordType)
        {
            Object = typedObject;
        }
    }
}