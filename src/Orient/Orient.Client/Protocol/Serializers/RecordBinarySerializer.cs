using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol.Serializers
{
    public class RecordBinarySerializer : IRecordSerializer
    {
        private long MILLISEC_PER_DAY = 86400000;

        public RecordBinarySerializer()
        {

        }

        #region Deserialize

        public ODocument Deserialize(byte[] recordString, ODocument document)
        {
            using (var stream = new MemoryStream(recordString))
            using (var reader = new BinaryReader(stream))
            {

                return Deserialize(reader, document);
            }
        }

        private ODocument Deserialize(BinaryReader reader, ODocument document)
        {
            var serializerVersion = reader.ReadByte();
            var classNameLength = readAsInteger(reader);
            var className = Encoding.UTF8.GetString(reader.ReadBytesRequired(classNameLength));
            document.OClassName = className;

            return parseDocument(reader, document);
        }

        private ODocument parseDocument(BinaryReader reader, ODocument document)
        {
            List<FieldDefinition> fd = new List<FieldDefinition>();
            var prop = String.Empty;

            while (true)
            {
                int len = readAsInteger(reader);
                if (len == 0)
                    break;

                if (len > 0)
                {
                    prop = Encoding.UTF8.GetString(reader.ReadBytesRequired(len));
                    int valuePos = reader.ReadInt32EndianAware();
                    byte valuetype = reader.ReadByte();
                    fd.Add(new FieldDefinition { FieldName = prop, Pointer = valuePos, DataType = (OType)valuetype });
                }
                else
                {
                    var propid = (len * -1) - 1;
                    prop = "eeee";
                    int valuePos = reader.ReadInt32EndianAware();
                    fd.Add(new FieldDefinition { FieldName = prop, Pointer = valuePos, DataType = OType.Date });
                }
            }

            foreach (var f in fd)
            {
                if (f.Pointer != 0)
                {
                    readOType(reader, f.FieldName, document, f.DataType);
                }
                else
                {
                    document[f.FieldName] = null;
                }
            }

            return document;
        }

        private void readOType(BinaryReader reader, string fieldName, ODocument document, OType type)
        {
            switch (type)
            {
                case OType.Integer:
                    document.SetField<int>(fieldName, readAsInteger(reader));
                    break;
                case OType.Long:
                    document.SetField<long>(fieldName, readAsLong(reader));
                    break;
                case OType.Short:
                    document.SetField<short>(fieldName, readAsShort(reader));
                    break;
                case OType.String:
                    document.SetField<string>(fieldName, readString(reader));
                    break;
                case OType.Double:
                    document.SetField<double>(fieldName, Convert.ToDouble(readLong(reader)));
                    break;
                case OType.Float:
                    document.SetField<float>(fieldName, readFloat(reader));
                    break;
                case OType.Byte:
                    document.SetField<byte>(fieldName, reader.ReadByte());
                    break;
                case OType.Boolean:
                    document.SetField<bool>(fieldName, reader.ReadByte() == 1 ? true : false);
                    break;
                case OType.DateTime:
                    document.SetField<DateTime>(fieldName, readDateTime(reader));
                    break;
                case OType.Date:
                    document.SetField<DateTime>(fieldName, readDate(reader));
                    break;
                case OType.EmbeddedList:
                    var listLength = readAsInteger(reader);
                    OType embeddedListRecorType = (OType)reader.ReadByte();
                    List<Object> embeddedList = new List<Object>();
                    for (int i = 0; i < listLength; i++)
                    {
                        var d = new ODocument();
                        OType dataType = (OType)reader.ReadByte();
                        readOType(reader, i.ToString(), d, dataType);
                        embeddedList.AddRange(d.Values);
                    }
                    document.SetField(fieldName, embeddedList);
                    break;
                case OType.EmbeddedSet:
                    var embeddedSetLen = readAsInteger(reader);
                    OType embeddedSetRecorType = (OType)reader.ReadByte();
                    HashSet<ODocument> embeddedSet = new HashSet<ODocument>();
                    for (int i = 0; i < embeddedSetLen; i++)
                    {
                        var d = new ODocument();
                        OType dataType = (OType)reader.ReadByte();
                        readOType(reader, "", d, dataType);
                        embeddedSet.Add(d);
                    }
                    document.SetField(fieldName, embeddedSet);
                    break;
                case OType.EmbeddedMap:
                    /* 
                     * header:headerStructure | values:valueStructure
                     * headerStructure
                     * ===============
                     * keyType:byte | keyValue:byte[]
                     * 
                     * valueStructure
                     * ==============
                     * valueType:byte | value:byte[]
                    */
                    var size = readAsInteger(reader);

                    var fd = new FieldDefinition[size];
                    Dictionary<string, Object> map = new Dictionary<string, object>();
                    for (int i = 0; i < size; i++)
                    {
                        fd[i] = new FieldDefinition();

                        var d = new ODocument();
                        var keyType = (OType)reader.ReadByte();
                        if (keyType != OType.String)
                            throw new NotImplementedException("key type " + keyType + " not implemented for EmbededMap");
                        readOType(reader, "key", d, keyType);

                        fd[i].FieldName = d.GetField<string>("key");
                        fd[i].Pointer = reader.ReadInt32EndianAware();
                        fd[i].DataType = (OType)reader.ReadByte();
                    }
                    for (int i = 0; i < size; i++)
                    {
                        var d = new ODocument();
                        if (fd[i].Pointer > 0)
                        {
                            readOType(reader, "value", d, fd[i].DataType);
                            map.Add(fd[i].FieldName, d.GetField<object>("value"));
                        }
                        else
                        {
                            map.Add(fd[i].FieldName, null);
                        }
                    }
                    document.SetField<Dictionary<string, Object>>(fieldName, map);
                    break;
                case OType.Embedded:
                    var version = reader.ReadByte();
                    parseDocument(reader, document);
                    break;
                case OType.Link:
                    var claster = readAsLong(reader);
                    var record = readAsLong(reader);
                    document.SetField(fieldName, new ORID((short)claster, record));
                    break;
                case OType.LinkBag:
                    var rids = new HashSet<ORID>();
                    var config = reader.ReadByte();
                    if ((config & 2) == 2)
                    {
                        // uuid parsing is not implemented
                        config += 16;
                    }

                    if ((config & 1) == 1) // 1 - embedded,0 - tree-based 
                    {
                        var entriesSize = reader.ReadInt32EndianAware();
                        for (int j = 0; j < entriesSize; j++)
                        {
                            var clusterid = reader.ReadInt16EndianAware();
                            var clusterposition = reader.ReadInt64EndianAware();
                            rids.Add(new ORID(clusterid, clusterposition));
                        }
                    }
                    else
                    {
                        throw new NotImplementedException("tree based ridbag");
                    }
                    document.SetField(fieldName, rids);
                    break;
                case OType.Any:
                    break;
                default:
                    throw new OException(OExceptionType.Deserialization, "The field type: " + type.ToString() + "not implemented");
            }
        }

        #endregion

        public string Serialize(ODocument document)
        {
            var buffer = new BinaryBuffer();

            // Version
            buffer.Add((byte)document.OVersion);

            // Class Name
            if (!String.IsNullOrEmpty(document.OClassName))
            {
                var length = BinarySerializer.ToArray(document.OClassName.Length);
                buffer.AddRange(length);
                var className = BinarySerializer.ToArray(document.OClassName);
                buffer.AddRange(className);
            }
            else
            {
                var length = BinarySerializer.ToArray((int)0);
                buffer.AddRange(length);
            }

            // Header
            var propNames = document.Keys.Where(k => !k.StartsWith("@")).ToArray();
            foreach (var prop in propNames)
            {

            }
            buffer.AddRange(BinarySerializer.ToArray((int)0));

            // Data
            return Encoding.UTF8.GetString(buffer.ToArray());

        }

        private string readString(BinaryReader reader)
        {
            int len = readAsInteger(reader);
            return Encoding.UTF8.GetString(reader.ReadBytesRequired(len));
        }

        private int readInteger(BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        private int readAsInteger(BinaryReader reader)
        {
            return (int)readSignedVarLong(reader);
        }

        public short readAsShort(BinaryReader reader)
        {
            return (short)readSignedVarLong(reader);
        }

        private long readLong(BinaryReader reader)
        {
            return (long)reader.ReadInt64EndianAware();
        }

        public long readAsLong(BinaryReader reader)
        {
            return readSignedVarLong(reader);
        }

        public float readFloat(BinaryReader reader)
        {
            return BitConverter.ToSingle(reader.ReadBytesRequired(sizeof(Int32)).CheckEndianess(), 0);
        }

        private long readSignedVarLong(BinaryReader reader)
        {
            long raw = readUnsignedVarLong(reader);
            long temp = (((raw << 63) >> 63) ^ raw) >> 1;
            return temp ^ (raw & (1L << 63));
        }

        private long readUnsignedVarLong(BinaryReader reader)
        {
            long value = 0L;
            int i = 0;
            long b = 0;
            while (((b = reader.ReadByte()) & 0x80) != 0)
            {
                value |= (b & 0x7F) << i;
                i += 7;
                if (i > 63)
                    throw new ArgumentOutOfRangeException("Variable length quantity is too long (must be <= 63)");

            }
            return value | (b << i);
        }

        private DateTime readDateTime(BinaryReader reader)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
            var ticks = readAsLong(reader);
            return unixEpoch.Add(TimeSpan.FromMilliseconds(ticks));
        }

        private DateTime readDate(BinaryReader reader)
        {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
            var ticks = readAsLong(reader) * MILLISEC_PER_DAY;
            return unixEpoch.Add(TimeSpan.FromMilliseconds(ticks));
        }

    }

    internal struct FieldDefinition
    {
        public string FieldName;
        public int Pointer;
        public OType DataType;
    }
}
