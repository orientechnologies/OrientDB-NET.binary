using System;
using System.Reflection;

namespace Orient.Client.Mapping
{
    internal class ORIDFieldMapping : FieldMapping
    {
        public ORIDFieldMapping(PropertyInfo propertyInfo) : base(propertyInfo, "ORID")
        {
            
        }

        public override void MapToObject(ODocument document, object typedObject, string basePath)
        {
            _propertyInfo.SetValue(typedObject, document.ORID, null);
        }
    }

    internal class OVersionFieldMapping : FieldMapping
    {
        public OVersionFieldMapping(PropertyInfo propertyInfo)
            : base(propertyInfo, "OVersion")
        {

        }

        public override void MapToObject(ODocument document, object typedObject, string basePath)
        {
            _propertyInfo.SetValue(typedObject, document.OVersion, null);
        }
    }

    internal class OClassIdFieldMapping : FieldMapping
    {
        public OClassIdFieldMapping(PropertyInfo propertyInfo)
            : base(propertyInfo, "OClassId")
        {

        }

        public override void MapToObject(ODocument document, object typedObject, string basePath)
        {
            _propertyInfo.SetValue(typedObject, document.OClassId, null);
        }
    }


    internal class OClassNameFieldMapping : FieldMapping
    {
        public OClassNameFieldMapping(PropertyInfo propertyInfo)
            : base(propertyInfo, "OClassName")
        {

        }

        public override void MapToObject(ODocument document, object typedObject, string basePath)
        {
            _propertyInfo.SetValue(typedObject, document.OClassName, null);
        }
    }

}