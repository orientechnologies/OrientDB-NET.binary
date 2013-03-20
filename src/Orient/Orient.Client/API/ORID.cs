
namespace Orient.Client
{
    public class ORID
    {
        public short ClusterId { get; set; }
        public long ClusterPosition { get; set; }
        public string RID 
        {
            get
            {
                return "#" + ClusterId + ":" + ClusterPosition;
            }

            set
            {
                string[] split = value.Split(':');

                ClusterId = short.Parse(split[0].Substring(1));
                ClusterPosition = long.Parse(split[1]);
            } 
        }

        public ORID()
        {

        }

        public ORID(short clusterId, long clusterPosition)
        {
            ClusterId = clusterId;
            ClusterPosition = clusterPosition;
        }

        public ORID(string orid)
        {
            string[] split = orid.Split(':');

            ClusterId = short.Parse(split[0].Substring(1));
            ClusterPosition = long.Parse(split[1]);
        }

        public override string ToString()
        {
            return RID;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            // if parameter cannot be cast to ORID return false.
            ORID orid = obj as ORID;
            
            if ((System.Object)orid == null)
            {
                return false;
            }

            return (this.ToString() == orid.ToString());
        }
    }
}
