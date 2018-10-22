using System;
using System.Collections.Generic;
using System.Linq;

using KenticoCloud.Delivery;

using VERSUS.Kentico.Models;

namespace VERSUS.Kentico
{
	public class VersusTypeProvider : ICodeFirstTypeProvider
	{
		private static readonly Dictionary<Type, string> _codenames = new Dictionary<Type, string>
		{
			{typeof(Shirt), "shirt"},
			{typeof(Site), "site"},
			{typeof(SiteLogo), "site_logo"},
			{typeof(SiteSection), "site_section"}
		};

		public Type GetType(string contentType)
		{
			return _codenames.Keys.FirstOrDefault(type => GetCodename(type).Equals(contentType));
		}

		public string GetCodename(Type contentType)
		{
			return _codenames.TryGetValue(contentType, out var codename) ? codename : null;
		}
	}
}