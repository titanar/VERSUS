using System;
using System.Collections.Concurrent;
using System.Linq;
using KenticoCloud.Delivery;

namespace VERSUS.Kentico.Extensions
{
    public static class MultipleChoiceOptionExtensions
    {
        private static ConcurrentDictionary<(string, string), Enum> _cachedEnums = new ConcurrentDictionary<(string, string), Enum>();

        public static T ToEnum<T>(this MultipleChoiceOption multipleChoiceOption) where T : struct, Enum
        {
            return (T)_cachedEnums.GetOrAdd((multipleChoiceOption.Codename, typeof(T).FullName), k =>
            {
                if (!Enum.TryParse(Capitalize(k.Item1), out T parsedEnum))
                {
                    //If simple capitalization failed, try ignoring case.
                    Enum.TryParse(k.Item1, true, out parsedEnum);
                }

                return parsedEnum;
            });
        }

        private static string Capitalize(string input)
        {
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}