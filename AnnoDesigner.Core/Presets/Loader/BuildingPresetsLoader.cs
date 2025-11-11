using AnnoDesigner.Core.Helper;
using AnnoDesigner.Core.Presets.Models;
using NLog;
using System;

namespace AnnoDesigner.Core.Presets.Loader;

public class BuildingPresetsLoader
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public BuildingPresets Load(string pathToBuildingPresetsFile)
    {
        BuildingPresets result;
        try
        {
            result = SerializationHelper.LoadFromFile<BuildingPresets>(pathToBuildingPresetsFile);
            if (result != null)
            {
                logger.Debug($"Loaded building presets version: {result.Version}");
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error loading the buildings.");
            throw;
        }

        return result;
    }
}
