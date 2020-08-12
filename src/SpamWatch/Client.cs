using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SpamWatch.Enums;
using SpamWatch.Models;

namespace SpamWatch
{
    public class Client
    {
        private readonly RestClient _client;
        public readonly SpamWatchToken Token;


        public Client(string token, string baseUrl = "https://api.spamwat.ch")
        {
            _client = new RestClient(baseUrl);
            _client.AddDefaultHeader("Authorization", $"Bearer {token}");
            Token = GetSelf();
        }

        /// <summary>
        /// Get the API version
        /// </summary>
        /// <returns>SpamWatchVersion</returns>
        public SpamWatchVersion GetVersion()
        {
            var response = MakeRequest("version", Method.GET);
            return JsonConvert.DeserializeObject<SpamWatchVersion>(
                response[0] as string ?? throw new Error("Something went wrong"));
        }

        /// <summary>
        /// Get all tokens.
        /// Requires Root permission
        /// </summary>
        /// <returns>SpamWatchToken</returns>
        public List<SpamWatchToken> GetTokens()
        {
            var response = MakeRequest("tokens", Method.GET);
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
        public SpamWatchToken CreateToken(int userId, Permissions permission)
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

            var response = MakeRequest("tokens", Method.POST, body);

            return JsonConvert.DeserializeObject<SpamWatchToken>(
                response[0] as string ?? throw new Error("Something went wrong"));
        }

        /// <summary>
        /// Gets the Token that the request was made with.
        /// </summary>
        /// <returns>Current Token</returns>
        public SpamWatchToken GetSelf()
        {
            var response = MakeRequest("tokens/self", Method.GET);
            return JsonConvert.DeserializeObject<SpamWatchToken>(
                response[0] as string ?? throw new Error("Something went wrong"));
        }

        /// <summary>
        /// Get a token using its ID
        /// Requires Root permission
        /// </summary>
        /// <param name="tokenId">The Token ID</param>
        /// <returns>The Token</returns>
        public SpamWatchToken GetToken(int tokenId)
        {
            var response = MakeRequest($"tokens/{tokenId}", Method.GET);
            return JsonConvert.DeserializeObject<SpamWatchToken>(
                response[0] as string ?? throw new Error("Something went wrong"));
        }

        /// <summary>
        /// Delete a token using its ID
        /// </summary>
        /// <param name="tokenId">The ID of the Token</param>
        public void DeleteToken(int tokenId)
        {
            MakeRequest($"tokens/{tokenId}", Method.DELETE);
        }

        /// <summary>
        /// Get a list with SpamWatchBan object with data of of all bans
        /// Requires Admin Permission
        /// </summary>
        /// <returns>A list of Bans</returns>
        public List<SpamWatchBan> GetBans()
        {
            var response = MakeRequest("banlist", Method.GET);
            return JsonConvert.DeserializeObject<List<SpamWatchBan>>(
                response[0] as string ?? throw new Error("Something went wrong"));
        }

        /// <summary>
        /// Get a list of all banned user IDs
        /// </summary>
        /// <returns></returns>
        public List<long> GetBansMin()
        {
            var response = MakeRequest("banlist/all", Method.GET);
            var content = response[0] as string;
            return content.Split(Environment.NewLine.ToCharArray()).Select(long.Parse).ToList();
        }

        /// <summary>
        /// Adds a ban
        /// </summary>
        /// <param name="userId">ID of the banned user</param>
        /// <param name="reason">Reason why the user was banned</param>
        /// <param name="message">The message that lead to the current ban. (Optional)</param>
        public void AddBan(int userId, string reason, string message = null)
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

            MakeRequest("banlist", Method.POST, body);
        }

        /// <summary>
        /// Gets a ban
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>SpamWatchBan object or None</returns>
        public SpamWatchBan GetBan(int userId)
        {
            var response = MakeRequest($"banlist/{userId}", Method.GET);
            return JsonConvert.DeserializeObject<SpamWatchBan>(
                response[0] as string ?? throw new Error("Something went wrong"));
        }

        /// <summary>
        /// Remove a ban
        /// </summary>
        /// <param name="userId">ID of the user</param>
        public void DeleteBan(int userId)
        {
            MakeRequest($"banlist/{userId}", Method.DELETE);
        }

        /// <summary>
        /// Get ban stats
        /// </summary>
        /// <returns></returns>
        public SpamWatchStats Stats()
        {
            var response = MakeRequest("stats", Method.GET);
            return JsonConvert.DeserializeObject<SpamWatchStats>(
                response[0] as string ?? throw new Error("Something went wrong"));
        }

        #region MakeRequest

        private object[] MakeRequest(string path, Method method, object body = null)
        {
            var request = new RestRequest(path, method);

            if (body != null)
            {
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(body);
            }

            foreach (var requestParameter in request.Parameters) Console.WriteLine(requestParameter.Name);

            var response = _client.Execute(request);

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

        #endregion
    }
}