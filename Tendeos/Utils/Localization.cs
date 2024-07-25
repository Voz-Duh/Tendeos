using System.Collections.Generic;
using System.IO;
using Tendeos.Utils.SaveSystem;
using System.Text.RegularExpressions;
using Tendeos.Modding;

namespace Tendeos.Utils
{
    public static class Localization
    {
        private static readonly Dictionary<string, string> data = new();

        public static void Load(string path, string languageKey)
        {
            data.Clear();
            string fullPath = Path.Join(path, $"languages/{Settings.GetString(languageKey)}.lng");
            
            foreach (var (key, value) in MIS.Generate(fullPath).GetAllParametersAs<string>())
                data[key] = value;
        }

        private static readonly Regex translateRegex = new(@"\<(\<[^(>>)]*\>)\>|\<([^>]*)\>");

        public static string Translate(this string key) => data.TryGetValue(key, out string value) ? value : key;

        public static string Translate(string key, params object[] args) =>
            data.TryGetValue(key, out string value) ? string.Format(value, args) : key;

        public static string WithTranslates(this string text, params object[] args)
        {
            text = translateRegex.Replace(text, match =>
            {
                if (match.Groups[0].Value.StartsWith("<<")) return match.Groups[1].Value;
                string key = match.Groups[2].Value;
                return data.TryGetValue(key, out string value) ? value : key;
            });

            return string.Format(text, args);
        }
    }
}