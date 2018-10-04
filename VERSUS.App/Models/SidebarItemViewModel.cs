using System.Linq;
using KenticoCloud.Delivery;

namespace VERSUS.App.Models
{
	public class SidebarItemViewModel
	{
		public readonly Asset Icon;

		public SidebarItemViewModel(object sidebarItem)
		{
			if (sidebarItem.GetType() == typeof(SiteSection))
			{
				Icon = ((SiteSection)sidebarItem).Icon.First();
			}
			else if (sidebarItem.GetType() == typeof(SiteLogo))
			{
				Icon = ((SiteLogo)sidebarItem).Logo.First();
			}
		}
	}
}