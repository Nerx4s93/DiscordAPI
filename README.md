### [-----EN-----] [[-----RU-----]](docs/README_RU.md)

# DiscordAPI
.NET client for Discord REST API (v9) with support for custom tokens.

Suitable for automation, scripts, and custom clients.

## Installation
Currently only available through the source code:
```bash
git clone <repo>
dotnet build
```

## Authentication
1. **Open Discrod in the browser**.
2. **Activate the developer tools:**
 * Windows/Linux: `F12` or `Ctrl+Shift+I`
 * macOS: `Cmd+Option+I`
3. **Go to the `Network` tab**.
4. **Start typing** in any active chat. This will trigger the "typing" event.
5. **Find the desired request.** In the list of requests, look for the one called `typing` (you can use the search or filter).
6. **Examine the request headers:**
 * Click on the found `typing` request.
 * In the opened panel, find the `Request Headers` section.
 * Scroll down the list of headers to the `Authorization` field.
7. **Copy the value.** Select and copy

### ⚠️ Important
**Never** commit the token to Git. Add the token file to `.gitignore`.
Remember: this token gives full access to your Discrod account.
### ❓ Why is it so difficult?
Discrod does not provide an official way to obtain a custom token for scripts.

## Quick start
``` csharp
using var client = new DiscordClient("YOUR_TOKEN");

// Information about yourself
var me = await client.GetMeAsync();
Console.WriteLine($"[{me.Username}]");

// List of servers
var guilds = await client.GetGuildsAsync();
foreach (var guild in guilds)
{
 Console.WriteLine($"Server: {guild.Name} (members: {guild.MemberCount})");
 
 // Server Channels
 var channels = await client.GetGuildChannelsAsync(guild.Id);
 Console.WriteLine($"Channels: {channels.Count}");
}

// Channel Messages
var messages = await client.GetChannelMessagesAsync("123456789", limit: 10);
Console.WriteLine($"Recent messages: {messages.Count}");
```

## ⚠️ Disclaimer
This project is not affiliated with Discord Inc.

The use of custom tokens for automation may violate Discord's Terms of Service. The library is intended for educational purposes only.

By using this software, you agree that you are solely responsible for any consequences, including temporary or permanent account bans.