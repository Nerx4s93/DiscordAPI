using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using DiscordApi.Models;

namespace DiscordApi;

public class DiscordClient(string token, ProxyInfo? proxy) : IDisposable
{
    private readonly string _token = token ?? throw new ArgumentNullException(nameof(token));
    private readonly HttpClient _httpClient = CreateHttpClient(proxy, token);
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    private const string BaseUrl = "https://discord.com/api/v9";

    public DiscordClient(string token) : this(token, proxy: null) { }

    #region Информация о боте

    public async Task<DiscordUser> GetMe()
    {
        return await MakeRequestAsync<DiscordUser>("users/@me");
    }

    public async Task<Image?> GetAvatar()
    {
        var user = await MakeRequestAsync<DiscordUser>("users/@me");

        if (string.IsNullOrEmpty(user.Avatar))
        {
            return null;
        }

        var response = await MakeRequestAsync(user.AvatarUrl!);
        await using var stream = await response.Content.ReadAsStreamAsync();
        return Image.FromStream(stream);
    }

    #endregion

    #region Сервера

    public async Task<IReadOnlyList<DiscordGuild>> GetGuildsAsync()
    {
        return await MakeRequestAsync<IReadOnlyList<DiscordGuild>>("users/@me/guilds");
    }

    public async Task<DiscordGuild> GetGuildInfoAsync(string guildId)
    {
        if (string.IsNullOrWhiteSpace(guildId))
        {
            throw new ArgumentNullException(nameof(guildId));
        }

        return await MakeRequestAsync<DiscordGuild>($"guilds/{guildId}");
    }

    public async Task<IReadOnlyList<DiscordChannel>> GetGuildChannelsAsync(string guildId)
    {
        if (string.IsNullOrWhiteSpace(guildId))
        {
            throw new ArgumentNullException(nameof(guildId));
        }

        return await MakeRequestAsync<IReadOnlyList<DiscordChannel>>($"guilds/{guildId}/channels");
    }

    public async Task<IReadOnlyList<DiscrodMessage>> GetChannelMessagesAsync(
        string channelId, 
        int limit = 100, 
        string? beforeMessageId = null)
    {
        if (string.IsNullOrWhiteSpace(channelId))
        {
            throw new ArgumentNullException(nameof(channelId));
        }

        if (limit is <= 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be between 1 and 100");
        }

        var query = QueryParametersBuilder.Create()
            .AddParameter("limit", limit)
            .AddParameterIf(!string.IsNullOrEmpty(beforeMessageId), "before", beforeMessageId)
            .Build();
        var endpoint = $"channels/{channelId}/messages{query}";

        var messages = await MakeRequestAsync<IReadOnlyList<DiscrodMessage>>(endpoint);
        return messages;
    }

    public async Task<DiscordMember> GetUser(string guildId, string userId)
    {
        if (string.IsNullOrWhiteSpace(guildId))
        {
            throw new ArgumentNullException(nameof(guildId));
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentNullException(nameof(userId));
        }

        var endpoint = $"guilds/{guildId}/members/{userId}";
        var member = await MakeRequestAsync<DiscordMember>(endpoint);
        return member;
    }


    #endregion

    #region Формирвание запроса

    private async Task<T> MakeRequestAsync<T>(string endpoint)
    {
        var response = await MakeRequestAsync($"{BaseUrl}/{endpoint}");

        await using var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions);

        return result ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    private async Task<HttpResponseMessage> MakeRequestAsync(string url)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return response;
    }


    #endregion

    private static HttpClient CreateHttpClient(ProxyInfo? proxy, string token)
    {
        var handler = new HttpClientHandler();

        if (proxy != null)
        {
            var webProxy = new WebProxy(proxy.Host, proxy.Port);

            if (proxy.HasCredentials)
            {
                webProxy.Credentials = new NetworkCredential(proxy.Username, proxy.Password);
            }

            handler.Proxy = webProxy;
            handler.UseProxy = true;
        }

        var httpClient = new HttpClient(handler, disposeHandler: true)
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(token);
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "DiscordApi/1.0 (api-client)");

        return httpClient;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        GC.SuppressFinalize(this);
    }
}
