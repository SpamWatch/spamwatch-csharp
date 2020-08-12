using System;
using Newtonsoft.Json;
using SpamWatch.Enums;

namespace SpamWatch.Models
{
    public class SpamWatchToken
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        [JsonProperty("permission", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(PermissionConverter))]
        public Permissions Permission { get; set; }

        [JsonProperty("retired", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Retired { get; set; }

        [JsonProperty("token", NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; }

        [JsonProperty("userid", NullValueHandling = NullValueHandling.Ignore)]
        public long? Userid { get; set; }
    }

    public class PermissionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue,
            JsonSerializer serializer)
        {
            var value = (string) reader.Value;
            if (value == "Root")
                return Permissions.Root;
            else if (value == "Admin")
                return Permissions.Admin;
            else
                return Permissions.User;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}