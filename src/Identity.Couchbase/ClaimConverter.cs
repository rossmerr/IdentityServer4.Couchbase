using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Identity.Couchbase
{
    public class ClaimConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var baseObj = serializer.Deserialize<JArray>(reader);
            if (baseObj == null)
            {
                return Enumerable.Empty<Claim>();
            }

            var claims = new List<Claim>();
            foreach (JObject obj in baseObj)
            {
                var claim = new Claim(obj["type"].Value<string>(), obj["value"].Value<string>(), obj["valueType"].Value<string>(), obj["issuer"].Value<string>(), obj["originalIssuer"].Value<string>());
                claims.Add(claim);
            }

            return claims;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IEnumerable<Claim>);
        }
    }
}