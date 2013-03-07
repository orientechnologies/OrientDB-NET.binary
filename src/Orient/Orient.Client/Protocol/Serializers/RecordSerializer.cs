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
        /*internal static byte[] ToArray(object objectToSerialize, Type objectType) 
        {
            string serializedString = objectType.Name + "@";

            serializedString += SerializeObject(objectToSerialize, objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance));

            return Encoding.UTF8.GetBytes(serializedString);
        }

        internal static ORecord ToRecord(DtoRecord record)
        {
            ORecord deserializedRecord = new ORecord(record);

            if (record.Type != ORecordType.Document)
            {
                return deserializedRecord;
            }

            string recordString = BinarySerializer.ToString(record.Content).Trim();

            int atIndex = recordString.IndexOf('@');
            int colonIndex = recordString.IndexOf(':');
            int index = 0;

            // parse class name
            if ((atIndex != -1) && (atIndex < colonIndex))
            {
                deserializedRecord.Class = recordString.Substring(0, atIndex);
                index = atIndex + 1;
            }
            else
            {
                deserializedRecord.Class = "";
            }

            // start document parsing with first field name
            do
            {
                index = ParseFieldName(index, recordString, deserializedRecord.Fields);
            }
            while (index < recordString.Length);

            return deserializedRecord;
        }*/

        internal static ORecord ToRecord(ORID orid, int version, ORecordType type, short classId, byte[] rawRecord)
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
                index = ParseFieldName(index, recordString, record.DataObject);
            }
            while (index < recordString.Length);

            return record;
        }

        #region Serialization private methods

        /*private static string SerializeObject(object objectToSerialize, PropertyInfo[] properties)
        {
            string serializedString = "";

            if ((properties != null) && (properties.Length > 0))
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo property = properties[i];

                    serializedString += property.Name + ":";
                    serializedString += SerializeValue(property.GetValue(objectToSerialize, null));

                    if (i < (properties.Length - 1))
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
                        else if (valueType.IsClass)
                        {
                            serializedString += "(";
                            serializedString += SerializeObject(value, valueType.GetProperties());
                            serializedString += ")";
                        }
                        break;
                    default:
                        break;
                }
            }

            return serializedString;
        }*/

        #endregion

        #region Deserialization private methods

        private static int ParseFieldName(int i, string recordString, DataObject dataObject)
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

            // parse field name string from raw document
            string fieldName = recordString.Substring(startIndex, i - startIndex);

            dataObject.Add(fieldName, null);

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
                    i = ParseString(i, recordString, dataObject, fieldName);
                    break;
                case '#':
                    i = ParseRecordID(i, recordString, dataObject, fieldName);
                    break;
                case '(':
                    i = ParseEmbeddedDocument(i, recordString, dataObject, fieldName);
                    break;
                case '[':
                    if (recordString[i + 1] == '(')
                    {
                        i = ParseCollection(i, recordString, dataObject, fieldName);
                    }
                    else
                    {
                        i = ParseCollection(i, recordString, dataObject, fieldName);
                    }
                    break;
                case '{':
                    i = ParseMap(i, recordString, dataObject, fieldName);
                    break;
                default:
                    i = ParseValue(i, recordString, dataObject, fieldName);
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

        private static int ParseString(int i, string recordString, DataObject dataObject, string fieldName)
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
            if (dataObject[fieldName] == null)
            {
                dataObject[fieldName] = value;
            }
            else
            {
                ((List<object>)dataObject[fieldName]).Add(value);
            }

            // move past the closing quote character
            i++;

            return i;
        }

        private static int ParseRecordID(int i, string recordString, DataObject dataObject, string fieldName)
        {
            int startIndex = i;

            // search for end of parsed record ID value
            while ((i < recordString.Length) && (recordString[i] != ',') && (recordString[i] != ')') && (recordString[i] != ']'))
            {
                i++;
            }

            string orid = recordString.Substring(startIndex, i - startIndex);

            //assign field value
            if (dataObject[fieldName] == null)
            {
                dataObject[fieldName] = new ORID(orid);
            }
            else
            {
                ((List<object>)dataObject[fieldName]).Add(new ORID(orid));
            }

            return i;
        }

        private static int ParseMap(int i, string recordString, DataObject dataObject, string fieldName)
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
            if (dataObject[fieldName] == null)
            {
                dataObject[fieldName] = recordString.Substring(startIndex, i - startIndex);
            }
            else
            {
                ((List<object>)dataObject[fieldName]).Add(recordString.Substring(startIndex, i - startIndex));
            }

            return i;
        }

        private static int ParseValue(int i, string recordString, DataObject dataObject, string fieldName)
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
            if (dataObject[fieldName] == null)
            {
                dataObject[fieldName] = value;
            }
            else
            {
                ((List<object>)dataObject[fieldName]).Add(value);
            }

            return i;
        }

        private static int ParseEmbeddedDocument(int i, string recordString, DataObject dataObject, string fieldName)
        {
            // move to the inside of embedded document (go past starting bracket character)
            i++;

            // create new dictionary which would hold K/V pairs of embedded document
            DataObject embeddedDataObject = new DataObject();

            if (dataObject[fieldName] == null)
            {
                dataObject[fieldName] = embeddedDataObject;
            }
            else
            {
                ((List<object>)dataObject[fieldName]).Add(embeddedDataObject);
            }

            // start parsing field names until the closing bracket of embedded document is reached
            while (recordString[i] != ')')
            {
                i = ParseFieldName(i, recordString, embeddedDataObject);
            }

            // move past close bracket of embedded document
            i++;

            return i;
        }

        private static int ParseCollection(int i, string recordString, DataObject dataObject, string fieldName)
        {
            // move to the first element of this collection
            i++;

            dataObject[fieldName] = new List<object>();

            while (recordString[i] != ']')
            {
                // check what follows after parsed field name and start parsing underlying type
                switch (recordString[i])
                {
                    case '"':
                        i = ParseString(i, recordString, dataObject, fieldName);
                        break;
                    case '#':
                        i = ParseRecordID(i, recordString, dataObject, fieldName);
                        break;
                    case '(':
                        i = ParseEmbeddedDocument(i, recordString, dataObject, fieldName);
                        break;
                    case '{':
                        i = ParseMap(i, recordString, dataObject, fieldName);
                        break;
                    case ',':
                        i++;
                        break;
                    default:
                        i = ParseValue(i, recordString, dataObject, fieldName);
                        break;
                }
            }

            // move past close bracket of collection
            i++;

            return i;
        }

        #endregion
    }
}
