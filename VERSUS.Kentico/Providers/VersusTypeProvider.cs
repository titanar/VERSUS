using System;
using System.Collections.Generic;
using System.Linq;

using KenticoCloud.Delivery;

using VERSUS.Core;
using VERSUS.Kentico.Models;

namespace VERSUS.Kentico
{
	public class VersusTypeProvider : ICodeFirstTypeProvider
	{
        private static readonly HashSet<(Type, string)> codenames = new HashSet<(Type, string)>(
                                                                            AppDomain.CurrentDomain.GetAssemblies()
                                                                                .SelectMany(a => a.GetTypes())
                                                                                .Where(t => t.IsClass && t.Namespace == VersusGlobals.VERSUS_KENTICO_MODELS_NAMESPACE)
                                                                                .Select(t => (t, t.GetField("Codename").GetValue(null).ToString())));

        /// <summary>
        /// Get the strong type of the given Kentico Cloud content type codename.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
		public Type GetType(string contentType)
		{
            return codenames.FirstOrDefault(p => p.Item2 == contentType).Item1;
		}

        /// <summary>
        /// Get the Kentico Cloud content type codename of the given type.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
		public string GetCodename(Type contentType)
		{
            return codenames.FirstOrDefault(p => p.Item1 == contentType).Item2;
		}
	}
}