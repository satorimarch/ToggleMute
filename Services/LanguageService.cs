using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace ToggleMute.Services;

public interface ILanguageService
{
    public void ChangeLanguage(string language);

    public string GetText(string key);
}

public class LanguageService : ILanguageService
{
    public void ChangeLanguage(string language)
    {
        var cultureInfo = new CultureInfo(language);
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        Thread.CurrentThread.CurrentCulture = cultureInfo;

        var dictionary = App.Current.Resources.MergedDictionaries.First(dict =>
            dict.Source.OriginalString.Contains("Languages"));

        if (dictionary != null) App.Current.Resources.MergedDictionaries.Remove(dictionary);

        App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
        {
            Source = new Uri($"Languages/Lang.{language}.xaml", UriKind.Relative)
        });
    }

    public string GetText(string key)
    {
        return App.Current.FindResource(key) as string ?? key;
    }
}