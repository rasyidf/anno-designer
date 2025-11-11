using AnnoDesigner.Core.Layout.Models;
using AnnoDesigner.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnnoDesigner.Core.Services;

public class ClipboardService : IClipboardService
{
    private readonly ILayoutLoader _layoutLoader;
    private readonly IClipboard _clipboard;

    public ClipboardService(ILayoutLoader layoutLoaderToUse, IClipboard clipboardToUse)
    {
        _layoutLoader = layoutLoaderToUse;
        _clipboard = clipboardToUse;
    }

    public void Copy(IEnumerable<AnnoObject> objects)
    {
        if (objects is not null && objects.Any())
        {
            using MemoryStream memoryStream = new();
            _layoutLoader.SaveLayout(new LayoutFile(objects), memoryStream);
            _ = memoryStream.Seek(0, SeekOrigin.Begin);
            _clipboard.Clear();
            _clipboard.SetData(CoreConstants.AnnoDesignerClipboardFormat, memoryStream);
            _clipboard.Flush();
        }
    }

    public ICollection<AnnoObject> Paste()
    {
        IReadOnlyList<string> files = _clipboard.GetFileDropList();
        if (files?.Count == 1)
        {
            try
            {
                return _layoutLoader.LoadLayout(files[0], forceLoad: true).Objects;
            }
            catch (JsonReaderException) { }
        }

        if (_clipboard.ContainsData(CoreConstants.AnnoDesignerClipboardFormat))
        {
            try
            {
                Stream stream = _clipboard.GetData(CoreConstants.AnnoDesignerClipboardFormat) as Stream;
                if (stream is not null)
                {
                    return _layoutLoader.LoadLayout(stream, forceLoad: true).Objects;
                }
            }
            catch (JsonReaderException) { }
        }

        if (_clipboard.ContainsText())
        {
            using MemoryStream memoryStream = new();
            using StreamWriter streamWriter = new(memoryStream);
            streamWriter.Write(_clipboard.GetText());
            streamWriter.Flush();
            _ = memoryStream.Seek(0, SeekOrigin.Begin);
            try
            {
                return _layoutLoader.LoadLayout(memoryStream, forceLoad: true).Objects;
            }
            catch (JsonReaderException) { }
        }

        return Array.Empty<AnnoObject>();
    }
}
