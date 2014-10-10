using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.API.Types
{
    internal enum ORecordFormat
    {
        ORecordDocument2csv, // default in protocol < v22
        ORecordSerializerBinary
    }
}
