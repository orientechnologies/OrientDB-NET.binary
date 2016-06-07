using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Orient.Client.Protocol.Operations;
using Orient.Client.API.Types;
using System.Reflection;
using System.Runtime.Serialization;

namespace Orient.Client.Protocol.Serializers
{
    internal class RecordCSVSerializer : IRecordSerializer
    {
        private Connection _connection;
        public RecordCSVSerializer(Connection connection)
        {
            _connection = connection;
        }

        public byte[] Serialize(ODocument document)
        {
            if (!document.HasField("@OClassName"))
            {
                throw new OException(OExceptionType.Serialization, "Document doesn't contain @OClassName field which is required for serialization.");
            }

            return Encoding.UTF8.GetBytes(document.GetField<string>("@OClassName") + "@" + SerializeDocument(document));
        }

        #region Deserialize

        internal ODocument Deserialize(ORID orid, int version, ORecordType type, short classId, byte[] rawRecord)
        {
            ODocument document = new ODocument();
            document.ORID = orid;
            document.OVersion = version;
            document.OType = type;
            document.OClassId = classId;

            string recordString = BinarySerializer.ToString(rawRecord).Trim();

            return Deserialize(recordString, document);
        }

        internal ODocument Deserialize(string recordString, ODocument document)
        {
            int atIndex = recordString.IndexOf('@');
            int colonIndex = recordString.IndexOf(':');
            int index = 0;

            // parse class name
            if ((atIndex != -1) && (atIndex < colonIndex))
            {
                document.OClassName = recordString.Substring(0, atIndex);
                index = atIndex + 1;
            }

            // start document parsing with first field name
            do
            {
                index = ParseFieldName(index, recordString, document);
            } while (index < recordString.Length);

            return document;
        }

        internal ODocument Deserialize(string recordString)
        {
            return Deserialize(recordString, new ODocument());

        }

        public ODocument Deserialize(byte[] rawRecord, ODocument document)
        {
            string recordString = BinarySerializer.ToString(rawRecord).Trim();
            return Deserialize(recordString, document);
        }

        #endregion

        #region Serialization private methods

        private string SerializeDocument(ODocument document)
        {
            StringBuilder bld = new StringBuilder();

            if (document.Keys.Count > 0)
            {
                foreach (KeyValuePair<string, object> field in document)
                {
                    // serialize only fields which doesn't start with @ character
                    if ((field.Key.Length > 0) && (field.Key[0] != '@'))
                    {
                        if (bld.Length > 0)
                            bld.Append(",");


                        bld.AppendFormat("{0}:{1}", field.Key, SerializeValue(field.Value));
                    }

                }
            }


            return bld.ToString();
        }

        private string SerializeValue(object value)
        {
            if (value == null)
                return string.Empty;

            var bytes = value as byte[];
            if (bytes != null)
            {
                return "_" + Convert.ToBase64String(bytes) + "_";
            }

            Type valueType = value.GetType();

            switch (System.Runtime.Serialization.TypeExtensionMethods.GetTypeCode(valueType))
            {
                case TypeCode.Empty:
                    // null case is empty
                    break;
                case TypeCode.Boolean:
                    return value.ToString().ToLower();
                case TypeCode.Byte:
                    return value.ToString() + "b";
                case TypeCode.Int16:
                    return value.ToString() + "s";
                case TypeCode.Int32:
                    return value.ToString();
                case TypeCode.Int64:
                    return value.ToString() + "l";
                case TypeCode.Single:
                    return ((float)value).ToString(CultureInfo.InvariantCulture) + "f";
                case TypeCode.Double:
                    return ((double)value).ToString(CultureInfo.InvariantCulture) + "d";
                case TypeCode.Decimal:
                    return ((decimal)value).ToString(CultureInfo.InvariantCulture) + "c";
                case TypeCode.DateTime:
                    DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    return ((long)((DateTime)value - unixEpoch).TotalMilliseconds).ToString() + "t";
                case TypeCode.String:
                case TypeCode.Char:
                    // strings must escape these characters:
                    // " -> \"
                    // \ -> \\
                    string stringValue = value.ToString();
                    // escape quotes
                    stringValue = stringValue.Replace("\\", "\\\\");
                    // escape backslashes
                    stringValue = stringValue.Replace("\"", "\\" + "\"");

                    return "\"" + stringValue + "\"";
                case TypeCode.Object:
                    return SerializeObjectValue(value, valueType);
            }

            throw new NotImplementedException();
        }

        private string SerializeObjectValue(object value, Type valueType)
        {
            StringBuilder bld = new StringBuilder();

            if ((valueType.IsArray) || (valueType.GetTypeInfo().IsGenericType))
            {
                if (valueType.Name == "Dictionary`2")
                {
                    bld.Append("{");

                    IDictionary<string, object> collection = (IDictionary<string, object>)value;

                    bool first = true;
                    foreach (var keyVal in collection)
                    {
                        if (!first)
                            bld.Append(",");

                        first = false;

                        string serialized = SerializeValue(keyVal.Value);
                        bld.Append("\"" + keyVal.Key + "\":" + serialized);
                    }

                    bld.Append("}");
                }
                else
                {
                    bld.Append(valueType.Name == "HashSet`1" ? "<" : "[");

                    IEnumerable collection = (IEnumerable)value;

                    bool first = true;
                    foreach (object val in collection)
                    {
                        if (!first)
                            bld.Append(",");

                        first = false;
                        bld.Append(SerializeValue(val));
                    }

                    bld.Append(valueType.Name == "HashSet`1" ? ">" : "]");
                }
            }
            // if property is ORID type it needs to be serialized as ORID
            else if (valueType.GetTypeInfo().IsClass && (valueType.Name == "ORID"))
            {
                bld.Append(((ORID)value).RID);
            }
            else if (valueType.GetTypeInfo().IsClass && (valueType.Name == "ODocument"))
            {
                bld.AppendFormat("({0})", SerializeDocument((ODocument)value));
            }
            else if (valueType.GetTypeInfo().IsClass && (valueType.Name == "OEmbeddedRidBag"))
            {
                //bld.AppendFormat("({0})", SerializeDocument((ODocument)value));
                OEmbeddedRidBag ridbag = (OEmbeddedRidBag)value;
                if (ridbag.Count > 0)
                {
                    BinaryBuffer buffer = new BinaryBuffer();
                    bld.Append("%");
                    buffer.Write((byte)1); // config
                    buffer.Write(ridbag.Count); //size
                    foreach (var item in ridbag)
                    {
                        buffer.Write(item);
                    }
                    bld.Append(Convert.ToBase64String(buffer.ToArray()));
                    bld.Append(";");
                }
            }

            return bld.ToString();
        }

        #endregion

        #region Deserialization private methods

        private int ParseFieldName(int i, string recordString, ODocument document)
        {
            int startIndex = i;

            int iColonPos = recordString.IndexOf(':', i);
            if (iColonPos == -1)
                return recordString.Length;

            i = iColonPos;

            // parse field name string from raw document string
            string fieldName = recordString.Substring(startIndex, i - startIndex);
            int pos = fieldName.IndexOf('@');
            if (pos > 0)
            {
                fieldName = fieldName.Substring(pos + 1, fieldName.Length - pos - 1);
            }

            fieldName = fieldName.Replace("\"", "");

            document.Add(fieldName, null);

            // move to position after colon (:)
            i++;

            // check if it's not the end of document which means that current field has null value
            if (i == recordString.Length)
            {
                return i;
            }

            // check what follows after parsed field name and start parsing underlying type
            switch (recordString[i])
            {
                case '"':
                    i = ParseString(i, recordString, document, fieldName);
                    break;
                case '#':
                    i = ParseRecordID(i, recordString, document, fieldName);
                    break;
                case '(':
                    i = ParseEmbeddedDocument(i, recordString, document, fieldName);
                    break;
                case '[':
                    i = ParseList(i, recordString, document, fieldName);
                    break;
                case '<':
                    i = ParseSet(i, recordString, document, fieldName);
                    break;
                case '{':
                    i = ParseEmbeddedDocument(i, recordString, document, fieldName);
                    break;
                case '%':
                    i = ParseRidBags(i, recordString, document, fieldName);
                    break;
                default:
                    i = ParseValue(i, recordString, document, fieldName);
                    break;
            }

            // check if it's not the end of document which means that current field has null value
            if (i == recordString.Length)
            {
                return i;
            }

            // single string value was parsed and we need to push the index if next character is comma
            if (recordString[i] == ',')
            {
                i++;
            }

            return i;
        }

        private int ParseString(int i, string recordString, ODocument document, string fieldName)
        {
            // move to the inside of string
            i++;

            int startIndex = i;

            // search for end of the parsed string value
            while (recordString[i] != '"')
            {
                // strings must escape these characters:
                // " -> \"
                // \ -> \\
                // therefore there needs to be a check for valid end of the string which
                // is quote character that is not preceeded by backslash character \
                if ((recordString[i] == '\\') && (recordString[i + 1] == '\\' || recordString[i + 1] == '"'))
                {
                    i = i + 2;
                }
                else
                {
                    i++;
                }
            }

            string value = recordString.Substring(startIndex, i - startIndex);
            // escape quotes
            value = value.Replace("\\" + "\"", "\"");
            // escape backslashes
            value = value.Replace("\\\\", "\\");

            // assign field value
            if (document[fieldName] == null)
            {
                document[fieldName] = value;
            }
            else if (document[fieldName] is HashSet<object>)
            {
                ((HashSet<object>)document[fieldName]).Add(value);
            }
            else
            {
                ((List<object>)document[fieldName]).Add(value);
            }

            // move past the closing quote character
            i++;

            return i;
        }

        private int ParseRecordID(int i, string recordString, ODocument document, string fieldName)
        {
            int startIndex = i;

            // search for end of parsed record ID value
            while (
                (i < recordString.Length) &&
                (recordString[i] != ',') &&
                (recordString[i] != ')') &&
                (recordString[i] != ']') &&
                (recordString[i] != '}') &&
                (recordString[i] != '>'))
            {
                i++;
            }


            //assign field value
            if (document[fieldName] == null)
            {
                // there is a special case when OEdge InV/OutV fields contains only single ORID instead of HashSet<ORID>
                // therefore single ORID should be deserialized into HashSet<ORID> type
                if (fieldName.Equals("in_") || fieldName.Equals("out_"))
                {
                    document[fieldName] = new HashSet<ORID>();
                    ((HashSet<ORID>)document[fieldName]).Add(new ORID(recordString, startIndex));
                }
                else
                {
                    document[fieldName] = new ORID(recordString, startIndex);
                }
            }
            else if (document[fieldName] is HashSet<object>)
            {
                ((HashSet<object>)document[fieldName]).Add(new ORID(recordString, startIndex));
            }
            else
            {
                ((List<object>)document[fieldName]).Add(new ORID(recordString, startIndex));
            }

            return i;
        }

        private int ParseMap(int i, string recordString, ODocument document, string fieldName)
        {
            int startIndex = i;
            int nestingLevel = 1;

            // search for end of parsed map
            while ((i < recordString.Length) && (nestingLevel != 0))
            {
                // check for beginning of the string to prevent finding an end of map within string value
                if (recordString[i + 1] == '"')
                {
                    // move to the beginning of the string
                    i++;

                    // go to the end of string
                    while ((i < recordString.Length) && (recordString[i + 1] != '"'))
                    {
                        i++;
                    }

                    // move to the end of string
                    i++;
                }
                else if (recordString[i + 1] == '{')
                {
                    // move to the beginning of the string
                    i++;

                    nestingLevel++;
                }
                else if (recordString[i + 1] == '}')
                {
                    // move to the beginning of the string
                    i++;

                    nestingLevel--;
                }
                else
                {
                    i++;
                }
            }

            // move past the closing bracket character
            i++;

            // do not include { and } in field value
            startIndex++;

            //assign field value
            if (document[fieldName] == null)
            {
                document[fieldName] = recordString.Substring(startIndex, i - 1 - startIndex);
            }
            else if (document[fieldName] is HashSet<object>)
            {
                ((HashSet<object>)document[fieldName]).Add(recordString.Substring(startIndex, i - 1 - startIndex));
            }
            else
            {
                ((List<object>)document[fieldName]).Add(recordString.Substring(startIndex, i - 1 - startIndex));
            }

            return i;
        }

        private int ParseValue(int i, string recordString, ODocument document, string fieldName)
        {
            int startIndex = i;

            // search for end of parsed field value
            while (
                (i < recordString.Length) &&
                (recordString[i] != ',') &&
                (recordString[i] != ')') &&
                (recordString[i] != ']') &&
                (recordString[i] != '}') &&
                (recordString[i] != '>'))
            {
                i++;
            }

            // determine the type of field value

            string stringValue = recordString.Substring(startIndex, i - startIndex);
            object value = new object();

            if (stringValue.Length > 0)
            {
                // binary content
                if ((stringValue.Length > 2) && (stringValue[0] == '_') && (stringValue[stringValue.Length - 1] == '_'))
                {
                    stringValue = stringValue.Substring(1, stringValue.Length - 2);

                    // need to be able for base64 encoding which requires content to be devidable by 4
                    int mod4 = stringValue.Length % 4;

                    if (mod4 > 0)
                    {
                        stringValue += new string('=', 4 - mod4);
                    }

                    value = Convert.FromBase64String(stringValue);
                }
                // datetime or date
                else if ((stringValue.Length > 2) && (stringValue[stringValue.Length - 1] == 't') || (stringValue[stringValue.Length - 1] == 'a'))
                {
                    // Unix timestamp is miliseconds past epoch
                    // FIXME: this assumes the server JVM timezone is UTC when it might not be
                    // instead the timezone should be read from the server
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    string foo = stringValue.Substring(0, stringValue.Length - 1);
                    double d = double.Parse(foo);
                    value = epoch.AddMilliseconds(d).ToUniversalTime();
                }
                // boolean
                else if ((stringValue.Length > 2) && (stringValue == "true") || (stringValue == "false"))
                {
                    value = (stringValue == "true") ? true : false;
                }
                // null
                else if ((stringValue.Length > 2) && (stringValue == "null"))
                {
                    value = null;
                }
                // numbers
                else
                {
                    char lastCharacter = stringValue[stringValue.Length - 1];

                    switch (lastCharacter)
                    {
                        case 'b':
                            value = byte.Parse(stringValue.Substring(0, stringValue.Length - 1));
                            break;
                        case 's':
                            value = short.Parse(stringValue.Substring(0, stringValue.Length - 1));
                            break;
                        case 'l':
                            value = long.Parse(stringValue.Substring(0, stringValue.Length - 1));
                            break;
                        case 'f':
                            value = float.Parse(stringValue.Substring(0, stringValue.Length - 1), CultureInfo.InvariantCulture);
                            break;
                        case 'd':
                            value = double.Parse(stringValue.Substring(0, stringValue.Length - 1), CultureInfo.InvariantCulture);
                            break;
                        case 'c':
                            value = decimal.Parse(stringValue.Substring(0, stringValue.Length - 1), CultureInfo.InvariantCulture);
                            break;
                        default:
                            value = int.Parse(stringValue);
                            break;
                    }
                }
            }
            // null
            else if (stringValue.Length == 0)
            {
                value = null;
            }

            //assign field value
            if (document[fieldName] == null)
            {
                document[fieldName] = value;
            }
            else if (document[fieldName] is HashSet<object>)
            {
                ((HashSet<object>)document[fieldName]).Add(value);
            }
            else
            {
                ((List<object>)document[fieldName]).Add(value);
            }

            return i;
        }

        //public  int IntParseFast(string value)
        //{
        //    int result = 0;
        //    for (int i = 0; i < value.Length; i++)
        //    {
        //        char letter = value[i];
        //        result = 10 * result + (letter - 48);
        //    }
        //    return result;
        //}

        /// <summary>
        /// Parse RidBags ex. %[content:binary]; where [content:binary] is the actual binary base64 content.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="recordString"></param>
        /// <param name="document"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private int ParseRidBags(int i, string recordString, ODocument document, string fieldName)
        {
            //move to first base64 char
            i++;

            StringBuilder builder = new StringBuilder();

            while (recordString[i] != ';')
            {
                builder.Append(recordString[i]);
                i++;
            }

            // use a list as it preserves order at this stage which may be important when using ordered edges
            var rids = new List<ORID>();

            var value = Convert.FromBase64String(builder.ToString());
            using (var stream = new MemoryStream(value))
            using (var reader = new BinaryReader(stream))
            {
                var first = reader.ReadByte();
                int offset = 1;
                if ((first & 2) == 2)
                {
                    // uuid parsing is not implemented
                    offset += 16;
                }

                if ((first & 1) == 1) // 1 - embedded,0 - tree-based 
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
                    // Maybe not parse this type of Field and only then Requested retrieve ?
                    // Lazy loading

                    if (_connection == null || !_connection.IsActive)
                        throw new OException(OExceptionType.Connection, "Connection is not opened or is null");

                    // Tree based RidBag - (collectionPointer)(size:int)(changes)

                    // Collection Pointer - (fileId:long)(pageIndex:long)(pageOffset:int)
                    var fileId = reader.ReadInt64EndianAware();
                    var pageIndex = reader.ReadInt64EndianAware();
                    var pageOffset = reader.ReadInt32EndianAware();

                    // size
                    var size = reader.ReadInt32EndianAware();

                    //only process ridbag if size > 0, otherwise the call to SBTreeBonsaiFirstKey operation makes the connection crash (the server probably isn't expecting this use case)
                    if (size > 0)
                    {
                        // Changes - (changesSize:int)[(link:rid)(changeType:byte)(value:int)]*
                        var changesSize = reader.ReadInt32EndianAware();
                        for (int j = 0; j < changesSize; j++)
                        {
                            throw new NotImplementedException("RidBag Changes not yet implemented");
                        }

                        var operation = new SBTreeBonsaiFirstKey(null);
                        operation.FileId = fileId;
                        operation.PageIndex = pageIndex;
                        operation.PageOffset = pageOffset;


                        // Not realy quiete about this
                        var connection = OClient.ReleaseConnection(_connection.Alias);

                        var entries = new Dictionary<ORID, int>();
                        try
                        {
                            var orid = connection.ExecuteOperation(operation);
                            var ft = true;
                            var key = orid.GetField<ORID>("rid");
                            do
                            {
                                var op = new SBTreeBonsaiGetEntriesMajor(null);
                                op.FileId = fileId;
                                op.PageIndex = pageIndex;
                                op.PageOffset = pageOffset;
                                op.FirstKey = key;
                                op.Inclusive = ft;

                                var res = connection.ExecuteOperation(op);
                                entries = res.GetField<Dictionary<ORID, int>>("entries");

                                rids.AddRange(entries.Keys);

                                if (entries.Count == 0)
                                    break;

                                key = entries.Last().Key;
                                ft = false;

                            } while (true);
                        }
                        finally
                        {
                            OClient.ReturnConnection(connection);
                        }
                    }
                }
            }

            document[fieldName] = rids;
            //move past ';'
            i++;

            return i;
        }

        private int ParseEmbeddedDocument(int i, string recordString, ODocument document, string fieldName)
        {
            // move to the inside of embedded document (go past starting bracket character)
            i++;


            if ((i < 15) && (recordString.Length > 15) && (recordString.Substring(i, 15).Equals("ORIDs@pageSize:")))
            {
                OLinkCollection linkCollection = new OLinkCollection();
                i = ParseLinkCollection(i, recordString, linkCollection);
                document[fieldName] = linkCollection;
            }
            else
            {
                // create new dictionary which would hold K/V pairs of embedded document
                ODocument embeddedDocument = new ODocument();

                // assign embedded object
                if (document[fieldName] == null)
                {
                    document[fieldName] = embeddedDocument;
                }
                else if (document[fieldName] is HashSet<object>)
                {
                    ((HashSet<object>)document[fieldName]).Add(embeddedDocument);
                }
                else
                {
                    ((List<object>)document[fieldName]).Add(embeddedDocument);
                }

                // start parsing field names until the closing bracket of embedded document is reached

                while (recordString[i] != ')' && recordString[i] != '}')
                {
                    i = ParseFieldName(i, recordString, embeddedDocument);
                }
            }

            // move past close bracket of embedded document
            i++;

            return i;
        }

        private int ParseLinkCollection(int i, string recordString, OLinkCollection linkCollection)
        {
            // move to the start of pageSize value
            i += 15;

            int index = recordString.IndexOf(',', i);

            linkCollection.PageSize = int.Parse(recordString.Substring(i, index - i));

            // move to root value
            i = index + 6;
            index = recordString.IndexOf(',', i);

            linkCollection.Root = new ORID(recordString.Substring(i, index - i));

            // move to keySize value
            i = index + 9;
            index = recordString.IndexOf(')', i);

            linkCollection.KeySize = int.Parse(recordString.Substring(i, index - i));

            // move past close bracket of link collection
            i++;

            return i;
        }

        private int ParseList(int i, string recordString, ODocument document, string fieldName)
        {
            // move to the first element of this list
            i++;

            if (document[fieldName] == null)
            {
                document[fieldName] = new List<object>();
            }

            while (recordString[i] != ']')
            {
                // check what follows after parsed field name and start parsing underlying type
                switch (recordString[i])
                {
                    case '"':
                        i = ParseString(i, recordString, document, fieldName);
                        break;
                    case '#':
                        i = ParseRecordID(i, recordString, document, fieldName);
                        break;
                    case '(':
                        i = ParseEmbeddedDocument(i, recordString, document, fieldName);
                        break;
                    case '{':
                        i = ParseEmbeddedDocument(i, recordString, document, fieldName);
                        break;
                    case ',':
                        i++;
                        break;
                    default:
                        i = ParseValue(i, recordString, document, fieldName);
                        break;
                }
            }

            // move past closing bracket of this list
            i++;

            return i;
        }

        private int ParseSet(int i, string recordString, ODocument document, string fieldName)
        {
            // move to the first element of this set
            i++;

            if (document[fieldName] == null)
            {
                document[fieldName] = new HashSet<object>();
            }

            while (recordString[i] != '>')
            {
                // check what follows after parsed field name and start parsing underlying type
                switch (recordString[i])
                {
                    case '"':
                        i = ParseString(i, recordString, document, fieldName);
                        break;
                    case '#':
                        i = ParseRecordID(i, recordString, document, fieldName);
                        break;
                    case '(':
                        i = ParseEmbeddedDocument(i, recordString, document, fieldName);
                        break;
                    case '{':
                        i = ParseMap(i, recordString, document, fieldName);
                        break;
                    case ',':
                        i++;
                        break;
                    default:
                        i = ParseValue(i, recordString, document, fieldName);
                        break;
                }
            }

            // move past closing bracket of this set
            i++;

            return i;
        }

        #endregion

    }
}
