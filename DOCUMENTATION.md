### [-----EN-----] [[-----RU-----]](docs/DOCUMENTATION_RU.md)

# Documentation

DiscordAPI is built on top of APIEngine.

APIEngine takes care of:
- sending HTTP requests 
- handling responses 
- handling errors 
- proxy support 

DiscordAPI is responsible for:
- working with Discord REST API v9 
- generating endpoints 
- deserializing models 
- setting headers

---

## Client API

### GetMe
Returns information about the current user.
``` C#
public async Task<DiscordUser> GetMe()
```
Returns:
- `DiscordUser`

### GetAvatar
Loads the current user's avatar.
``` C#
public async Task<Image?> GetAvatar()
```
Returns:
- Image or null

### GetGuildsAsync
Returns a list of the user's servers.
``` C#
public async Task<IReadOnlyList<DiscordGuild>> GetGuildsAsync()
```

### GetGuildInfoAsync
Gets information about a server.
``` C#
public async Task<DiscordGuild> GetGuildInfoAsync(string guildId)
```
Parameters:
- guildId - ID of the server

### GetGuildChannelsAsync
Gets channels from