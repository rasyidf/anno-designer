using System;
using System.Collections.Generic;
using AnnoDesigner.Core.Models;

namespace AnnoDesigner.Core.Tests.Mocks
{
    public class MockedClipboard : IClipboard
    {
        private readonly List<string> _files = [];
        private readonly Dictionary<string, object> _data = [];
        private string _text;

        public void AddFilesToClipboard(List<string> filesToAdd)
        {
            _files.AddRange(filesToAdd);
        }

        public void Clear()
        {
            _files.Clear();
            _data.Clear();
            _text = null;
        }

        public bool ContainsData(string format)
        {
            return _data.ContainsKey(format);
        }

        public bool ContainsText()
        {
            return _text is not null;
        }

        public void Flush()
        {
            //It is a no-op because how should it be implemented/tested?
        }

        public object GetData(string format)
        {
            return format is null ? throw new ArgumentNullException(nameof(format)) : !_data.TryGetValue(format, out var value) ? null : value;
        }

        public IReadOnlyList<string> GetFileDropList()
        {
            return _files.AsReadOnly();
        }

        public string GetText()
        {
            return string.IsNullOrEmpty(_text) ? string.Empty : _text;
        }

        public void SetText(string text)
        {
            _text = text;
        }

        public void SetData(string format, object data)
        {
            if (data is System.IO.Stream stream)
            {
                var copiedData = new System.IO.MemoryStream();
                stream.CopyTo(copiedData);
                _ = copiedData.Seek(0, System.IO.SeekOrigin.Begin);
                _data[format] = copiedData;
                return;
            }

            _data[format] = data;
        }
    }
}
