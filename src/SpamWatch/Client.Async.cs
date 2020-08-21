using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SpamWatch.Enums;
using SpamWatch.Models;

namespace SpamWatch
{
    public partial class Client
    {
        /// <summary>
        /// Get the API version
        /// </summary>
        /// <returns>SpamWatchVersion</returns>
        public async Task<SpamWatchVersion> GetVersionAsync()
        {
            var response = await MakeRequestAsync("version", Method.GET);
            return JsonConvert.DeserializeObject<SpamWatchVersion>(
                response[0] as string ?? throw new Error("Something went wrong"));
        }

        /// <summary>
        /// Get all tokens.
        /// Requires Root permission
        /// </summary>
        /// <returns>SpamWatchToken</returns>
        public async Task<List<SpamWatchToken>> GetTokensAsync()
        {
            var response = await MakeRequestAsync("tokens", Method.GET);
            return JsonConvert.DeserializeObject<List<SpamWatchToken>>(
                response[0] as string ?? throw new Error("Something went wrong"));
        }

        /// <summary>
        /// Creates a token with the given parameters
        /// Requires Root permission
        /// </summary>
        /// <param name="userId">The Telegram User ID of the token owner</param>
        /// <param name="permission">The permission level the Token should have</param>
        /// <returns>The created Token</returns>
        public async Task<SpamWatchToken> CreateTokenAsync(int userId, Permissions permission)
        {
            
            object body;
            if (permission == Permissions.Root)
                body = new {id = userId, permission = "Root"};
            else if (permission == Permissions.Admin)
                body = new {id = userId, permission = "Admin"};
            else if (permission == Permissions.User)
                body = new {id = userId, permission = "User"};
            else
                body = null;

            var response = await MakeRequestAsync("tokens", Method.POST, body);

            return JsonConvert.DeserializeObject<SpamWatchToken>(
                response[0] as string ?? throw new Error("Something went wrong"));
        }

        /// <summary>
        /// Gets the Token that the request was made with.
        /// </summary>
        /// <returns>Current Token</returns>
        public async Task<SpamWatchToken> GetSelfAsync()
        {
            var response = await MakeRequestAsync("tokens/self", Method.GET);
            return JsonConvert.DeserializeObject<SpamWatchToken>(
                response[0] as string ?? throw new Error("Something went wrong"));
        }

        /// <summary>
        /// Get a token using its ID
        /// Requires Root permission
        /// </summary>
        /// <param name="tokenId">The Token ID</param>
        /// <returns>The Token</returns>
        public async Task<SpamWatchToken> GetTokenAsync(int tokenId)
        {
            var response = await MakeRequestAsync($"tokens/{tokenId}", Method.GET);
            return JsonConvert.DeserializeObject<SpamWatchToken>(
                response[0] as string ?? throw new Error("Something went wrong"));
        }

        /// <summary>
        /// Delete a token using its ID
        /// </summary>
        /// <param name="tokenId">The ID of the Token</param>
        public async Task DeleteTokenAsync(int tokenId)
        {
            await MakeRequestAsync($"tokens/{tokenId}", Method.DELETE);
        }

        /// <summary>
        /// Get a list with SpamWatchBan object with data of of all bans
        /// Requires Admin Permission
        /// </summary>
        /// <returns>A list of Bans</returns>
        public async Task<List<SpamWatchBan>> GetBansAsync()
        {
            var response = await MakeRequestAsync("banlist", Method.GET);
            return JsonConvert.DeserializeObject<List<SpamWatchBan>>(
                response[0] as string ?? throw new Error("Something went wrong"));
        }

        /// <summary>
        /// Get a list of all banned user IDs
        /// </summary>
        /// <returns></returns>
        public async Task<List<long>> GetBansMinAsync()
        {
            var response = await MakeRequestAsync("banlist/all", Method.GET);
            var content = response[0] as string;
            return content.Split(Environment.NewLine.ToCharArray()).Select(long.Parse).ToList();
        }

        /// <summary>
        /// Adds a ban
        /// </summary>
        /// <param name="userId">ID of the banned user</param>
        /// <param name="reason">Reason why the user was banned</param>
        /// <param name="message">The message that lead to the current ban. (Optional)</param>
        public async Task AddBanAsync(int userId, string reason, string message = null)
        {
            var body = new List<object>();

            if (message == null)
                body.Add(new
                {
                    id = userId, reason
                });
            else
                body.Add(new
                {
                    id = userId,
                    reason,
                    message
                });

            await MakeRequestAsync("banlist", Method.POST, body);
        }

        /// <summary>
        /// Gets a ban
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>SpamWatchBan object or None</returns>
        public async Task<SpamWatchBan> GetBanAsync(int userId)
        {
            var response = await MakeRequestAsync($"banlist/{userId}", Method.GET);
            return JsonConvert.DeserializeObject<SpamWatchBan>(
                response[0] as string ?? throw new Error("Something went wrong"));
        }

        /// <summary>
        /// Remove a ban
        /// </summary>
        /// <param name="userId">ID of the user</param>
        public async Task DeleteBanAsync(int userId)
        {
            await MakeRequestAsync($"banlist/{userId}", Method.DELETE);
        }

        /// <summary>
        /// Get ban stats
        /// </summary>
        /// <returns></returns>
        public async Task<SpamWatchStats> StatsAsync()
        {
            var response = await MakeRequestAsync("stats", Method.GET);
            return JsonConvert.DeserializeObject<SpamWatchStats>(
                response[0] as string ?? throw new Error("Something went wrong"));
        }

        private async Task<object[]> MakeRequestAsync(string path, Method method, object body = null)
        {
            var request = new RestRequest(path, method);
            if (body != null)
            {
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(body);
            }

            var response = await _client.ExecuteAsync(request);
            
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                return new object[] {response.Content, response};
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return new object[] {string.Empty};
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException("Make sure your Token is correct");
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new ForbiddenException($"Your tokens permission '{Token.Permission}' is not high enough.");
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundException($"{path} does not exist.");
            }
            else if (response.StatusCode == (HttpStatusCode)429)
            {
                var data = (JObject) JsonConvert.DeserializeObject(response.Content);
                var unix = (long) data["until"];
                DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0, DateTimeKind.Utc);
                var until = dtDateTime.AddSeconds(unix).ToLocalTime();
                throw new TooManyRequests(path, until);
            }
            else
            {
                return new object[] {string.Empty, response};
            }
        }
    }
}