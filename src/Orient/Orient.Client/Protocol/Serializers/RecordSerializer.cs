using System;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Orient.Client.Protocol.Serializers
{
    internal static class RecordSerializer
    {
        internal static string Serialize(ODocument document)
        {
            if (!document.HasField("@ClassName"))
            {
                throw new OException(OExceptionType.Serialization, "Document doesn't contain @ClassName field which is required for serialization.");
            }

            return document.GetField<string>("@ClassName") + "@" + SerializeDocument(document);
        }

        #region Deserialize

        internal static ODocument Deserialize(ORID orid, int version, ORecordType type, short classId, byte[] rawRecord)
        {
            ODocument document = new ODocument();
            document.ORID = orid;
            document.OVersion = version;
            document.OType = type;
            document.OClassId = classId;

            string recordString = BinarySerializer.ToString(rawRecord).Trim();

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
            }
            while (index < recordString.Length);

            return document;
        }

        internal static ODocument Deserialize(string recordString)
        {
            ODocument document = new ODocument();

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
            }
            while (index < recordString.Length);

            return document;
        }

        #endregion

        #region Serialization private methods

        private static string SerializeDocument(ODocument document)
        {
            string serializedString = "";

            if (document.Keys.Count > 0)
            {
                int iteration = 0;

                foreach (KeyValuePair<string, object> field in document)
                {
                    // serialize only fields which doesn't start with @ character
                    if ((field.Key.Length > 0) && (field.Key[0] != '@'))
                    {
                        serializedString += field.Key + ":";
                        serializedString += SerializeValue(field.Value);
                    }

                    iteration++;

                    if (iteration < document.Keys.Count)
                    {
                        if ((field.Key.Length > 0) && (field.Key[0] != '@'))
                        {
                            serializedString += ",";
                        }
                    }
                }
            }

            return serializedString;
        }

        private static string SerializeValue(object value)
        {
            string serializedString = "";

            if (value != null)
            {
                Type valueType = value.GetType();

                switch (Type.GetTypeCode(valueType))
                {
                    case TypeCode.Empty:
                        // null case is empty
                        break;
                    case TypeCode.Boolean:
                        serializedString += value.ToString().ToLower();
                        break;
                    case TypeCode.Byte:
                        serializedString += value.ToString() + "b";
                        break;
                    case TypeCode.Int16:
                        serializedString += value.ToString() + "s";
                        break;
                    case TypeCode.Int32:
                        serializedString += value.ToString();
                        break;
                    case TypeCode.Int64:
                        serializedString += value.ToString() + "l";
                        break;
                    case TypeCode.Single:
                        serializedString += ((float)value).ToString(CultureInfo.InvariantCulture) + "f";
                        break;
                    case TypeCode.Double:
                        serializedString += ((double)value).ToString(CultureInfo.InvariantCulture) + "d";
                        break;
                    case TypeCode.Decimal:
                        serializedString += ((decimal)value).ToString(CultureInfo.InvariantCulture) + "c";
                        break;
                    case TypeCode.DateTime:
                        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        serializedString += ((long)((DateTime)value - unixEpoch).TotalMilliseconds).ToString() + "t";
                        break;
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

                        serializedString += "\"" + stringValue + "\"";
                        break;
                    case TypeCode.Object:
                        if ((valueType.IsArray) || (valueType.IsGenericType))
                        {
                            if (valueType.Name == "HashSet`1")
                            {
                                serializedString += "<";
                            }
                            else
                            {
                                serializedString += "[";
                            }

                            IEnumerable collection = (IEnumerable)value;

                            foreach (object val in collection)
                            {
                                if (valueType.IsArray)
                                {
                                    serializedString += SerializeValue(val);
                                }
                                else
                                {
                                    serializedString += SerializeValue(val);
                                }

                                serializedString += ",";
                            }

                            // remove last comma from currently parsed collection
                            if (serializedString[serializedString.Length - 1] == ',')
                            {
                                serializedString = serializedString.Remove(serializedString.Length - 1);
                            }

                            if (valueType.Name == "HashSet`1")
                            {
                                serializedString += ">";
                            }
                            else
                            {
                                serializedString += "]";
                            }
                        }
                        // if property is ORID type it needs to be serialized as ORID
                        else if (valueType.IsClass && (valueType.Name == "ORID"))
                        {
                            serializedString += ((ORID)value).RID;
                        }
                        else if (valueType.IsClass && (valueType.Name == "ODocument"))
                        {
                            serializedString += "(";
                            serializedString += SerializeDocument((ODocument)value);
                            serializedString += ")";
                        }
                        break;
                    default:
                        break;
                }
            }

            return serializedString;
        }

        #endregion

        #region Deserialization private methods

        private static int ParseFieldName(int i, string recordString, ODocument document)
        {
            int startIndex = i;

            // iterate until colon is found since it's the character which ends the field name
            while (recordString[i] != ':')
            {
                i++;

                if (i >= recordString.Length)
                {
                    return recordString.Length;
                }
            }

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
                    i = ParseMap(i, recordString, document, fieldName);
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

        private static int ParseString(int i, string recordString, ODocument document, string fieldName)
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
                if ((recordString[i] == '\\') && (recordString[i + 1] == '"'))
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

        private static int ParseRecordID(int i, string recordString, ODocument document, string fieldName)
        {
            int startIndex = i;

            // search for end of parsed record ID value
            while (
                (i < recordString.Length) && 
                (recordString[i] != ',') && 
                (recordString[i] != ')') && 
                (recordString[i] != ']') &&
                (recordString[i] != '>'))
            {
                i++;
            }

            string orid = recordString.Substring(startIndex, i - startIndex);

            //assign field value
            if (document[fieldName] == null)
            {
                // there is a special case when OEdge InV/OutV fields contains only single ORID instead of HashSet<ORID>
                // therefore single ORID should be deserialized into HashSet<ORID> type
                if (fieldName.Equals("in_") || fieldName.Equals("out_"))
                {
                    document[fieldName] = new HashSet<ORID>();
                    ((HashSet<ORID>)document[fieldName]).Add(new ORID(orid));
                }
                else
                {
                    document[fieldName] = new ORID(orid);
                }
            }
            else if (document[fieldName] is HashSet<object>)
            {
                ((HashSet<object>)document[fieldName]).Add(new ORID(orid));
            }
            else
            {
                ((List<object>)document[fieldName]).Add(new ORID(orid));
            }

            return i;
        }

        private static int ParseMap(int i, string recordString, ODocument document, string fieldName)
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

        private static int ParseValue(int i, string recordString, ODocument document, string fieldName)
        {
            int startIndex = i;

            // search for end of parsed field value
            while (
                (i < recordString.Length) && 
                (recordString[i] != ',') && 
                (recordString[i] != ')') && 
                (recordString[i] != ']') &&
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

        /// <summary>
        /// Parse RidBags ex. %[content:binary]; where [content:binary] is the actual binary base64 content.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="recordString"></param>
        /// <param name="document"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static int ParseRidBags(int i, string recordString, ODocument document, string fieldName)
        {
            //move to first base64 char
            i++;

            StringBuilder builder = new StringBuilder();

            while (recordString[i] != ';')
            {
                builder.Append(recordString[i]);
                i++;
            }

            var value = Convert.FromBase64String(builder.ToString());

            document[fieldName] = value;

            //move past ';'
            i++;

            return i;
        }

        private static int ParseEmbeddedDocument(int i, string recordString, ODocument document, string fieldName)
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
                while (recordString[i] != ')')
                {
                    i = ParseFieldName(i, recordString, embeddedDocument);
                }
            }

            // move past close bracket of embedded document
            i++;

            return i;
        }

        private static int ParseLinkCollection(int i, string recordString, OLinkCollection linkCollection)
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

        private static int ParseList(int i, string recordString, ODocument document, string fieldName)
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

            // move past closing bracket of this list
            i++;

            return i;
        }

        private static int ParseSet(int i, string recordString, ODocument document, string fieldName)
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
