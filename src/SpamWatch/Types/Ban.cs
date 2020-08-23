using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace SpamWatch.Types
{
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class Ban : TypeBase
    {
        [JsonProperty("admin")]
        public long Admin { get; set; }
        
        [JsonProperty("date")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Date { get; set; } = DateTime.Now;
        
        [JsonProperty("id")]
        public long UserId { get; set; }
        
        [JsonProperty("reason")]
        public string Reason { get; set; }
        
        [JsonProperty("message")]
        public string Message { get; set; }

        public string SerializeObject()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}