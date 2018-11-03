using KenticoCloud.Delivery;

namespace VERSUS.App.Resolvers
{
    public class VersusContentLinkUrlResolver : IContentLinkUrlResolver
    {
        /// <summary>
        /// Resolves a broken link URL.
        /// </summary>
        /// <returns>A relative URL to the site's 404 page.</returns>
        public string ResolveBrokenLinkUrl()
        {
            return "/Error/404";
        }

        /// <summary>
        /// Resolves the link URL.
        /// </summary>
        /// <param name="link">The link.</param>
        /// <returns>A relative URL to the page where the content is displayed.</returns>
		public string ResolveLinkUrl(ContentLink link)
        {
            return $"/{link.UrlSlug}";
        }
    }
}