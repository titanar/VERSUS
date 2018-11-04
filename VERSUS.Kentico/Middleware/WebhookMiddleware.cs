using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace VERSUS.Kentico.Middleware
{
    public class WebhookMiddleware : IMiddleware
    {
        private readonly string _secret;
        private readonly IWebhookListener _webhookListener;

        public WebhookMiddleware(IOptionsSnapshot<VersusOptions> versusOptions, IWebhookListener webhookListener)
        {
            _secret = versusOptions.Value.KenticoCloudWebhookSecret;
            _webhookListener = webhookListener;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var signature = context.Request.Headers["X-Kc-Signature"].FirstOrDefault();
            var request = context.Request;

            using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                context.Request.EnableBuffering();

                request.Body.Position = 0;
                var content = reader.ReadToEnd();
                request.Body.Position = 0;

                var generatedSignature = GenerateHash(content, _secret);

                if (generatedSignature != signature)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await next(context);
                }
                else
                {
                    var model = JsonConvert.DeserializeObject<WebhookModel>(content);

                    switch (model.Message.Type)
                    {
                        case KenticoCloudCacheHelper.CONTENT_ITEM_SINGLE_IDENTIFIER:
                        case KenticoCloudCacheHelper.CONTENT_ITEM_VARIANT_SINGLE_IDENTIFIER:
                        case KenticoCloudCacheHelper.CONTENT_TYPE_SINGLE_IDENTIFIER:
                            RaiseNotificationForSupportedOperations(model.Message.Operation, model.Message.Type, model.Data.Items);
                            break;

                        case KenticoCloudCacheHelper.TAXONOMY_GROUP_SINGLE_IDENTIFIER:
                            RaiseNotificationForSupportedOperations(model.Message.Operation, model.Message.Type, model.Data.Taxonomies);
                            break;
                    }
                }
            }
        }

        private static string GenerateHash(string message, string secret)
        {
            secret = secret ?? "";
            var safeUTF8 = new UTF8Encoding(false, true);
            byte[] keyBytes = safeUTF8.GetBytes(secret);
            byte[] messageBytes = safeUTF8.GetBytes(message);

            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                byte[] hashMessage = hmacsha256.ComputeHash(messageBytes);

                return Convert.ToBase64String(hashMessage);
            }
        }

        private void RaiseNotificationForSupportedOperations(string operation, string artefactType, IEnumerable<IWebhookCodenamedData> data)
        {
            foreach (var item in data)
            {
                _webhookListener.WebhookObservable.OnNext(new CacheInvalidationModel(
                        new CacheTokenPair
                        {
                            TypeName = artefactType,
                            Codename = item.Codename
                        },
                        operation
                    ));
            }
        }
    }
}