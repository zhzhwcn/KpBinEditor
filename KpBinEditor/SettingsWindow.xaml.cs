using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.DependencyInjection;
using KpBinEditor.Properties;
using KpBinEditor.ViewModels;

namespace KpBinEditor
{
    /// <summary>
    /// SettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            DataContext = Ioc.Default.GetRequiredService<SettingsViewModel>();
        }

        private SettingsViewModel _vm => (SettingsViewModel)DataContext;


        private void SettingsWindows_OnClosing(object? sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(Settings.Default.ResMetaDrFile) ||
                string.IsNullOrEmpty(Settings.Default.BinFilesDir))
            {
                MessageBox.Show("配置不能为空");
                e.Cancel = true;
            }
        }
    }
}
