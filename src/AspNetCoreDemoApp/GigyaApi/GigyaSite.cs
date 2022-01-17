using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Elsa.Services.Models;
using Jint.Native.Json;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using EnumerableExtensions = NetBox.Extensions.EnumerableExtensions;
using HttpMethod = System.Net.Http.HttpMethod;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AspNetCoreDemoApp.GigyaApi
{
    public class GigyaSite
    {
        public string ApiKey { get; set; }
        public string Domain { get; set; }


        public GigyaSite(string apiKey = null, string domain = "gigya.com")
        {
            ApiKey = apiKey;
            Domain = domain;
        }


        public Uri AccountsPath(string path) => new Uri($"https://accounts.{Domain}/{path}?apiKey={ApiKey}");

        public static GigyaSite Test =
            new GigyaSite("3_VL0lfWLluGwf2VZ5niQd4Xx6HFf6hSdYHfHoDMJDF2njekgvaEbnxryRAsaXwZK2", "us1.gigya.com");

        public IEnumerable<(string name, string value)> SiteParams()
        {
            yield return (name: "apiKey", value: ApiKey);
        }

        public HttpGetRequest Apply(HttpGetRequest request)
        {
            request.Param = ("apiKey", ApiKey);
            return request;
        }
    }

    public class Api
    {
        private readonly GigyaSite _site;
        private readonly GigyaCreds _creds;

        public Api(GigyaSite site, GigyaCreds creds)
        {
            _site = site;
            _creds = creds;
        }

        public Namespace accounts => new Namespace(_site, _creds);
        public Namespace ds => new Namespace(_site, _creds);
    }

    public class DsGetRequest : Request
    {
        public DsGetRequest(string oid, string type) : base("ds.get",
            new Dictionary<string, string> {[nameof(oid)] = oid, [nameof(type)] = type})
        {
        }
    }

    public class DsDeleteRequest : Request
    {
        public DsDeleteRequest(string oid, string type) : base("ds.delete",
            new Dictionary<string, string> {[nameof(oid)] = oid, [nameof(type)] = type})
        {
        }
    }

    public class DsStoreRequest : Request
    {
        public DsStoreRequest(string oid, string type, object data) : base("ds.store",
            new Dictionary<string, string>
            {
                [nameof(oid)] = oid, [nameof(type)] = type,
                [nameof(data)] = JsonSerializer.Serialize(data)
            })
        {
        }
    }

    public class DsSearchRequest : Request
    {
        public DsSearchRequest(string query) : base("ds.search",
            new Dictionary<string, string>
            {
                [nameof(query)] = query
            })
        {
        }
    }

    public class Namespace
    {
        private readonly GigyaSite _site;
        private readonly GigyaCreds _creds;
        private readonly string _name;

        public Namespace(GigyaSite site, GigyaCreds creds, [CallerMemberName] string name = null)
        {
            _site = site;
            _creds = creds;
            _name = name;
        }


        public HttpGetRequest GetRequest(Request request) =>
            _creds.Apply(_site.Apply(new HttpGetRequest(request)
            {
                Host = Domain()
            }));

        public string Domain() => $"{_name}.{_site.Domain}";
    }


    public class Request
    {
        public string Api { get; }
        public Dictionary<string, string> Params { get; }

        public Request(string api = "gs/ver.htm", Dictionary<string, string> @params = null)
        {
            Api = api;
            Params = @params ?? new Dictionary<string, string>();
        }

        public (string name, string value) Param
        {
            set => Params[value.name] = value.value;
        }

        public IEnumerable<(string name, string value)> SetParams
        {
            set { _ = EnumerableExtensions.ForEach(value, param => Params[param.name] = param.value); }
        }
    }


    public class HttpRequest
    {
        public Request Request { get; set; }
        public string Host { get; set; }

        public HttpRequest(Request request, string host = null)
        {
            Request = request;
            Host = host;
        }

        public string Scheme = HttpScheme.Https.ToString();

        public IEnumerable<(string name, string value)> SetParams
        {
            set => Request.SetParams = value;
        }

        public (string name, string value) Param
        {
            set => Request.Param = value;
        }
    }

    public class HttpGetRequest : HttpRequest
    {
        public HttpGetRequest(Request request) : base(request)
        {
        }

        public Uri Uri()
        {
            return new UriBuilder()
            {
                Scheme = Scheme,
                Host = Host,
                Path = Request.Api,
                Query = Query()
            }.Uri;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpClient client, CancellationToken cancellationToken)
        {
            return await client.SendAsync(new HttpRequestMessage(HttpMethod.Get,  Uri()), cancellationToken);

        }


        private string Query() =>
            Request
                .Params
                .Select(tuple => $"{HttpUtility.UrlEncode(tuple.Key)}={HttpUtility.UrlEncode(tuple.Value)}")
                .Aggregate("?", (query, param) => $"{query}{param}&")
                .TrimEnd('&');


        // public Request SetParam(string name, string value)
        // {
        //      Params[name] = value;
        //      return this;
        // }
    }

    public class GigyaCreds
    {
        public string UserKey { get; set; }
        public string Secret { get; set; }


        public GigyaCreds(string userKey, string secret)
        {
            UserKey = userKey;
            Secret = secret;
        }

        public static GigyaCreds Test =
            new GigyaCreds("AIvzv0Pv8IFX", "e6DgUArTi5mGQgaqqAzPdFNYiWaPNaqu");


        public HttpGetRequest Apply(HttpGetRequest request)
        {
            request.Param = (name: "userKey", value: UserKey);
            request.Param = (name: "secret", value: Secret);

            return request;
        }

        public IEnumerable<(string name, string value)> CredsParams()
        {
            yield return (name: "userKey", value: UserKey);
            yield return (name: "secret", value: Secret);
        }

        public Uri ApplyCreds(Uri uri)
        {
            return uri
                .AddQuery("userKey", UserKey)
                .AddQuery("secret", Secret);
        }
    }

    public static class SiteApiExecutionContextExtensions
    {
        public static GigyaSite Site(this ActivityExecutionContext context)
        {
            return context.GetVariable<GigyaSite>() ?? GigyaSite.Test;
        }

        public static GigyaCreds Creds(this ActivityExecutionContext context)
        {
            return context.GetVariable<GigyaCreds>() ?? GigyaCreds.Test;
        }

        public static Api Api(this ActivityExecutionContext context) => new Api(context.Site(), context.Creds());

        public static Namespace AccountsApi(this ActivityExecutionContext context)
        {
            return context
                .Api()
                .accounts;
        }
    }

    public static class HttpExtensions
    {
        public static Uri AddQuery(this Uri uri, string name, string value)
        {
            var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

            httpValueCollection.Remove(name);
            httpValueCollection.Add(name, value);

            var ub = new UriBuilder(uri)
            {
                Query = httpValueCollection.ToString() ?? string.Empty
            };

            return ub.Uri;
        }
    }
}