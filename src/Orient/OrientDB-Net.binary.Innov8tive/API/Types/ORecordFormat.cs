using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.API.Types
{
    public enum ORecordFormat
    {
        ORecordDocument2csv, // default in protocol < v22
        ORecordSerializerBinary
    }
}
