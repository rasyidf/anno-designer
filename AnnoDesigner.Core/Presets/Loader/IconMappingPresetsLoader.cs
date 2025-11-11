using AnnoDesigner.Core.Helper;
using AnnoDesigner.Core.Presets.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace AnnoDesigner.Core.Presets.Loader;

/// <summary>
/// This encapsulates the logic of loading different versions of the icons.json file.
/// </summary>
public class IconMappingPresetsLoader
{
    private readonly IFileSystem _fileSystem;
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
    public IconMappingPresetsLoader()
    {
        _fileSystem = new FileSystem();
    }

    public IconMappingPresets LoadFromFile(string pathToIconNameMappingFile)
    {
        if (string.IsNullOrWhiteSpace(pathToIconNameMappingFile))
        {
            throw new ArgumentNullException(nameof(pathToIconNameMappingFile));
        }
        string fileContents = _fileSystem.File.ReadAllText(pathToIconNameMappingFile);
        return Load(fileContents);
    }

    public IconMappingPresets Load(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            throw new ArgumentNullException(nameof(jsonString));
        }
        IconMappingPresets result = new();
        try
        {
            result = SerializationHelper.LoadFromJsonString<IconMappingPresets>(jsonString);
        }
        catch (Newtonsoft.Json.JsonSerializationException)
        {
            //failed deserialization = old version of file without version info
            List<IconNameMap> oldIconMapping = SerializationHelper.LoadFromJsonString<List<IconNameMap>>(jsonString);
            result.IconNameMappings = oldIconMapping;
            result.Version = string.Empty;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error loading the icon name mapping file.");
            throw;
        }

        return result;
    }
}
