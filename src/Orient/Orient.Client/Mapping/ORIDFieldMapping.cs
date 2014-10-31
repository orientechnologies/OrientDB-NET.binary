using System;
using System.Reflection;

namespace Orient.Client.Mapping
{
    internal class ORIDFieldMapping<TTarget> : FieldMapping<TTarget>
    {
        public ORIDFieldMapping(PropertyInfo propertyInfo) : base(propertyInfo, "ORID")
        {
            
        }

        public override void MapToObject(ODocument document, TTarget typedObject)
        {
            SetPropertyValue(typedObject, document.ORID);
        }

        public override void MapToDocument(TTarget typedObject, ODocument document)
        {
            document.ORID = (ORID)GetPropertyValue(typedObject);
        }
    }

    internal class OVersionFieldMapping<TTarget> : FieldMapping<TTarget>
    {
        public OVersionFieldMapping(PropertyInfo propertyInfo)
            : base(propertyInfo, "OVersion")
        {

        }

        public override void MapToObject(ODocument document, TTarget typedObject)
        {
            SetPropertyValue(typedObject, document.OVersion);
        }

        public override void MapToDocument(TTarget typedObject, ODocument document)
        {
            document.OVersion = (int)GetPropertyValue(typedObject);
        }
    }
    
    internal class OTypeFieldMapping<TTarget> : FieldMapping<TTarget>
    {
        public OTypeFieldMapping(PropertyInfo propertyInfo)
            : base(propertyInfo, "OType")
        {
            
        }
        
        public override void MapToObject(ODocument document, TTarget typedObject)
        {
            SetPropertyValue(typedObject, document.OType);
        }
        
        public override void MapToDocument(TTarget typedObject, ODocument document)
        {
            document.OType = (ORecordType)GetPropertyValue(typedObject);
        }
    }

    internal class OClassIdFieldMapping<TTarget> : FieldMapping<TTarget>
    {
        public OClassIdFieldMapping(PropertyInfo propertyInfo)
            : base(propertyInfo, "OClassId")
        {

        }

        public override void MapToObject(ODocument document, TTarget typedObject)
        {
            SetPropertyValue(typedObject, document.OClassId);
        }

        public override void MapToDocument(TTarget typedObject, ODocument document)
        {
            document.OClassId = (short)GetPropertyValue(typedObject);
        }
    }


    internal class OClassNameFieldMapping<TTarget> : FieldMapping<TTarget>
    {
        public OClassNameFieldMapping(PropertyInfo propertyInfo)
            : base(propertyInfo, "OClassName")
        {

        }

        public override void MapToObject(ODocument document, TTarget typedObject)
        {
            SetPropertyValue(typedObject, document.OClassName);
        }

        public override void MapToDocument(TTarget typedObject, ODocument document)
        {
            document.OClassName = (string) GetPropertyValue(typedObject);
        }
    }

}
