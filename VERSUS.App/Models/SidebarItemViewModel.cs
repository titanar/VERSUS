using System.Linq;

using KenticoCloud.Delivery;

using VERSUS.Kentico.Types;

namespace VERSUS.App.Models
{
    public class SidebarItemViewModel
    {
        public readonly Asset Icon;

        public string Title { get; private set; }

        public SidebarItemViewModel(object sidebarItem)
        {
            if (sidebarItem.GetType() == typeof(SiteSection) && sidebarItem is SiteSection sectionItem)
            {
                Icon = sectionItem.Icon.First();
                Title = sectionItem.Title;
            }
            else if (sidebarItem.GetType() == typeof(SiteLogo) && sidebarItem is SiteLogo logoItem)
            {
                Icon = logoItem.Logo.First();
            }
        }
    }
}