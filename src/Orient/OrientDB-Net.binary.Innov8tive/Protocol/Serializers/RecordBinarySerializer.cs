using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Orient.Client.API.Types;

namespace Orient.Client.Protocol.Serializers
{
    internal class RecordBinarySerializer : IRecordSerializer
    {
        private long MILLISEC_PER_DAY = 86400000;
        private const int SERIALIZER_VERSION = 0;
        private ORID NULL_RECORD_ID = new ORID(-2, -1);
        private Connection _connection;

        public RecordBinarySerializer(Connection connection)
        {
            _connection = connection;
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
            byte[] rawBinary = reader.ReadBytesRequired(classNameLength);
            var className = Encoding.UTF8.GetString(rawBinary,0,rawBinary.Length);
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
                    byte[] rawBinary = reader.ReadBytesRequired(len);
                    prop = Encoding.UTF8.GetString(rawBinary,0,rawBinary.Length);
                    int valuePos = reader.ReadInt32EndianAware();
                    OType valuetype = (OType)reader.ReadByte();
                    fd.Add(new FieldDefinition { FieldName = prop, Pointer = valuePos, DataType = valuetype });
                }
                else
                {
                    //TODO: Implement schema aware read
                    var propid = (len * -1) - 1;
                    prop = "no_schema";
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
                    document.SetField<double>(fieldName, BitConverter.Int64BitsToDouble(readLong(reader)));
                    break;
                case OType.Float:
                    document.SetField<float>(fieldName, readFloat(reader));
                    break;
                case OType.Decimal:
                    var scale = reader.ReadInt32EndianAware();

                    var valueSize = reader.ReadInt32EndianAware();

                    // read Fine the value
                    var valuex = reader.ReadBytesRequired(valueSize);

                    Int64 x1 = 0;

                    if ((valuex[0] & 0x80) == 0x80)
                        x1 = (sbyte)valuex[0];
                    else
                        x1 = valuex[0];

                    for (int i = 1; i < valuex.Length; i++)
                    {
                        x1 = (x1 << 8) | valuex[i];
                    }

                    try
                    {
                        document.SetField(fieldName, new Decimal(x1 * Math.Pow(10, (-1) * scale)));
                    }
                    catch (OverflowException)
                    {
                        document.SetField(fieldName, x1 * Math.Pow(10, (-1) * scale));
                    }
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
                case OType.LinkList:
                    var linkList = readLinkCollection(reader);
                    document.SetField(fieldName, linkList);
                    break;
                case OType.LinkSet:
                    var linkSet = new HashSet<ORID>(readLinkCollection(reader));
                    document.SetField(fieldName, linkSet);
                    break;
                case OType.Any:
                    break;
                default:
                    throw new OException(OExceptionType.Deserialization, "The field type: " + type.ToString() + "not implemented");
            }
        }

        private IEnumerable<ORID> readLinkCollection(BinaryReader reader)
        {
            var links = new List<ORID>();
            var collectionLength = readAsLong(reader);
            for (int i = 0; i < collectionLength; i++)
            {
                var clusterid = readAsInteger(reader);
                var clusterPosition = readAsLong(reader);
                if (clusterid == NULL_RECORD_ID.ClusterId && clusterPosition == NULL_RECORD_ID.ClusterPosition)
                    links.Add(null);
                else
                    links.Add(new ORID((short)clusterid, clusterPosition));
            }
            return links;
        }

        #endregion

        #region Serialize

        public byte[] Serialize(ODocument document)
        {
            var buffer = new BinaryBuffer();
            ODocument schema = null; // Until Implementing schema this is null class

            // Version
            buffer.Add((byte)SERIALIZER_VERSION);

            // Class Name
            if (!String.IsNullOrEmpty(document.OClassName))
            {
                buffer.WriteVariant(document.OClassName.Length);
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
            var pos = new int[propNames.Length];
            var val = new KeyValuePair<string, Object>[propNames.Length];
            for (var i = 0; i < propNames.Length; i++)
            {
                var prop = propNames[i];
                if (schema != null)
                {

                }
                else
                {
                    buffer.WriteVariant(prop.Length);
                    var propName = BinarySerializer.ToArray(prop);
                    buffer.AddRange(propName);
                    pos[i] = buffer.Allocate(sizeof(int) + 1);
                }
                val[i] = new KeyValuePair<string, object>(prop, document.GetField<object>(prop));
            }

            buffer.WriteVariant(0); // Header stop byte

            // Data
            for (int i = 0; i < val.Length; i++)
            {
                int pointer = 0;
                var value = val[i].Value;
                if (value != null)
                {
                    var valueType = TypeConverter.TypeToDbName(value.GetType());
                    pointer = writeOType(buffer, value, valueType, getLinkedType(valueType, val[i].Key));
                    buffer.Write(pos[i], pointer);
                    if (schema == null)
                    {
                        buffer.Write((pos[i] + sizeof(int)), (byte)valueType);
                    }
                }
            }
            return buffer.ToArray();

        }

        private int writeOType(BinaryBuffer buffer, Object value, OType valueType, OType? linkedType)
        {
            int pointer = 0;
            switch (valueType)
            {
                case OType.Integer:
                case OType.Long:
                case OType.Short:
                    pointer = buffer.WriteVariant(Convert.ToInt32(value));
                    break;
                case OType.String:
                    pointer = buffer.Write((string)value);
                    break;
                case OType.Double:
                    pointer = buffer.Write((double)value);
                    break;
                case OType.Float:
                    pointer = buffer.Write((float)value);
                    break;
                case OType.Byte:
                    pointer = buffer.Write((byte)value);
                    break;
                case OType.Boolean:
                    pointer = buffer.Write(((bool)value) ? (byte)1 : (byte)0);
                    break;
                case OType.DateTime:
                    DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    pointer = buffer.WriteVariant((long)((DateTime)value - unixEpoch).TotalMilliseconds);
                    break;
                //case OType.Decimal:
                //    /*
                //     * The Decimal is converted to an integer and stored as scale and value 
                //     * (example "10234.546" is stored as scale "3" and value as:"10234546")
                //     *  +---------------+-------------------+--------------+
                //     *  | scale:byte[4] | valueSize:byte[4] | value:byte[] |
                //     *  +---------------+-------------------+--------------+
                //     *  scale an 4 byte integer that represent the scale of the value 
                //     *  valueSize the length of the value 
                //     *  bytes value the bytes that represent the value of the   decimal in big-endian order.
                //     */
                //    var dec = ((decimal)value);
                //    byte[] bytes = BinarySerializer.ToArray(dec);
                //    var unscaledValueBytes = FromDecimal(dec);
                //    var unscaledValue = new BigInteger(unscaledValueBytes);
                //    break;
                case OType.Link:
                    ORID rid = (ORID)value;
                    pointer = buffer.Write(rid);
                    break;
                case OType.LinkList:
                case OType.LinkSet:
                    var col = (ICollection<ORID>)value;
                    pointer = buffer.WriteVariant(col.Count);

                    foreach (var item in col)
                    {
                        if (item == null)
                        {
                            buffer.Write(NULL_RECORD_ID);
                        }
                        else
                        {
                            buffer.Write(item);
                        }
                    }
                    break;
                case OType.LinkBag:
                    break;
                default:
                    throw new NotImplementedException("Type " + valueType + " still not supported");
            }
            return pointer;
        }

        #endregion
        private static byte[] FromDecimal(decimal d)
        {
            byte[] bytes = new byte[16];

            int[] bits = decimal.GetBits(d);
            int lo = bits[0];
            int mid = bits[1];
            int hi = bits[2];
            int flags = bits[3];

            bytes[0] = (byte)lo;
            bytes[1] = (byte)(lo >> 8);
            bytes[2] = (byte)(lo >> 0x10);
            bytes[3] = (byte)(lo >> 0x18);
            bytes[4] = (byte)mid;
            bytes[5] = (byte)(mid >> 8);
            bytes[6] = (byte)(mid >> 0x10);
            bytes[7] = (byte)(mid >> 0x18);
            bytes[8] = (byte)hi;
            bytes[9] = (byte)(hi >> 8);
            bytes[10] = (byte)(hi >> 0x10);
            bytes[11] = (byte)(hi >> 0x18);
            bytes[12] = (byte)flags;
            bytes[13] = (byte)(flags >> 8);
            bytes[14] = (byte)(flags >> 0x10);
            bytes[15] = (byte)(flags >> 0x18);

            return bytes;
        }

        private OType? getLinkedType(OType type, String key)
        {
            if (type != OType.EmbeddedList && type != OType.EmbeddedSet && type != OType.EmbeddedMap)
                return null;
            throw new NotImplementedException("Linked Type still not implemented");
        }

        private string readString(BinaryReader reader)
        {
            int len = readAsInteger(reader);
            byte[] rawBinary = reader.ReadBytesRequired(len);
            return Encoding.UTF8.GetString(rawBinary,0,rawBinary.Length);
        }

        private int readInteger(BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        private int readAsInteger(BinaryReader reader)
        {
            return (int)readSignedVarLong(reader);
        }

        private short readAsShort(BinaryReader reader)
        {
            return (short)readSignedVarLong(reader);
        }

        private long readLong(BinaryReader reader)
        {
            return (long)reader.ReadInt64EndianAware();
        }

        private long readAsLong(BinaryReader reader)
        {
            return readSignedVarLong(reader);
        }

        private float readFloat(BinaryReader reader)
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
