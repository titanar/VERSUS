using VERSUS.Kentico.Types;

namespace VERSUS.App.Models
{
    public class AnnouncementViewModel
    {
        public string Title => "Did you know";

        public string Announcement { get; set; }

        public AnnouncementLevelEnum Level => AnnouncementLevelEnum.Info;

        public AnnouncementViewModel(Site site, AnnouncementTypeEnum announcementType)
        {
            switch (announcementType)
            {
                case AnnouncementTypeEnum.Top:
                    Announcement = site.Announcement;
                    break;

                case AnnouncementTypeEnum.Bottom:
                    break;
            }
        }
    }
}