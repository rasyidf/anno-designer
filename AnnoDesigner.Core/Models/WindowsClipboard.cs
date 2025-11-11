using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace AnnoDesigner.Core.Models;

public class WindowsClipboard : IClipboard
{
    public void Clear()
    {
        Clipboard.Clear();
    }

    public bool ContainsData(string format)
    {
        return Clipboard.ContainsData(format);
    }

    public bool ContainsText()
    {
        return Clipboard.ContainsText();
    }

    public void Flush()
    {
        Clipboard.Flush();
    }

    public object GetData(string format)
    {
        return Clipboard.GetData(format);
    }

    public IReadOnlyList<string> GetFileDropList()
    {
        StringCollection clipboardData = Clipboard.GetFileDropList();
        return clipboardData is null ? null : (IReadOnlyList<string>)new ReadOnlyCollection<string>([.. clipboardData.Cast<string>()]);
    }

    public string GetText()
    {
        return Clipboard.GetText();
    }

    public void SetData(string format, object data)
    {
        Clipboard.SetData(format, data);
    }
}
