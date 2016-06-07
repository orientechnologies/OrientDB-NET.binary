
namespace Orient.Client.Protocol
{
    internal enum PayloadStatus
    {
        NoRemainingRecords = 0,
        ResultSet = 1,
        PreFetched = 2,
        NullResult = 110, // 'n'
        SingleRecord = 114, // 'r'
        SerializedResult = 97, // 'a'
        RecordCollection = 108 // 'l'
    }
}
