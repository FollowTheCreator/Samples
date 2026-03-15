using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using Kernel.Globalization.Extensions;

namespace IRT.Modules.DataTransfer.Generic.Helpers.Infrastucture
{
    public static class ResourcesHelper
    {
        public static Dictionary<string, string> GetResourceManagerDictionary(
            this ResourceManager resourceManager,
            CultureInfo cultureInfo,
            bool createIfNotExists = false,
            bool tryParents = false)
        {
            var resourceSetDictionary = resourceManager
                .GetResourceSet(
                    cultureInfo,
                    createIfNotExists,
                    tryParents)
                .ToDictionary(x => x.Key.ToString(), x => x.Value.ToString());
            return resourceSetDictionary;
        }

        public static List<string> GetResourceManagerKeys(
            this ResourceManager resourceManager,
            CultureInfo cultureInfo,
            bool createIfNotExists = false,
            bool tryParents = false)
        {
            var result = new List<string>();

            var resourceSet = resourceManager
                .GetResourceSet(
                    cultureInfo,
                    createIfNotExists,
                    tryParents);

            foreach (DictionaryEntry entry in resourceSet)
            {
                result.Add(entry.Key.ToString());
            }

            return result;
        }
    }
}
