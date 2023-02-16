using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;
using Hybrid.Mock.Core.Extensions;
using Hybrid.Mock.Core.Models;

namespace Hybrid.Mock.Core.Utilities
{
    [ExcludeFromCodeCoverage]
    public static class MaskHelper
    {
        public static JObject MaskJObjectProperty(JObject jObject, string propertyName)
        {
            List<JObjectProperty> list = jObject.Descendants()
                .Where(t => t.Type == JTokenType.Property && propertyName.Equals(((JProperty)t).Name, StringComparison.OrdinalIgnoreCase))
                .Select(p => new JObjectProperty
                {
                    PropPath = ((JProperty)p).Path,
                    PropValue = ((JProperty)p).Value.ToString()
                })
                .ToList();

            foreach (JObjectProperty prop in list)
            {
                var originalValue = prop.PropValue;
                var encryptedValue = EncryptString(prop.PropValue);
                jObject = MaskJObjectProperty(jObject, propertyName, prop.PropValue, encryptedValue);
            }

            return jObject;
        }

        public static JObject MaskJObjectProperty(JObject jObject, string propertyName, string unencryptedValue, string encryptedValue)
        {
            var list = jObject.Descendants()
                .Where(t => t.Type == JTokenType.Property && (propertyName.Equals(((JProperty)t).Name, StringComparison.OrdinalIgnoreCase)) && ((JProperty)t).Value.ToString() == unencryptedValue)
                .Select(p => ((JProperty)p).Path)
                .ToList();

            foreach (var path in list)
            {
                jObject = JsonExtensions.ReplacePath(jObject, path, encryptedValue);
            }

            return jObject;
        }

        public static string EncryptString(string unencryptedString)
        {
            return Constants.TransactionFieldMask;
        }
    }
}
