using System.Linq;

using KenticoCloud.Delivery;

using VERSUS.Kentico.Extensions;
using VERSUS.Kentico.Types;

namespace VERSUS.App.Models
{
    public class SidebarItemViewModel
    {
        public readonly Asset Icon;

        public string Title { get; }

        public SidebarItemLocationEnum SidebarItemLocation { get; } = SidebarItemLocationEnum.Top;

        public SidebarItemTypeEnum SidebarItemType { get; } = SidebarItemTypeEnum.Link;

        public SidebarItemViewModel(SidebarItem sidebarItem)
        {
            Icon = sidebarItem.Icon.FirstOrDefault();
            Title = sidebarItem.Title;
            SidebarItemLocation = sidebarItem.Location.FirstOrDefault().ToEnum<SidebarItemLocationEnum>();

            if (sidebarItem.Type.FirstOrDefault() is MultipleChoiceOption sidebarItemType)
            {
                SidebarItemType = sidebarItemType.ToEnum<SidebarItemTypeEnum>();
            }
        }
    }
}