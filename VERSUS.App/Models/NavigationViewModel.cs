using System.Collections.Generic;
using System.Linq;
using VERSUS.Kentico.Types;

namespace VERSUS.App.Models
{
    public class NavigationViewModel
    {
        public IEnumerable<SidebarItemViewModel> Sidebar { get; set; }

        public NavigationViewModel(Site site)
        {
            Sidebar = site.Sidebar?.Select(s => new SidebarItemViewModel(s));
        }
    }
}