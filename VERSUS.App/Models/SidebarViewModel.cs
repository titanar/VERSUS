using System.Collections.Generic;
using System.Linq;
using VERSUS.Kentico.Types;

namespace VERSUS.App.Models
{
    public class SidebarViewModel
    {
        public IEnumerable<SidebarItemViewModel> Sidebar { get; set; }

        public SidebarViewModel(Site site)
        {
            Sidebar = site.Sidebar?.Select(s => new SidebarItemViewModel(s));
        }
    }
}