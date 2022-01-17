using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreDemoApp.GigyaApi;

namespace AspNetCoreDemoApp.DsStore
{
    public class HttpJsonClient
    {
        private readonly HttpClient _client = new HttpClient();

        public HttpJsonClient()
        {
        }

        public async Task<T> Get<T>(HttpGetRequest request, CancellationToken cancellationToken)
        {
            var response = await request.SendAsync(_client, cancellationToken);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            // Console.WriteLine(jsonResponse);
            return JsonSerializer.Deserialize<T>(jsonResponse,SerializerOptions);
        }

        public async Task<GStatus> Send(HttpGetRequest request, CancellationToken cancellationToken)
        {
            var response = await request.SendAsync(_client, cancellationToken);
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<GStatus>(jsonResponse, SerializerOptions);
        }

        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            IgnoreNullValues = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };
    }
}