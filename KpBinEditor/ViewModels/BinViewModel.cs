using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KpBinEditor.Properties;
using System.Buffers.Binary;
using System.Reflection.PortableExecutable;

namespace KpBinEditor.ViewModels
{
    internal class BinViewModel : ObservableRecipient
    {
        private readonly TextViewModel _textViewModel;
        private readonly ResMeta _resMeta;

        public BinViewModel(TextViewModel textViewModel, ResMeta resMeta)
        {
            _textViewModel = textViewModel;
            _resMeta = resMeta;
        }

        public DataTable Data { get; } = new DataTable();

        public void Init(string binFile)
        {
            var name = binFile;
            if (!binFile.EndsWith(".bin"))
            {
                binFile += ".bin";
            }
            else
            {
                name = binFile.Replace(".bin", "");
            }
            var file = Path.Combine(Settings.Default.BinFilesDir, binFile);
            if (!File.Exists(file))
            {
                MessageBox.Show($"未找到{binFile}");
                return;
            }

            var sd = _resMeta.Structs.FirstOrDefault(s => s.Name == name);
            if (sd == null)
            {
                MessageBox.Show($"未找到数据定义{binFile}");
                return;
            }

            AddColumns(sd.Entries, string.Empty);

            

            using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new BinaryReader(fs);
            fs.Seek(20, SeekOrigin.Begin);
            while (fs.Position < fs.Length)
            {
                var row = Data.NewRow();
                ReadEntries(sd.Entries, row, reader, string.Empty);
                Data.Rows.Add(row);
            }

            OnPropertyChanged(nameof(Data));
        }

        private static readonly Dictionary<string, Type> BaseTypes = new()
        {
            { "float", typeof(float) },
            { "byte", typeof(byte) },
            { "int", typeof(int) },
            { "uint", typeof(uint) },
            { "smallint", typeof(short) },
            { "string", typeof(string) }
        };

        private void AddColumns(List<StructEntry> entries, string colPrefix)
        {
            foreach (var structEntry in entries)
            {
                if (structEntry.Count > 1)
                {
                    for (int i = 0; i < structEntry.Count; i++)
                    {
                        var colName = $"{colPrefix}{structEntry.Name}_{i}";

                        if (BaseTypes.TryGetValue(structEntry.Type, out var type))
                        {
                            Data.Columns.Add(new DataColumn(colName, type));
                        }
                        else
                        {
                            var nestStruct = _resMeta.Structs.FirstOrDefault(s => s.Name == structEntry.Type);
                            if (nestStruct != null)
                            {
                                AddColumns(nestStruct.Entries, $"{colName}_");
                            }
                            else
                            {
                                MessageBox.Show($"未知数据类型：{structEntry.Type}");
                                return;
                            }
                        }
                    }
                }
                else
                {
                    var colName = $"{colPrefix}{structEntry.Name}";

                    if (BaseTypes.TryGetValue(structEntry.Type, out var type))
                    {
                        Data.Columns.Add(new DataColumn(colName, type));
                    }
                    else
                    {
                        var nestStruct = _resMeta.Structs.FirstOrDefault(s => s.Name == structEntry.Type);
                        if (nestStruct != null)
                        {
                            AddColumns(nestStruct.Entries, $"{colName}_");
                        }
                        else
                        {
                            MessageBox.Show($"未知数据类型：{structEntry.Type}");
                            return;
                        }
                    }
                }
                
            }
        }

        private void ReadEntries(List<StructEntry> entries, DataRow row, BinaryReader reader, string colPrefix)
        {
            foreach (var structEntry in entries)
            {
                if (structEntry.Count > 1)
                {
                    for (int i = 0; i < structEntry.Count; i++)
                    {
                        var colName = $"{colPrefix}{structEntry.Name}_{i}";
                        switch (structEntry.Type)
                        {
                            case "float":
                                row[colName] = BinaryPrimitives.ReadSingleBigEndian(reader.ReadBytes(4));
                                break;
                            case "byte":
                                row[colName] = reader.ReadByte();
                                break;
                            case "int":
                                row[colName] = BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));
                                break;
                            case "uint":
                                row[colName] = BinaryPrimitives.ReadUInt32BigEndian(reader.ReadBytes(4));
                                break;
                            case "smallint":
                                row[colName] = BinaryPrimitives.ReadUInt16BigEndian(reader.ReadBytes(2));
                                break;
                            case "string":
                                var len = BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));
                                var strBytes = reader.ReadBytes(len);
                                row[colName] = Encoding.UTF8.GetString(strBytes);
                                break;
                            default:
                                var nestStruct = _resMeta.Structs.FirstOrDefault(s => s.Name == structEntry.Type);
                                if (nestStruct != null)
                                {
                                    ReadEntries(nestStruct.Entries, row, reader, $"{colName}_");
                                    break;
                                }
                                else
                                {
                                    MessageBox.Show($"未知数据类型：{structEntry.Type}");
                                    return;
                                }
                        }
                    }
                }
                else
                {
                    var colName = $"{colPrefix}{structEntry.Name}";
                    switch (structEntry.Type)
                    {
                        case "float":
                            row[colName] = BinaryPrimitives.ReadSingleBigEndian(reader.ReadBytes(4));
                            break;
                        case "byte":
                            row[colName] = reader.ReadByte();
                            break;
                        case "int":
                            row[colName] = BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));
                            break;
                        case "uint":
                            row[colName] = BinaryPrimitives.ReadUInt32BigEndian(reader.ReadBytes(4));
                            break;
                        case "smallint":
                            row[colName] = BinaryPrimitives.ReadUInt16BigEndian(reader.ReadBytes(2));
                            break;
                        case "string":
                            var len = BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));
                            var strBytes = reader.ReadBytes(len);
                            row[colName] = Encoding.UTF8.GetString(strBytes);
                            break;
                        default:
                            var nestStruct = _resMeta.Structs.FirstOrDefault(s => s.Name == structEntry.Type);
                            if (nestStruct != null)
                            {
                                ReadEntries(nestStruct.Entries, row, reader, $"{colName}_");
                                break;
                            }
                            else
                            {
                                MessageBox.Show($"未知数据类型：{structEntry.Type}");
                                return;
                            }
                    }
                }
            }
        }
    }
}
