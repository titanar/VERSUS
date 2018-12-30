using VERSUS.Kentico.Types;

namespace VERSUS.App.Models
{
    public class CookieConsentViewModel : AnnouncementViewModel
    {
        public string CookieString { get; }

        public CookieConsentViewModel(Announcement announcement, string cookieString) : base(announcement)
        {
            CookieString = cookieString;
        }
    }
}