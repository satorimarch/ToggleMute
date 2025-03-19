using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using Microsoft.Extensions.Logging;

namespace ToggleMute.Services;

public interface ILanguageService
{
    public void ChangeLanguage(string language);

    public string GetText(string key);

    public event Action<string>? OnLanguageChanged;
}

public class LanguageService(ILogger<LanguageService> logger) : ILanguageService
{
    public void ChangeLanguage(string language)
    {
        logger.LogInformation("Changing language to {language}", language);

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

        OnLanguageChanged?.Invoke(language);
    }

    public string GetText(string key)
    {
        return App.Current.FindResource(key) as string ?? key;
    }

    public event Action<string>? OnLanguageChanged;
}