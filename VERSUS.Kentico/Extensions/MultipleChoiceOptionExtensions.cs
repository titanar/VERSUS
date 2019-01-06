using System;
using System.Linq;

using KenticoCloud.Delivery;

namespace VERSUS.Kentico.Extensions
{
    public static class MultipleChoiceOptionExtensions
    {
        public static T ToEnum<T>(this MultipleChoiceOption multipleChoiceOption) where T : struct, Enum
        {
            if (!Enum.TryParse(Capitalize(multipleChoiceOption.Codename), out T parsedEnum))
            {
                //If simple capitalization failed, try ignoring case.
                Enum.TryParse(multipleChoiceOption.Codename, true, out parsedEnum);
            }

            Enum.TryParse(multipleChoiceOption.Codename, true, out parsedEnum);

            return parsedEnum;
        }

        private static string Capitalize(string input)
        {
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}