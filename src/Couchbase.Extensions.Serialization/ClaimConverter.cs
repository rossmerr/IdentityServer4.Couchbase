using System;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Couchbase.Extensions.Serialization
{
    public class ClaimConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var claim = value as Claim;
            if (claim == null)
            {
                return;
            }
            var jo = new JObject
            {
                {"type", claim.Type},
                {"value", claim.Value},
                {"valueType", claim.ValueType},
                {"issuer", claim.Issuer},
                {"originalIssuer", claim.OriginalIssuer}
            };
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var obj = serializer.Deserialize<JObject>(reader);
            if (obj == null)
            {
                return null;
            }

            return new Claim(obj["type"].Value<string>(), obj["value"].Value<string>(), obj["valueType"].Value<string>(), obj["issuer"].Value<string>(), obj["originalIssuer"].Value<string>());
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Claim);
        }
    }
}