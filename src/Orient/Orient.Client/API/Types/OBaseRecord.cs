using Orient.Client;

namespace Orient.Client
{
    public abstract class OBaseRecord
    {
        private string _oClassName;

        #region Orient record specific properties

        public ORID ORID { get; set; }

        public int OVersion { get; set; }

        public short OClassId { get; set; }

        public string OClassName {
            get
            {
                if (string.IsNullOrEmpty(_oClassName))
                {
                    return GetType().Name;
                }

                return _oClassName;
            }

            set
            {
                _oClassName = value;
            }
        }

        #endregion

        public ODocument ToDocument()
        {
            return ODocument.ToDocument(this);
        }
    }
}
