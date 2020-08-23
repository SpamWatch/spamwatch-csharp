using System;
using Newtonsoft.Json;
using SpamWatch.Enums;

namespace SpamWatch.Types.Utils
{
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