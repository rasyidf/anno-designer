using AnnoDesigner.Core.Helper;
using AnnoDesigner.Core.Presets.Models;
using NLog;
using System;
using System.Diagnostics;
using System.IO.Abstractions;

namespace AnnoDesigner.Core.Presets.Loader;

public class TreeLocalizationLoader : ITreeLocalizationLoader
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly IFileSystem _fileSystem;

    public TreeLocalizationLoader(IFileSystem fileSystemToUse)
    {
        _fileSystem = fileSystemToUse;
    }

    ///<inheritdoc/>
    public TreeLocalizationContainer LoadFromFile(string pathToTreeLocalizationFile)
    {
        if (string.IsNullOrWhiteSpace(pathToTreeLocalizationFile))
        {
            throw new ArgumentNullException(nameof(pathToTreeLocalizationFile));
        }

        string fileContents = _fileSystem.File.ReadAllText(pathToTreeLocalizationFile);

        return Load(fileContents);
    }

    ///<inheritdoc/>
    public TreeLocalizationContainer Load(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            throw new ArgumentNullException(nameof(jsonString));
        }

        TreeLocalizationContainer result;
        try
        {
            Stopwatch sw = Stopwatch.StartNew();

            result = SerializationHelper.LoadFromJsonString<TreeLocalizationContainer>(jsonString);

            sw.Stop();
            _logger.Trace($"loading tree localization took: {sw.ElapsedMilliseconds}ms");

            if (result != null)
            {
                _logger.Info($"Loaded tree localization version: {result.Version}");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error loading tree localization.");
            throw;
        }

        return result;
    }
}
