using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using KpBinEditor.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace KpBinEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var sc = new ServiceCollection();
            sc.AddSingleton<MainViewModel>();
            sc.AddSingleton<SettingsViewModel>();
            sc.AddSingleton<TextViewModel>();
            sc.AddSingleton<ResMeta>();
            sc.AddTransient<BinViewModel>();
            Ioc.Default.ConfigureServices(sc.BuildServiceProvider());

            
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
            App.Current.Shutdown();
        }
    }

}
