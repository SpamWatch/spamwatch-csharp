# SpamWatch API C# Client

#### Install from Nuget

![Nuget](https://img.shields.io/nuget/v/SpamWatch?style=for-the-badge)
![Nuget](https://img.shields.io/nuget/dt/SpamWatch?style=for-the-badge)

```
$ dotnet add package SpamWatch --version 1.1.2
```

#### Build from source

```
$ git clone https://github.com/SpamWatch
$ cd SpamWatch/src/SpamWatch
$ dotnet restore
$ dotnet build
```

#### Basic Usage

```c#
           var client = new Client(_token, baseUrl: "https://sapi.spamwat.ch/");
            
            
            // Get your token details

            var self = client.GetSelf();
            
            
            // You can also use any method async

            self = await client.GetSelfAsync();
            
            Console.WriteLine($"Current token permission: {self.UserId}");
            
            // You can also get any type as JSON strings
            Console.WriteLine(self.SerializeObject());
            
            // Get a Token using the token id

            var token = client.GetToken(self.Id);

            // Get all created Tokens

            var listTokens = client.GetTokens();
            
            
            // Create a token
            
            var newToken = new Token()
            {
                UserId = 638997860,
                Permission = Permissions.User
            };
            
            token = client.CreateToken(newToken);
            
            // Delete a specific Token
            
            client.DeleteToken(token.Id);
            
            // Or
            
            client.DeleteToken(token);
            
            // Get a specific ban

            var ban = client.GetBan(638997860);
            
            // Get a list with all the bans

            var allBans = client.GetBans();
            
            // Get a list with all banned ids

            var allIdsBanned = client.GetBansMin();
            
            // Add a ban
            
            var newBan = new Ban()
            {
                UserId = 638997860,
                Reason = "Ban Reason",
                Message = "Telegram Message that got the user banned"
            };

            client.AddBan(newBan);

            // Delete a ban
            
            client.DeleteBan(638997860);
            
            // or
            
            client.DeleteBan(newBan);
```