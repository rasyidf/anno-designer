using AnnoDesigner.Core.Models;
using AnnoDesigner.Core.Presets.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace AnnoDesigner.Core.Presets.Loader;

public class IconLoader
{
    private readonly IFileSystem _fileSystem;
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    public IconLoader()
    {
        _fileSystem = new FileSystem();
    }
    public Dictionary<string, IconImage> Load(string pathToIconFolder, IconMappingPresets iconNameMapping)
    {
        Dictionary<string, IconImage> result = null;

        try
        {
            result = [];

            foreach (string path in _fileSystem.Directory.EnumerateFiles(pathToIconFolder, CoreConstants.IconFolderFilter))
            {
                string filenameWithoutExt = Path.GetFileNameWithoutExtension(path);
                if (string.IsNullOrWhiteSpace(filenameWithoutExt))
                {
                    continue;
                }

                string filenameWithExt = Path.GetFileName(path);

                // try mapping to the icon translations
                Dictionary<string, string> localizations = null;
                if (iconNameMapping?.IconNameMappings != null)
                {
                    IconNameMap map = iconNameMapping.IconNameMappings.Find(x => string.Equals(x.IconFilename, filenameWithExt, StringComparison.OrdinalIgnoreCase));
                    if (map != null)
                    {
                        localizations = map.Localizations.Dict;
                    }
                }

                // add the current icon
                result.Add(filenameWithoutExt, new IconImage(filenameWithoutExt, localizations, path));
            }

            // sort icons by their DisplayName
            result = result.OrderBy(x => x.Value.DisplayName).ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);//make sure ContainsKey is caseInSensitive
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error loading the icons.");
            throw;
        }

        return result;
    }
}
