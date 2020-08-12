using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SpamWatch.Models
{
    public class SpamWatchBan
    {
        [JsonProperty("admin", NullValueHandling = NullValueHandling.Ignore)]
        public long? Admin { get; set; }

        [JsonProperty("date", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Date { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        [JsonProperty("reason", NullValueHandling = NullValueHandling.Ignore)]
        public string Reason { get; set; }
    }
}