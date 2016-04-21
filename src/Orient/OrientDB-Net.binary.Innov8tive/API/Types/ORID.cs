using System;

namespace Orient.Client
{
    public class ORID : IEquatable<ORID>
    {
        static readonly char[] colon = new char[] { ':' };
        public short ClusterId { get; set; }
        public long ClusterPosition { get; set; }
        public string RID
        {
            get
            {
                return string.Format("#{0}:{1}", ClusterId, ClusterPosition);
            }

            set
            {
                int offset = 1;
                ClusterId = (short)FastParse(value, ref offset);
                offset += 1;
                ClusterPosition = FastParse(value, ref offset);

                //ClusterId = short.Parse(split[0].Substring(1));
                //ClusterPosition = long.Parse(split[1]);
            }
        }

        long FastParse(string s, ref int offset)
        {
            long result = 0;
            short multiplier = 1;
            if (s[offset] == '-')
            {
                offset++;
                multiplier = -1;
            }

            while (offset < s.Length)
            {
                int iVal = s[offset] - '0';
                if (iVal < 0 || iVal > 9)
                    break;
                result = result * 10 + iVal;
                offset++;
            }

            return (result * multiplier);
        }

        public ORID()
        {
            ClusterId = -1;
            ClusterPosition = -1;
        }

        public ORID(ORID other)
        {
            ClusterId = other.ClusterId;
            ClusterPosition = other.ClusterPosition;
        }

        public ORID(short clusterId, long clusterPosition)
        {
            ClusterId = clusterId;
            ClusterPosition = clusterPosition;
        }

        public ORID(string orid)
        {
            RID = orid;
        }

        public ORID(string source, int offset)
        {
            if (source[offset] == '#')
                offset++;
            ClusterId = (short)FastParse(source, ref offset);
            offset += 1;
            ClusterPosition = FastParse(source, ref offset);
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

            if (orid == null)
            {
                return false;
            }

            return Equals(orid);
        }

        public override int GetHashCode()
        {
            return (ClusterId * 17) ^ ClusterPosition.GetHashCode();
        }

        public static bool operator ==(ORID left, ORID right)
        {
            if (System.Object.ReferenceEquals(left, right))
            {
                return true;
            }

            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(ORID left, ORID right)
        {
            return !(left == right);
        }

        public bool Equals(ORID other)
        {
            if (other == null)
                return false;

            return ClusterId == other.ClusterId && ClusterPosition == other.ClusterPosition;
        }
    }
}
