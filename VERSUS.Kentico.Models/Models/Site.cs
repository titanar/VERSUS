// This code was generated by a cloud-generators-net tool
// (see https://github.com/Kentico/cloud-generators-net).
//
// Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// For further modifications of the class, create a separate file with the partial class.

using System.Collections.Generic;

using KenticoCloud.Delivery;

namespace VERSUS.Kentico.Models
{
    public partial class Site
    {
        public const string Codename = "site";
        public const string SidebarCodename = "sidebar";
        public const string HeroItemsOptionsCodename = "hero_items_options";
        public const string HeroItemsCodename = "hero_items";
        public const string AnnouncementCodename = "announcement";

        public IEnumerable<object> Sidebar { get; set; }

        public IEnumerable<MultipleChoiceOption> HeroItemsOptions { get; set; }

        public IEnumerable<object> HeroItems { get; set; }

        public string Announcement { get; set; }

        public ContentItemSystemAttributes System { get; set; }
    }
}