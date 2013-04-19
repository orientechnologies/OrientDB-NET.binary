using System;
using System.Reflection;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Orient.Client.Protocol.Serializers
{
    internal static class RecordSerializer
    {
        internal static string ToString(string className, ODocument document)
        {
            return className + "@" + SerializeDocument(document);
        }

        /*internal static ORecord ToRecord(ORID orid, int version, ORecordType type, short classId, byte[] rawRecord)
        {
            ORecord record = new ORecord(orid, version, type, classId);

            string recordString = BinarySerializer.ToString(rawRecord).Trim();

            int atIndex = recordString.IndexOf('@');
            int colonIndex = recordString.IndexOf(':');
            int index = 0;

            // parse class name
            if ((atIndex != -1) && (atIndex < colonIndex))
            {
                record.ClassName = recordString.Substring(0, atIndex);
                index = atIndex + 1;
            }

            // start document parsing with first field name
            do
            {
                index = ParseFieldName(index, recordString, record.Document);
            }
            while (index < recordString.Length);

            return record;
        }*/

        /*internal static ORecord ToRecord(string recordString)
        {
            ORecord record = new ORecord();

            int atIndex = recordString.IndexOf('@');
            int colonIndex = recordString.IndexOf(':');
            int index = 0;

            // parse class name
            if ((atIndex != -1) && (atIndex < colonIndex))
            {
                record.ClassName = recordString.Substring(0, atIndex);
                index = atIndex + 1;
            }

            // start document parsing with first field name
            do
            {
                index = ParseFieldName(index, recordString, record.Document);
            }
            while (index < recordString.Length);

            return record;
        }*/

        #region Deserialize

        internal static ODocument Deserialize(ORID orid, int version, ORecordType type, short classId, byte[] rawRecord)
        {
            ODocument document = new ODocument()
                .SetField("@ORID", orid)
                .SetField("@Version", version)
                .SetField("@Type", type)
                .SetField("@ClassId", classId);

            string recordString = BinarySerializer.ToString(rawRecord).Trim();

            int atIndex = recordString.IndexOf('@');
            int colonIndex = recordString.IndexOf(':');
            int index = 0;

            // parse class name
            if ((atIndex != -1) && (atIndex < colonIndex))
            {
                document.SetField("@ClassName", recordString.Substring(0, atIndex));
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
                document.SetField("@ClassName", recordString.Substring(0, atIndex));
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
                    serializedString += field.Key + ":";
                    serializedString += SerializeValue(field.Value);
                    iteration++;

                    if (iteration < document.Keys.Count)
                    {
                        serializedString += ",";
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
                            serializedString += "[";

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

                            serializedString += "]";
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
                    if (recordString[i + 1] == '(')
                    {
                        i = ParseCollection(i, recordString, document, fieldName);
                    }
                    else
                    {
                        i = ParseCollection(i, recordString, document, fieldName);
                    }
                    break;
                case '{':
                    i = ParseMap(i, recordString, document, fieldName);
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
            while ((i < recordString.Length) && (recordString[i] != ',') && (recordString[i] != ')') && (recordString[i] != ']'))
            {
                i++;
            }

            string orid = recordString.Substring(startIndex, i - startIndex);

            //assign field value
            if (document[fieldName] == null)
            {
                document[fieldName] = new ORID(orid);
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
                    while ((i < recordString.Length) && (recordString[i] != '"'))
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

            //assign field value
            if (document[fieldName] == null)
            {
                document[fieldName] = recordString.Substring(startIndex, i - startIndex);
            }
            else
            {
                ((List<object>)document[fieldName]).Add(recordString.Substring(startIndex, i - startIndex));
            }

            return i;
        }

        private static int ParseValue(int i, string recordString, ODocument document, string fieldName)
        {
            int startIndex = i;

            // search for end of parsed field value
            while ((i < recordString.Length) && (recordString[i] != ',') && (recordString[i] != ')') && (recordString[i] != ']'))
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
            else
            {
                ((List<object>)document[fieldName]).Add(value);
            }

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

                if (document[fieldName] == null) // embedded object
                {
                    document[fieldName] = embeddedDocument;
                }
                else // collection of embedded objects
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

        private static int ParseCollection(int i, string recordString, ODocument document, string fieldName)
        {
            // move to the first element of this collection
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

            // move past closing bracket of collection
            i++;

            return i;
        }

        #endregion
    }
}
