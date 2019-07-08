using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Distancify.LitiumAddOns.Foundation.Json
{
    public static class JSONArrayStreamReader
    {
        public static IEnumerable<JObject> GetJObjects(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                if (jsonReader.Read() && jsonReader.TokenType == JsonToken.StartArray)
                {
                    while (jsonReader.Read())
                    {
                        if (jsonReader.TokenType == JsonToken.StartObject)
                        {
                            yield return JObject.Load(jsonReader);
                        }
                    }
                }
            }
        }
    }
}