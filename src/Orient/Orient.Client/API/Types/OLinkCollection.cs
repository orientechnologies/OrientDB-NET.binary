using System.Collections.Generic;

namespace Orient.Client
{
    internal class OLinkCollection : List<ORID>
    {
        internal int PageSize { get; set; }
        internal ORID Root { get; set; }
        internal int KeySize { get; set; }
    }
}
