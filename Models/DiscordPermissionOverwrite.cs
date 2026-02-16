using System.Text.Json.Serialization;

namespace DiscordAPI.Models;

public sealed class DiscordPermissionOverwrite
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("allow")]
    public string Allow { get; set; } = null!;

    [JsonPropertyName("deny")]
    public string Deny { get; set; } = null!;
}