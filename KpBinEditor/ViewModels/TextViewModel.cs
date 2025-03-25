using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpBinEditor.ViewModels
{
    internal class TextViewModel
    {
        private readonly Dictionary<int, string> _texts = new();

        public IReadOnlyDictionary<int, string> Texts => _texts;

        public void Init(string textBinFile)
        {
            if (!File.Exists(textBinFile))
            {
                throw new Exception("未找到TextConfig.bin");
            }

            using var fs = new FileStream(textBinFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new BinaryReader(fs);
            fs.Seek(20, SeekOrigin.Begin);
            while (fs.Position < fs.Length)
            {
                var id = BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));
                var argNum = BinaryPrimitives.ReadInt16BigEndian(reader.ReadBytes(2));
                var len = BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));
                var strBytes = reader.ReadBytes(len);
                var str = Encoding.UTF8.GetString(strBytes);
                _texts[id] = str;
            }
            
        }
    }
}
