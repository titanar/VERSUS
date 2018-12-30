using KenticoCloud.Delivery;

using Microsoft.Extensions.Options;

using VERSUS.Core;

namespace VERSUS.Kentico.Providers
{
    public class ContentLinkUrlResolver : IContentLinkUrlResolver
    {
        private readonly string _errorHandlingRoute;
        private readonly string _kenticoCloudUrlSlugEndpoint;

        public ContentLinkUrlResolver(IOptionsSnapshot<VersusOptions> versusOptions, IOptionsSnapshot<KenticoOptions> kenticoOptions)
        {
            _errorHandlingRoute = versusOptions.Value.ErrorHandlingRoute;
            _kenticoCloudUrlSlugEndpoint = kenticoOptions.Value.KenticoCloudUrlSlugEndpoint;
        }

        /// <summary>
        /// Resolves a broken link URL.
        /// </summary>
        /// <returns>A relative URL to the site's 404 page.</returns>
        public string ResolveBrokenLinkUrl()
        {
            return $"{_errorHandlingRoute}/404";
        }

        /// <summary>
        /// Resolves the link URL.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <returns>A relative URL to the page where the content is displayed.</returns>
		public string ResolveLinkUrl(ContentLink link)
        {
            return $"{_kenticoCloudUrlSlugEndpoint}/{link.UrlSlug}";
        }
    }
}