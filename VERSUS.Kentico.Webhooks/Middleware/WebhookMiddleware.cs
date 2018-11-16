using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

using VERSUS.Core;
using VERSUS.Kentico.Helpers;
using VERSUS.Kentico.Webhooks.Models;
using VERSUS.Kentico.Webhooks.Services;

namespace VERSUS.Kentico.Webhooks.Middleware
{
    public class WebhookMiddleware : IMiddleware
    {
        private readonly string _secret;
        private readonly IWebhookListener<WebhookSubjectModel> _webhookListener;

        public WebhookMiddleware(IOptionsSnapshot<VersusOptions> versusOptions, IWebhookListener<WebhookSubjectModel> webhookListener)
        {
            _secret = versusOptions.Value.KenticoCloudWebhookSecret;
            _webhookListener = webhookListener;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var request = context.Request;

            var (generatedSignature, signatureFromRequest, content) = await ParseRequest(request);

            if (generatedSignature != signatureFromRequest)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                var model = JsonConvert.DeserializeObject<WebhookModel>(content);

                switch (model.Message.Type)
                {
                    case KenticoCloudCacheHelper.CONTENT_ITEM_SINGLE_IDENTIFIER:
                    case KenticoCloudCacheHelper.CONTENT_ITEM_VARIANT_SINGLE_IDENTIFIER:
                    case KenticoCloudCacheHelper.CONTENT_TYPE_SINGLE_IDENTIFIER:
                        NotifyWebhookSubscribers(model.Message.Operation, model.Message.Type, model.Data.Items);
                        break;

                    case KenticoCloudCacheHelper.TAXONOMY_GROUP_SINGLE_IDENTIFIER:
                        NotifyWebhookSubscribers(model.Message.Operation, model.Message.Type, model.Data.Taxonomies);
                        break;
                }

                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
        }

        private async Task<(string generatedSignature, string signatureFromRequest, string content)> ParseRequest(HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                request.Headers.TryGetValue("X-Kc-Signature", out var signatureFromRequest);

                request.EnableBuffering();
                request.Body.Position = 0;
                var content = await reader.ReadToEndAsync();

                return (GenerateHash(content), signatureFromRequest, content);
            }
        }

        private string GenerateHash(string content)
        {
            var secret = _secret ?? "";
            var safeUTF8 = new UTF8Encoding(false, true);
            byte[] keyBytes = safeUTF8.GetBytes(secret);
            byte[] messageBytes = safeUTF8.GetBytes(content);

            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                byte[] hashMessage = hmacsha256.ComputeHash(messageBytes);

                return Convert.ToBase64String(hashMessage);
            }
        }

        private void NotifyWebhookSubscribers(string operation, string typeName, IEnumerable<IWebhookCodenamedData> data)
        {
            foreach (var item in data)
            {
                _webhookListener.WebhookObservable.OnNext(
                    new WebhookSubjectModel
                    {
                        TypeName = typeName,
                        Codename = item.Codename,
                        Operation = operation
                    }
                );
            }
        }
    }
}