using Imagin.Core.Analytics;
using Imagin.Core.Local;
using Imagin.Core.Local.Engine;
using System.Globalization;

namespace Imagin.Core.Linq
{
    public static class XLanguage
    {
        public static void Set(this Language language)
        {
            Try.Invoke(() =>
            {
                LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
                LocalizeDictionary.Instance.Culture = new CultureInfo(language.GetAttribute<CultureAttribute>().Code);
            },
            e => Log.Write<Language>(e));
        }
    }
}