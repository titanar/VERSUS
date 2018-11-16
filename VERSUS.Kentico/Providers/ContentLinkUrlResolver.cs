using KenticoCloud.Delivery;

using Microsoft.Extensions.Options;

using VERSUS.Core;

namespace VERSUS.Kentico.Providers
{
    public class ContentLinkUrlResolver : IContentLinkUrlResolver
    {
        private readonly VersusOptions _versusOptions;

        /// <summary>
        /// Resolves a broken link URL.
        /// </summary>
        /// <returns>A relative URL to the site's 404 page.</returns>
        public string ResolveBrokenLinkUrl()
        {
            return $"{_versusOptions.ErrorHandlingRoute}/404";
        }

        /// <summary>
        /// Resolves the link URL.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <returns>A relative URL to the page where the content is displayed.</returns>
		public string ResolveLinkUrl(ContentLink link)
        {
            return $"{_versusOptions.KenticoCloudUrlSlugEndpoint}/{link.UrlSlug}";
        }

        public ContentLinkUrlResolver(IOptionsSnapshot<VersusOptions> versusOptions)
        {
            _versusOptions = versusOptions.Value;
        }
    }
}