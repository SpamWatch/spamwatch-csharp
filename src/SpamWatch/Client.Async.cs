using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SpamWatch.Enums;
using SpamWatch.Types;
using SpamWatch.Types.ExceptionTypes;
using Version = SpamWatch.Types.Version;

namespace SpamWatch
{
    public partial class Client
    {
        /// <summary>
        /// Gets the Token that the request was made with.
        /// </summary>
        /// <returns>Token</returns>
        public async Task<Token> GetSelfAsync()
        {
            return await MakeRequestsAsync<Token>("tokens/self", Method.GET);
        }

        /// <summary>
        /// Get a token using its ID
        /// Requires Root permission
        /// </summary>
        /// <param name="tokenId">The Token ID</param>
        /// <returns>The Token</returns>
        public async Task<Token> GetTokenAsync(int tokenId)
        {
            return await MakeRequestsAsync<Token>($"tokens/{tokenId}", Method.GET);
        }

        /// <summary>
        /// Get all tokens.
        /// Requires Root permission
        /// </summary>
        /// <returns><list type="Token"></list></returns>
        public async Task<List<Token>> GetTokensAsync()
        {
            return await MakeRequestsAsync<List<Token>>("tokens", Method.GET);
        }

        /// <summary>
        /// Creates a token with the given parameters
        /// Requires Root permission
        /// </summary>
        /// <param name="token">Token object</param>
        /// <returns>Token</returns>
        public async Task<Token> CreateTokenAsync(Token token)
        {
            return await MakeRequestsAsync<Token>("tokens", Method.POST, token.SerializeObject());
        }

        /// <summary>
        /// Delete a token using its ID
        /// Requires Root permission
        /// </summary>
        /// <param name="tokenId">The ID of the Token</param>
        public async Task DeleteTokenAsync(int tokenId)
        {
            await MakeRequestsAsync($"tokens/{tokenId}", Method.DELETE);
        }

        /// <summary>
        /// Delete a token using a Token object
        /// Requires Root permission
        /// </summary>
        /// <param name="token">Token object</param>
        public async Task DeleteTokenAsync(Token token)
        {
            int tokenId = token.Id;
            await MakeRequestsAsync($"tokens/{tokenId}", Method.DELETE);
        }



        /// <summary>
        /// Gets a ban
        /// </summary>
        /// <param name="userId">ID of the user</param>
        /// <returns>SpamWatchBan object or None</returns>
        public async Task<Ban> GetBanAsync(long userId)
        {
            return await MakeRequestsAsync<Ban>($"banlist/{userId}", Method.GET);
        }

        /// <summary>
        /// Get a list with SpamWatchBan object with data of of all bans
        /// Requires Admin Permission
        /// </summary>
        /// <returns><list type="Ban"></list></returns>
        public async Task<List<Ban>> GetBansAsync()
        {
            return await MakeRequestsAsync<List<Ban>>("banlist", Method.GET);
        }

        /// <summary>
        /// Get a list of all banned user IDs
        /// </summary>
        /// <returns><list type="long"></list></returns>
        public async Task<List<long>> GetBansMinASsync()
        {
            var response = await MakeRequestsAsync("banlist/all", Method.GET);
            return response.Split(Environment.NewLine.ToCharArray()).Select(long.Parse).ToList();
        }

        /// <summary>
        /// Adds a ban
        /// Requires Admin Permission
        /// </summary>
        /// <param name="ban">Ban object</param>
        public async Task AddBanAsync(Ban ban)
        {
            await MakeRequestsAsync("banlist", Method.POST, ban.SerializeObject());
        }

        /// <summary>
        /// Remove a ban
        /// Requires Admin Permission
        /// </summary>
        /// <param name="userId">ID of the user</param>
        public async Task DeleteBanAsync(long userId)
        {
            await MakeRequestsAsync($"banlist/{userId}", Method.DELETE);
        }

        /// <summary>
        /// Remove a ban
        /// Requires Admin Permission
        /// </summary>
        /// <param name="ban">Ban object</param>
        public async Task DeleteBanAsync(Ban ban)
        {
            long userId = ban.UserId;
            await MakeRequestsAsync($"banlist/{userId}", Method.DELETE);
        }

        /// <summary>
        /// Get ban stats
        /// </summary>
        /// <returns></returns>
        public async Task<Stats> GetStatsAsync()
        {
            return await MakeRequestsAsync<Stats>("stats", Method.GET);
        }

        private async Task<T> MakeRequestsAsync<T>(string path, Method method, string body = null)
        {
            var request = new RestRequest(path, method);

            if (body != null)
            {
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(body);
            }

            var response = await _client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var badRequest = JsonConvert.DeserializeObject<BadRequest>(response.Content);
                throw new BadRequestException(badRequest.Reason);
            }
            
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return default(T);
            }
            
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException("Make sure your Token is correct");
            }

            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                request = new RestRequest("tokens/self", Method.GET);
                var self = _client.Execute<Token>(request);
                throw new ForbiddenException($"Your tokens permission '{self.Data.Permission}' is not high enough.");
            }
            
            else if (response.StatusCode == (HttpStatusCode) 429)
            {
                var tooManyRequests = JsonConvert.DeserializeObject<TooManyRequests>(response.Content);
                
                DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0, DateTimeKind.Utc);
                var until = dtDateTime.AddSeconds(tooManyRequests.Until).ToLocalTime();
                throw new TooManyRequestsException(path, until);
            }

            return JsonConvert.DeserializeObject<T>(response.Content);
        }
        
        private async Task<string> MakeRequestsAsync(string path, Method method, string body = null)
        {
            var request = new RestRequest(path, method);

            if (body != null)
            {
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(body);
            }

            var response = await _client.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var badRequest = JsonConvert.DeserializeObject<BadRequest>(response.Content);
                throw new BadRequestException(badRequest.Reason);
            }
            
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return String.Empty;
            }
            
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException("Make sure your Token is correct");
            }

            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                request = new RestRequest("tokens/self", Method.GET);
                var self = _client.Execute<Token>(request);
                throw new ForbiddenException($"Your tokens permission '{self.Data.Permission}' is not high enough.");
            }
            
            else if (response.StatusCode == (HttpStatusCode) 429)
            {
                var tooManyRequests = JsonConvert.DeserializeObject<TooManyRequests>(response.Content);
                
                DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0, DateTimeKind.Utc);
                var until = dtDateTime.AddSeconds(tooManyRequests.Until).ToLocalTime();
                throw new TooManyRequestsException(path, until);
            }

            return response.Content;
        }
    }
}