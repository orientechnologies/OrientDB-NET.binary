using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Orient.Client.Protocol;

namespace Orient.Client.API.Types
{
    public class OToken
    {
        private byte[] rawToken;
        private byte type;
        private byte key;
        private byte algorithm;
        private string database;
        private string serverUserName;
        private short protocolVersion;
        private string serializer;
        private string driverName;
        private string driverVersion;
        private byte[] signature;
        private ORID databaseUserRid;
        private DateTime tokenExpire;
        private sbyte databaseType;

        public static OToken Parse(byte[] buffer)
        {
            var token = new OToken();
            token.rawToken = buffer;

            if (buffer.Length <= 0)
                return null;

            using (var stream = new MemoryStream(buffer))
            using (var reader = new BinaryReader(stream))
            {
                token.type = reader.ReadByte();
                token.key = reader.ReadByte();
                token.algorithm = reader.ReadByte();
                token.database = ReadString(reader);

                var pos = (sbyte)reader.ReadByte();

                if (pos >= 0)
                {
                    token.databaseType = pos;
                }
                var clusterid = reader.ReadInt16EndianAware();
                var clusterpos = reader.ReadInt64EndianAware();

                if (clusterid != -1 && clusterpos != -1)
                {
                    token.databaseUserRid = new ORID(clusterid, clusterpos);
                }

                var tokenExpire = reader.ReadInt64EndianAware();
                var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                token.tokenExpire = unixEpoch.AddMilliseconds(tokenExpire).ToLocalTime();

                var serverUser = reader.ReadBoolean();
                if (serverUser)
                {
                    token.serverUserName = ReadString(reader);
                }

                token.protocolVersion = reader.ReadInt16EndianAware();
                token.serializer = ReadString(reader);
                token.driverName = ReadString(reader);
                token.driverVersion = ReadString(reader);

                var p = reader.BaseStream.Position;
                var l = reader.BaseStream.Length;

                token.signature = reader.ReadBytes((int)(l - p));
            }

            return token;
        }

        private static string ReadString(BinaryReader reader)
        {
            var length = reader.ReadInt16EndianAware();
            if (length > 0)
            {
                byte[] rawBytes = reader.ReadBytes(length);
                return Encoding.UTF8.GetString(rawBytes,0,rawBytes.Length);
            }

            return "";
        }
    }
}
