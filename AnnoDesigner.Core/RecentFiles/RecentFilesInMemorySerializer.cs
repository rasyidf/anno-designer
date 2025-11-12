using AnnoDesigner.Core.Models;
using System.Collections.Generic;

namespace AnnoDesigner.Core.RecentFiles;

public class RecentFilesInMemorySerializer : IRecentFilesSerializer
{
    private List<RecentFile> _recentFiles;

    public RecentFilesInMemorySerializer()
    {
        _recentFiles = [];
    }

    public List<RecentFile> Deserialize()
    {
        return _recentFiles;
    }

    public void Serialize(List<RecentFile> recentFiles)
    {
        _recentFiles = [.. recentFiles];
    }
}
