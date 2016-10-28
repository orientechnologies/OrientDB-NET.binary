using System.Collections.Generic;
using System.Linq;
using Orient.Client.API.Types;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client
{
    public static class OClient
    {
        private static object _syncRoot;
        internal static string ClientID { get; set; }
        private static short _protocolVersion = 35;
        public static string DriverName { get { return "OrientDB-NET.binary"; } }
        public static string DriverVersion { get { return "0.2.2"; } }

        public static short ProtocolVersion
        {
            get { return _protocolVersion; }
            set
            {
                if (value != _protocolVersion)
                    _protocolVersion = value;
            }
        }

        public static int BufferLength { get; set; }
        public static ORecordFormat Serializer { get; set; }
        public static bool UseTokenBasedSession { get; set; }

        public static string SerializationImpl { get { return Serializer.ToString(); } }

        static OClient()
        {
            _syncRoot = new object();
            BufferLength = 1024;
            Serializer = ORecordFormat.ORecordDocument2csv;
            ClientID = "null";
            /* 
              If you enable token based session make shure enable it in server config
              <!-- USE SESSION TOKEN, TO TURN ON SET THE 'ENABLED' PARAMETER TO 'true' -->
              <handler class="com.orientechnologies.orient.server.token.OrientTokenHandler">
                  <parameters>
                      <parameter name="enabled" value="true"/>
                      <!-- PRIVATE KEY -->
                      <parameter name="oAuth2Key" value="GVsbG8gd29ybGQgdGhpcyBpcyBteSBzZWNyZXQgc2VjcmV0"/>
                      <!-- SESSION LENGTH IN MINUTES, DEFAULT=1 HOUR -->
                      <parameter name="sessionLength" value="60"/>
                      <!-- ENCRYPTION ALGORITHM, DEFAULT=HmacSHA256 -->
                      <parameter name="encryptionAlgorithm" value="HmacSHA256"/>
                  </parameters>            
               </handler>
             */
            UseTokenBasedSession = false;
        }
    }
}
