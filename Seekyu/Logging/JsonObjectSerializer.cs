using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seekyu.Logging
{
    public class JsonObjectSerializer : IObjectSerializer
    {
        private readonly JsonSerializerSettings SettingsToAvoidSelfReferenceLoops = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            ContractResolver = new EncryptedStringPropertyResolver("As7Her(ule3nc%ypt1onK3y")
        };

        private string ToGenericTypeString(Type t)
        {
            if (!t.IsGenericType)
                return t.Name;
            string genericTypeName = t.GetGenericTypeDefinition().Name;
            genericTypeName = genericTypeName.Substring(0,
                genericTypeName.IndexOf('`'));
            string genericArgs = string.Join(",",
                t.GetGenericArguments()
                    .Select(ta => ToGenericTypeString(ta)).ToArray());
            return genericTypeName + "<" + genericArgs + ">";
        }

        public string Serialize(object obj)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{ToGenericTypeString(obj.GetType())}");
            try
            {
                sb.AppendLine(JsonConvert.SerializeObject(obj, Formatting.Indented, SettingsToAvoidSelfReferenceLoops));
            }
            catch (Exception e)
            {
                sb.AppendLine($"Failed to SerializeObject");
                sb.AppendLine(e.Message);
            }

            return sb.ToString();
        }
    }
}
