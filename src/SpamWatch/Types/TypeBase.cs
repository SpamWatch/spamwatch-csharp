using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SpamWatch.Types
{
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public abstract class TypeBase
    {
        
    }
}