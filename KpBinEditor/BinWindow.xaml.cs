using System;
using System.Collections.Generic;
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
using KpBinEditor.ViewModels;

namespace KpBinEditor
{
    /// <summary>
    /// BinWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BinWindow : Window
    {
        private readonly BinViewModel _vm;

        public BinWindow(string binFile)
        {
            _vm = Ioc.Default.GetRequiredService<BinViewModel>();
            _vm.Init(binFile);
            DataContext = _vm;
            InitializeComponent();
        }
    }
}
