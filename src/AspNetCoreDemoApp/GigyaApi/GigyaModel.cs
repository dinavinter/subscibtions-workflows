using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AspNetCoreDemoApp.GigyaApi
{

    public class GigyaModel
    {
        public readonly string ApiKey;
        public readonly string Domain;

        public GigyaModel(string apiKey, string domain = "gigya.com")
        {
            ApiKey = apiKey;
            Domain = domain;
        }

        public string Message { get; set; }

        public string ScreenSet { get; set; }
        public string StartScreen { get; set; }

        public IHtmlContent SdkScript()
        {
            return new HtmlString($"<script src='https://cdns.${Domain}/js/gigya.js?apikey=${ApiKey}'></script>");
        }
    }

    public class ErrorMessage
    {
        public string Message { get; set; }
        public string? RequestId { get; set; }
        public int? ErrorCode { get; set; }
        public object? Details { get; set; }

        public ErrorMessage(string message, string? requestId = null, int? errorCode = 0, object? details = null)
        {
            Message = message;
            RequestId = requestId;
            ErrorCode = errorCode;
            Details = details;
        }
    }

    public class GStatus
    {
        public int statusCode;
        public int errorCode;
        public string statusReason;
        public string callId;
        public string errorMessage;
        public string errorDetails;
        public string time;
        public DebugInfo debug;
        public IgnoredParam[] ignoredParams;
        public int apiVersion;
        public bool soa => apiVersion >= 2;

        [JsonIgnore]
        public string kibanaLink =>
            $"http://kibana/kibana3/#/dashboard/elasticsearch/SearchByCallID?my_url_parameter=callID:{callId}";

        public class IgnoredParam
        {
            public string paramName { get; set; }
            public string message { get; set; }
            public int warningCode { get; set; }
        }

        public class DebugInfo
        {
            public JToken exceptionDatas;

            public string exceptionMessage;

            public string offendingService;

            public string stackTrace;
        }

        public ErrorMessage ErrorMessage() => new ErrorMessage(errorMessage, callId, errorCode, this);
    }


    public interface ISearchResults<out TResult>
    {
        TResult[] Results { get; }

        int ObjectsCount { get; }


        int TotalCount { get; }

        string NextCursorId { get; }

        string CallId { get; }

        int ErrorCode { get; }

        string ErrorMessage { get; }
    }
}