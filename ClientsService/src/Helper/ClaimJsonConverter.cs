using System;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class ClaimJsonConverter : JsonConverter<Claim>
{
    public override Claim Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Parse the JSON object into a JsonDocument for robust property access
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        string? type = root.TryGetProperty("Type", out var pType) ? pType.GetString() : null;
        string? value = root.TryGetProperty("Value", out var pValue) ? pValue.GetString() : null;
        string? valueType = root.TryGetProperty("ValueType", out var pValueType) ? pValueType.GetString() : ClaimValueTypes.String;
        string? issuer = root.TryGetProperty("Issuer", out var pIssuer) ? pIssuer.GetString() : ClaimsIdentity.DefaultIssuer;
        string? originalIssuer = root.TryGetProperty("OriginalIssuer", out var pOrig) ? pOrig.GetString() : ClaimsIdentity.DefaultIssuer;

        // Use the 5-arg constructor to preserve as much info as possible
        return new Claim(type ?? string.Empty, value ?? string.Empty, valueType, issuer, originalIssuer);
    }

    public override void Write(Utf8JsonWriter writer, Claim value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Type", value.Type);
        writer.WriteString("Value", value.Value);
        // Only write optional fields if they exist (keeps JSON smaller)
        if (!string.IsNullOrEmpty(value.ValueType))
            writer.WriteString("ValueType", value.ValueType);
        if (!string.IsNullOrEmpty(value.Issuer))
            writer.WriteString("Issuer", value.Issuer);
        if (!string.IsNullOrEmpty(value.OriginalIssuer))
            writer.WriteString("OriginalIssuer", value.OriginalIssuer);
        writer.WriteEndObject();
    }
}
