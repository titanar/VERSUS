using System.Linq;
using KenticoCloud.Delivery;
using VERSUS.Kentico.Extensions;
using VERSUS.Kentico.Types;

namespace VERSUS.App.Models
{
    public class AnnouncementViewModel
    {
        public string Title { get; }

        public IRichTextContent Body { get; }

        public AnnouncementLevelEnum Level { get; } = AnnouncementLevelEnum.Info;

        public AnnouncementLocationEnum Location { get; } = AnnouncementLocationEnum.Top;

        public AnnouncementViewModel(Announcement announcement)
        {
            Title = announcement.Title;
            Body = announcement.Body;

            if (announcement.Level.FirstOrDefault() is MultipleChoiceOption announcementLevel)
            {
                Level = announcementLevel.ToEnum<AnnouncementLevelEnum>();
            }

            if (announcement.Location.FirstOrDefault() is MultipleChoiceOption announcementLocation)
            {
                Location = announcementLocation.ToEnum<AnnouncementLocationEnum>();
            }
        }
    }
}