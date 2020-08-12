# SpamWatch API C# Client
![Nuget](https://img.shields.io/nuget/v/SpamWatch?style=for-the-badge)

#### Install from Nuget

```
$ dotnet add package SpamWatch --version 1.0.0
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
using System;
using SpamWatch;

namespace SpamWatchExample
{
    internal class Program
    {
        private const string _token = "Your token here";

        private static void Main(string[] args)
        {
            var client = new Client(_token);

            // Get your token permission

            var permission = client.Token.Permission;
            Console.WriteLine($"Current token permission: {permission}");
            
            // Get user ban

            var ban = client.GetBan(638997860);
            Console.WriteLine(ban.Id);
            Console.WriteLine(ban.Reason);
            Console.WriteLine(ban.Date);
            Console.WriteLine(ban.Admin);
        }
    }
}
```