using System.IO;

namespace AnnoDesigner.Core.Layout.Models;

public interface ILayoutLoader
{
    void SaveLayout(LayoutFile layout, string pathToLayoutFile);

    void SaveLayout(LayoutFile layout, Stream streamWithLayout);

    LayoutFile LoadLayout(string pathToLayoutFile, bool forceLoad = false);

    LayoutFile LoadLayout(Stream streamWithLayout, bool forceLoad = false);
}
