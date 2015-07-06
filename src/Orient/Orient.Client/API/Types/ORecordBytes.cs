using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client
{
    /// <summary>
    /// Raw record.
    /// </summary>
    public class ORecordBytes : IBaseRecord
    {

#region Properties which holds orient specific fields

        public ORID ORID { get; set; }

        public int OVersion { get; set; }

        public short OClassId { get; set; }

        public string OClassName { get; set; }

        public ORecordType ORecordType
        {
            get { return ORecordType.RawBytes; }
            set { }
        }

#endregion

        public byte[] Content { get; set; }
    }
}
