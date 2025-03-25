using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using CommunityToolkit.Mvvm.Input;
using KpBinEditor.Properties;

namespace KpBinEditor.ViewModels
{
    internal class MainViewModel : ObservableRecipient
    {
        private readonly TextViewModel _textViewModel;
        private readonly ResMeta _resMeta;
        
        public MainViewModel(TextViewModel textViewModel, ResMeta resMeta)
        {
            _textViewModel = textViewModel;
            _resMeta = resMeta;
            while (string.IsNullOrEmpty(Settings.Default.ResMetaDrFile) || string.IsNullOrEmpty(Settings.Default.BinFilesDir))
            {
                (new SettingsWindow()).ShowDialog();
            }

            _textViewModel.Init(Path.Combine(Settings.Default.BinFilesDir, "TextConfig.bin"));

            resMeta.Init();

            var structs = resMeta.Structs.ToDictionary(s => s.Name, s => s.Desc ?? s.Name);

            _files.Columns.Add(new DataColumn("name"));
            _files.Columns.Add(new DataColumn("desc"));

            foreach (var file in Directory.GetFiles(Settings.Default.BinFilesDir))
            {
                if (Path.GetExtension(file) == ".bin")
                {
                    var name = Path.GetFileNameWithoutExtension(file);
                    var row = _files.NewRow();
                    row["name"] = name;
                    row["desc"] = structs.GetValueOrDefault(name, name);
                    _files.Rows.Add(row);
                }
            }
            OnPropertyChanged(nameof(Files));
        }

        private readonly DataTable _files = new DataTable();
        public DataTable Files => _files;

        private DataRowView? _selectedRow;

        public DataRowView? SelectRow
        {
            get { return _selectedRow; }
            set { SetProperty(ref _selectedRow, value); }
        }


        public ICommand OpenTextWindowCommand => new RelayCommand(OpenTextWindow);
        public ICommand OpenSettingsWindowCommand => new RelayCommand(OpenSettingsWindow);
        public ICommand OpenBinWindowCommand => new RelayCommand(OpenBinWindow);

        private void OpenTextWindow()
        {
            new TextWindow().Show();
        }

        private void OpenSettingsWindow()
        {
            new SettingsWindow().Show();
        }

        private void OpenBinWindow()
        {
            if (SelectRow == null)
            {
                return;
            }

            var binFile = SelectRow["name"]?.ToString();
            if (string.IsNullOrEmpty(binFile))
            {
                return;
            }
            new BinWindow(binFile).Show();
            
        }
    }
}
