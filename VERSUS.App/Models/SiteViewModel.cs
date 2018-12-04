using System.Collections.Generic;
using System.Linq;

using VERSUS.Kentico.Types;

namespace VERSUS.App.Models
{
    public class SiteViewModel
    {
        public IEnumerable<SidebarItemViewModel> Sidebar { get; set; }

        public string Announcement { get; set; }

        public SiteViewModel(Site site)
        {
            Announcement = site.Announcement;
            Sidebar = site.Sidebar?.Select(s => new SidebarItemViewModel(s));
        }
    }
}