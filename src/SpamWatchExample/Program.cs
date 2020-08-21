using System;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using SpamWatch;

namespace SpamWatchExample
{
    internal class Program
    {
        private const string _token = "Your token here";

        private static async Task Main(string[] args)
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
            
            // You can also use async methods

            var asyncBan = await client.GetBanAsync(638997860);
            Console.WriteLine(ban.Id);
            Console.WriteLine(ban.Reason);
            Console.WriteLine(ban.Date);
            Console.WriteLine(ban.Admin);
        }
    }
}