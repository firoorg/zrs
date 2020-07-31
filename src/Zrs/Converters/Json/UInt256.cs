namespace Zrs.Converters.Json
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using NBitcoin;

    public sealed class UInt256 : JsonConverter<uint256>
    {
        public override uint256 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return uint256.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, uint256 value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
