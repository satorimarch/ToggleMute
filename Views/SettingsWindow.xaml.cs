﻿using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ToggleMute.ViewModels;

namespace ToggleMute.Views;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        DataContext = App.Current.ServiceProvider.GetRequiredService<SettingsViewModel>();
    }
}