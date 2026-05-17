### [[-----EN-----]](../DOCUMENTATION.md) [-----RU-----]

# Документация

DiscordAPI построен на базе APIEngine.

APIEngine берёт на себя:
- отправку HTTP-запросов  
- обработку ответов  
- обработку ошибок  
- поддержку прокси  

DiscordAPI отвечает за:
- работу с Discord REST API v9  
- формирование endpoint'ов  
- десериализацию моделей  
- установку заголовков

---

## Client API

### GetMe
Возвращает информацию о текущем пользователе.
``` C#
public async Task<DiscordUser> GetMe()
```
Возвращает:
- `DiscordUser`

### GetAvatar
Загружает аватар текущего пользователя.
``` C#
public async Task<Image?> GetAvatar()
```
Возвращает:
- Image или null

### GetGuildsAsync
Возвращает список серверов пользователя.
``` C#
public async Task<IReadOnlyList<DiscordGuild>> GetGuildsAsync()
```

### GetGuildInfoAsync
Получает информацию о сервере.
``` C#
public async Task<DiscordGuild> GetGuildInfoAsync(string guildId)
```
Параметры:
- guildId — ID сервера

### GetGuildChannelsAsync
Получает каналы сервера.
``` C#
public async Task<IReadOnlyList<DiscordChannel>> GetGuildChannelsAsync(string guildId)
```

### GetChannelMessagesAsync
Получает сообщения канала.
``` C#
public async Task<IReadOnlyList<DiscordMessage>> GetChannelMessagesAsync(
    string channelId,
    int limit = 100,
    string? beforeMessageId = null)
```
Параметры:
- `channelId` — ID канала
- `limit` — 1–100
- `beforeMessageId` — для пагинации

### GetUser
Получает участника сервера.
``` C#
public async Task<DiscordMember> GetUser(string guildId, string userId)
```

### GetMyChannelsAsync
Возвращает все личные каналы и группы.
``` C#
public async Task<IReadOnlyList<DiscordChannel>> GetMyChannelsAsync()
```

### GetPrivateMessagesAsync
Возвращает только личные сообщения (DM).
``` C#
public async Task<IReadOnlyList<DiscordChannel>> GetPrivateMessagesAsync()
```

### GetGroupChatsAsync
Возвращает только групповые чаты.
``` C#
public async Task<IReadOnlyList<DiscordChannel>> GetGroupChatsAsync()
```

## Models

### DiscordUser
Информация о пользователе.
``` C#
string Id
string Username
string? GlobalName
string Discriminator
string? Avatar
string? Banner
int? AccentColor
string? Bio
string? Email
bool? Verified
```
Дополнительно:
- `FullUsername` — username#discriminator
- `DisplayName` — GlobalName или Username
- `AvatarUrl` — ссылка на аватар
- `BannerUrl` — ссылка на баннер
- `AccentColorHex` — HEX цвет

### DiscordGuild
Информация о сервере.
``` C#
string Id
string Name
string? Icon
string? Banner
DiscordRole[] Roles
```
Дополнительно:
- `IconUrl`
- `BannerUrl`

### DiscordChannel
Информация о канале.
``` C#
string Id
string? GuildId
int Type
string? Name
string? ParentId
int Position
string? LastMessageId
int Flags
bool Nsfw
```
Дополнительно:
- `IconUrl` (для групповых чатов)

### DiscordMessage
Сообщение.
``` C#
string Id
string Content
string ChannelId
DateTime Timestamp
DiscordUser Author
```

### DiscordMember
Участник сервера.
``` C#
string? Nick
string? Avatar
string? Banner
string[] Roles
DiscordUser User
DateTime? PremiumSince
DateTime? JoinedAt
```

### DiscordRole
Роль.
``` C#
string Id
string Name
string? Description
string Permissions
```