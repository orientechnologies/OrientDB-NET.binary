using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.API.Types
{
    public class ODatabaseInfo
    {
        public string DataBaseName { get; set; }
        public OStorageType StorageType { get; set; }
        public string Path { get; set; }
    }
}
