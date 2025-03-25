using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using KpBinEditor.Properties;
using Microsoft.Win32;

namespace KpBinEditor.ViewModels
{
    internal class SettingsViewModel : ObservableRecipient
    {
        public SettingsViewModel()
        {
            ServerPath = Settings.Default.ServerDir;
            ResMetaDrFile = Settings.Default.ResMetaDrFile;
            BinPath = Settings.Default.BinFilesDir;
        }

		private string? _serverPath;

		public string? ServerPath
		{
			get { return _serverPath; }
            set { SetProperty(ref _serverPath, value); }
        }

		private string? _resMetaDrFile;

		public string? ResMetaDrFile
		{
			get { return _resMetaDrFile; }
			set { SetProperty(ref _resMetaDrFile, value); }
		}

		private string? _binPath;

		public string? BinPath
		{
			get { return _binPath; }
            set { SetProperty(ref _binPath, value); }
        }

        public ICommand SelectServerPathCommand => new RelayCommand(SelectServerPath);
        public ICommand SelectResMetaDrFileCommand => new RelayCommand(SelectResMetaDrFile);
        public ICommand SelectBinPathCommand => new RelayCommand(SelectBinPath);
        public ICommand SaveCommand => new RelayCommand(Save);

        private void Save()
        {
            Settings.Default.ServerDir = ServerPath;
            Settings.Default.ResMetaDrFile = ResMetaDrFile;
            Settings.Default.BinFilesDir = BinPath;
            Settings.Default.Save();
        }

        private void SelectServerPath()
        {
            var dlg = new OpenFolderDialog();
            if (dlg.ShowDialog() == true)
            {
                ServerPath = dlg.FolderName;
                Settings.Default.ServerDir = ServerPath;
                var drFile = Path.Combine(ServerPath, "resource\\dr\\ResMeta.dr");
                if (File.Exists(drFile))
                {
                    ResMetaDrFile = drFile;
                    Settings.Default.ResMetaDrFile = ResMetaDrFile;
                }

                var textBinFile = Path.Combine(ServerPath, "runenv\\zone_svr\\cfg\\res\\TextConfig.bin");
                if (File.Exists(textBinFile))
                {
                    BinPath = Path.GetDirectoryName(textBinFile);
                    Settings.Default.BinFilesDir = BinPath;
                }

                Settings.Default.Save();
            }
        }

        private void SelectResMetaDrFile()
        {
            var dlg = new OpenFileDialog()
            {
                FileName = "ResMeta.dr",
                Filter = "ResMeta File|*.dr"
            };

            if (dlg.ShowDialog() == true)
            {
                ResMetaDrFile = dlg.FileName;
                Settings.Default.ResMetaDrFile = ResMetaDrFile;
                Settings.Default.Save();
            }
        }

        private void SelectBinPath()
        {
            var dlg = new OpenFolderDialog();
            if (dlg.ShowDialog() == true)
            {
                var textBinFile = Path.Combine(dlg.FolderName, "TextConfig.bin");
                if (File.Exists(textBinFile))
                {
                    BinPath = dlg.FolderName;
                    Settings.Default.BinFilesDir = BinPath;

                    Settings.Default.Save();
                }
                else
                {
                    MessageBox.Show("No TextConfig.bin");
                }
            }
        }
    }
}
