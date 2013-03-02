
namespace Orient.Client
{
    public class OCluster
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public OClusterType Type { get; set; }
        public string Location { get; set; }
        public short DataSegmentID { get; set; }
        public string DataSegmentName { get; set; }
        
        // TODO:
        /*public long RecordsCount
        {
            get
            {
                Count operation = new Count();
                operation.ClusterName = Name;

                return (long)WorkerConnection.ExecuteOperation<Count>(operation);
            }
        }

        public long[] DataRange
        {
            get
            {
                DataClusterDataRange operation = new DataClusterDataRange();
                operation.ClusterID = ID;

                return (long[])WorkerConnection.ExecuteOperation<DataClusterDataRange>(operation);
            }
        }*/
    }
}
