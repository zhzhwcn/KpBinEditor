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
        
        public MainViewModel(TextViewModel textViewModel)
        {
            _textViewModel = textViewModel;
            while (string.IsNullOrEmpty(Settings.Default.ResMetaDrFile) || string.IsNullOrEmpty(Settings.Default.BinFilesDir))
            {
                (new SettingsWindow()).ShowDialog();
            }

            _textViewModel.Init(Path.Combine(Settings.Default.BinFilesDir, "TextConfig.bin"));

            var xml = new XmlDocument();
            xml.Load(Settings.Default.ResMetaDrFile);
            var nodes = xml.SelectNodes("/metalib/struct");
            var structs = new Dictionary<string, string>();
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    var name = node.Attributes?["name"]?.Value;
                    if (!string.IsNullOrEmpty(name))
                    {
                        var desc = node.Attributes?["desc"]?.Value ?? name;
                        structs[name] = desc;
                    }
                }
            }

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

        public ICommand OpenTextWindowCommand => new RelayCommand(OpenTextWindow);
        public ICommand OpenSettingsWindowCommand => new RelayCommand(OpenSettingsWindow);

        private void OpenTextWindow()
        {
            new TextWindow().Show();
        }

        private void OpenSettingsWindow()
        {
            new SettingsWindow().Show();
        }
    }
}
