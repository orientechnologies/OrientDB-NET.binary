using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orient.Nunit.Test
{
    public static class TestConstants
    {

        public const string newDocumentJson = @"{
            ""simpleString"": ""TestString"",
            ""embeddedObject"": {                
                ""embeddedArrayOfObjects"": [{
                    ""stringProperty"": ""ValueA1""
                }, {
                    ""numericProperty"": 2
                }, {
                    ""booleanProperty"": true
                }]                
            }
        }";

    }
}
