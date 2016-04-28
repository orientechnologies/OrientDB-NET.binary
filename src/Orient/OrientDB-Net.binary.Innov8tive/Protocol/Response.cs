using Orient.Client.Protocol.Serializers;
using System.Linq;
using System.IO;
using System;

namespace Orient.Client.Protocol
{
    internal class Response
    {
        internal ResponseStatus Status { get; set; }
        internal int SessionId { get; set; }
        internal byte[] Token { get; set; }
        internal Connection Connection { get; private set; }
        internal BinaryReader Reader { get; private set; }

        public Response(Connection connection)
        {
            this.Connection = connection;
        }

        public void Receive()
        {
            Reader = new BinaryReader(Connection.GetNetworkStream());
            var reader = Reader;
            Status = (ResponseStatus)reader.ReadByte();
            SessionId = reader.ReadInt32EndianAware();

            //if (Connection.UseTokenBasedSession && Connection.ProtocolVersion > 26)
            //{
            //    var size = reader.ReadInt32EndianAware();
            //    Token = reader.ReadBytesRequired(size);
            //}

            if (Status == ResponseStatus.ERROR)
            {
                string exceptionString = "";

                byte followByte = reader.ReadByte();

                while (followByte == 1)
                {
                    int exceptionClassLength = reader.ReadInt32EndianAware();
                    byte[] exceptionSringByte = reader.ReadBytes(exceptionClassLength);
                    exceptionString += System.Text.Encoding.UTF8.GetString(exceptionSringByte,0, exceptionSringByte.Length) + ": ";

                    int exceptionMessageLength = reader.ReadInt32EndianAware();

                    // don't read exception message string if it's null
                    if (exceptionMessageLength != -1)
                    {
                        byte[] exceptionByte = reader.ReadBytes(exceptionMessageLength);
                        exceptionString += System.Text.Encoding.UTF8.GetString(exceptionByte,0, exceptionByte.Length) + "\n";
                    }

                    followByte = reader.ReadByte();
                }
                if (OClient.ProtocolVersion >= 19)
                {
                    int serializedVersionLength = reader.ReadInt32EndianAware();
                    var buffer = reader.ReadBytes(serializedVersionLength);
                }
                throw new OException(OExceptionType.Operation, exceptionString);
            }
        }
    }
}
