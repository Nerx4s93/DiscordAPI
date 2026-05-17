using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json.Serialization;

namespace DiscordAPI.Models;

public sealed class DiscordChannel
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("guild_id")]
    public string? GuildId { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("parent_id")]
    public string? ParentId { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("last_message_id")]
    public string? LastMessageId { get; set; }

    [JsonPropertyName("flags")]
    public int Flags { get; set; }

    [JsonPropertyName("nsfw")]
    public bool Nsfw { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public int? RateLimitPerUser { get; set; }

    [JsonPropertyName("bitrate")]
    public int? Bitrate { get; set; }

    [JsonPropertyName("user_limit")]
    public int? UserLimit { get; set; }

    [JsonPropertyName("rtc_region")]
    public string? RtcRegion { get; set; }

    [JsonPropertyName("permission_overwrites")]
    public List<DiscordPermissionOverwrite>? PermissionOverwrites { get; set; }

    [JsonPropertyName("recipients")]
    public List<DiscordUser>? Recipients { get; set; }

    [JsonPropertyName("owner_id")]
    public string? OwnerId { get; set; }

    [JsonPropertyName("last_pin_timestamp")]
    public DateTime? LastPinTimestamp { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonIgnore]
    public string? IconUrl => string.IsNullOrEmpty(Icon) ? null : 
        $"https://cdn.discordapp.com/channel-icons/{Id}/{Icon}.png";
}