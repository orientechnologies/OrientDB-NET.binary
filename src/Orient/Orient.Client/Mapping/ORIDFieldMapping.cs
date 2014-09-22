using System;
using System.Reflection;

namespace Orient.Client.Mapping
{
    internal class ORIDFieldMapping : FieldMapping
    {
        public ORIDFieldMapping(PropertyInfo propertyInfo) : base(propertyInfo, "ORID")
        {
            
        }

        public override void MapToObject(ODocument document, object typedObject)
        {
            _propertyInfo.SetValue(typedObject, document.ORID, null);
        }

        public override void MapToDocument(object typedObject, ODocument document)
        {
            document.ORID = (ORID) _propertyInfo.GetValue(typedObject, null);
        }
    }

    internal class OVersionFieldMapping : FieldMapping
    {
        public OVersionFieldMapping(PropertyInfo propertyInfo)
            : base(propertyInfo, "OVersion")
        {

        }

        public override void MapToObject(ODocument document, object typedObject)
        {
            _propertyInfo.SetValue(typedObject, document.OVersion, null);
        }

        public override void MapToDocument(object typedObject, ODocument document)
        {
            document.OVersion = (int) _propertyInfo.GetValue(typedObject, null);
        }
    }

    internal class OClassIdFieldMapping : FieldMapping
    {
        public OClassIdFieldMapping(PropertyInfo propertyInfo)
            : base(propertyInfo, "OClassId")
        {

        }

        public override void MapToObject(ODocument document, object typedObject)
        {
            _propertyInfo.SetValue(typedObject, document.OClassId, null);
        }

        public override void MapToDocument(object typedObject, ODocument document)
        {
            document.OClassId = (short) _propertyInfo.GetValue(typedObject, null);
        }
    }


    internal class OClassNameFieldMapping : FieldMapping
    {
        public OClassNameFieldMapping(PropertyInfo propertyInfo)
            : base(propertyInfo, "OClassName")
        {

        }

        public override void MapToObject(ODocument document, object typedObject)
        {
            _propertyInfo.SetValue(typedObject, document.OClassName, null);
        }

        public override void MapToDocument(object typedObject, ODocument document)
        {
            document.OClassName = (string) _propertyInfo.GetValue(typedObject, null);
        }
    }

}