using APIEngine;
using DiscordAPI.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordAPI;

public class DiscordClient : HttpApiClient
{
    private readonly string _token;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public DiscordClient(string token, ProxyInfo? proxy = null)
        : base("https://discord.com/api/v9", proxy)
    {
        _token = token ?? throw new ArgumentNullException(nameof(token));
    }

    #region Информация о боте

    /// <summary>
    /// Возвращает информацию о текущем пользователе (боте),
    /// от имени которого выполняются запросы.
    /// </summary>
    /// <returns>
    /// Объект <see cref="DiscordUser"/> с данными текущего пользователя.
    /// </returns>
    public async Task<DiscordUser> GetMe()
    {
        var response = await GetAsync("users/@me");
        return JsonSerializer.Deserialize<DiscordUser>(response, _jsonOptions)!;
    }

    /// <summary>
    /// Загружает аватар текущего пользователя (бота).
    /// </summary>
    /// <returns>
    /// Объект <see cref="Image"/> с аватаром,
    /// либо <c>null</c>, если аватар отсутствует.
    /// </returns>
    public async Task<Image?> GetAvatar()
    {
        var user = await GetMe();

        if (string.IsNullOrEmpty(user.Avatar))
        {
            return null;
        }

        var response = await GetRawAsync(user.AvatarUrl!);
        await using var stream = await response.Content.ReadAsStreamAsync();
        return Image.FromStream(stream);
    }

    #endregion

    #region Сервера

    /// <summary>
    /// Возвращает список серверов (гильдий),
    /// в которых состоит текущий пользователь.
    /// </summary>
    /// <returns>
    /// Коллекция <see cref="DiscordGuild"/>.
    /// </returns>
    public async Task<IReadOnlyList<DiscordGuild>> GetGuildsAsync()
    {
        var response = await GetAsync("users/@me/guilds");
        return JsonSerializer.Deserialize<IReadOnlyList<DiscordGuild>>(response, _jsonOptions)!;
    }

    /// <summary>
    /// Возвращает подробную информацию о сервере по его идентификатору.
    /// </summary>
    /// <param name="guildId">
    /// Идентификатор сервера.
    /// </param>
    /// <returns>
    /// Объект <see cref="DiscordGuild"/> с информацией о сервере.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Возникает, если <paramref name="guildId"/> пустой или null.
    /// </exception>
    public async Task<DiscordGuild> GetGuildInfoAsync(string guildId)
    {
        if (string.IsNullOrWhiteSpace(guildId))
        {
            throw new ArgumentNullException(nameof(guildId));
        }

        var response = await GetAsync($"guilds/{guildId}");
        return JsonSerializer.Deserialize<DiscordGuild>(response, _jsonOptions)!;
    }

    /// <summary>
    /// Возвращает список каналов указанного сервера.
    /// </summary>
    /// <param name="guildId">
    /// Идентификатор сервера.
    /// </param>
    /// <returns>
    /// Коллекция <see cref="DiscordChannel"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Возникает, если <paramref name="guildId"/> пустой или null.
    /// </exception>
    public async Task<IReadOnlyList<DiscordChannel>> GetGuildChannelsAsync(string guildId)
    {
        if (string.IsNullOrWhiteSpace(guildId))
        {
            throw new ArgumentNullException(nameof(guildId));
        }

        var response = await GetAsync($"guilds/{guildId}/channels");
        return JsonSerializer.Deserialize<IReadOnlyList<DiscordChannel>>(response, _jsonOptions)!;
    }

    /// <summary>
    /// Возвращает сообщения из указанного канала.
    /// </summary>
    /// <param name="channelId">
    /// Идентификатор канала.
    /// </param>
    /// <param name="limit">
    /// Максимальное количество сообщений (от 1 до 100).
    /// </param>
    /// <param name="beforeMessageId">
    /// Идентификатор сообщения, перед которым нужно получить сообщения (опционально).
    /// </param>
    /// <returns>
    /// Коллекция <see cref="DiscordMessage"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Возникает, если <paramref name="channelId"/> пустой или null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Возникает, если <paramref name="limit"/> вне диапазона 1–100.
    /// </exception>
    public async Task<IReadOnlyList<DiscordMessage>> GetChannelMessagesAsync(
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
            .AddParameterIf(!string.IsNullOrEmpty(beforeMessageId), "before", beforeMessageId!)
            .Build();
        var endpoint = $"channels/{channelId}/messages{query}";

        var response = await GetAsync(endpoint);
        return JsonSerializer.Deserialize<IReadOnlyList<DiscordMessage>>(response, _jsonOptions)!;
    }

    /// <summary>
    /// Возвращает информацию о пользователе на указанном сервере.
    /// </summary>
    /// <param name="guildId">
    /// Идентификатор сервера.
    /// </param>
    /// <param name="userId">
    /// Идентификатор пользователя.
    /// </param>
    /// <returns>
    /// Объект <see cref="DiscordMember"/> с информацией о пользователе.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Возникает, если <paramref name="guildId"/> или <paramref name="userId"/> пустые или null.
    /// </exception>
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

        var response = await GetAsync($"guilds/{guildId}/members/{userId}");
        return JsonSerializer.Deserialize<DiscordMember>(response, _jsonOptions)!;
    }

    #endregion

    #region Личные сообщения и группы

    /// <summary>
    /// Возвращает список всех личных сообщений и групповых чатов текущего пользователя.
    /// </summary>
    /// <returns>
    /// Коллекция <see cref="DiscordChannel"/>, представляющая все доступные чаты.
    /// </returns>
    public async Task<IReadOnlyList<DiscordChannel>> GetMyChannelsAsync()
    {
        var response = await GetAsync("users/@me/channels");
        return JsonSerializer.Deserialize<IReadOnlyList<DiscordChannel>>(response, _jsonOptions)!;
    }

    /// <summary>
    /// Возвращает список только личных сообщений (DM) текущего пользователя.
    /// </summary>
    /// <returns>
    /// Коллекция <see cref="DiscordChannel"/> только с типом 1 (личные сообщения).
    /// </returns>
    public async Task<IReadOnlyList<DiscordChannel>> GetPrivateMessagesAsync()
    {
        var allChannels = await GetMyChannelsAsync();
        return [.. allChannels.Where(c => c.Type == 1)]; // Тип 1 = личные сообщения (DM)
    }


    /// <summary>
    /// Возвращает список только групповых чатов текущего пользователя.
    /// </summary>
    /// <returns>
    /// Коллекция <see cref="DiscordChannel"/> только с типом 3 (групповые чаты).
    /// </returns>
    public async Task<IReadOnlyList<DiscordChannel>> GetGroupChatsAsync()
    {
        var allChannels = await GetMyChannelsAsync();
        return [.. allChannels.Where(c => c.Type == 3)]; // Тип 3 = личные сообщения (DM)
    }

    #endregion

    protected override Task ConfigureRequestAsync(HttpRequestMessage request)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue(_token);
        return Task.CompletedTask;
    }
}
