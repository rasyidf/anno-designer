using AnnoDesigner.Core.Helper;
using AnnoDesigner.Core.Presets.Models;
using NLog;
using System;
using System.Linq;

namespace AnnoDesigner.Core.Presets.Loader;

public class ColorPresetsLoader
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public ColorPresets Load(string pathToColorPresetsFile)
    {
        ColorPresets result;
        try
        {
            result = SerializationHelper.LoadFromFile<ColorPresets>(pathToColorPresetsFile);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error loading the colors.");
            throw;
        }

        return result;
    }

    public ColorScheme LoadDefaultScheme(string colorPresetsFilePath)
    {
        ColorScheme result = null;

        try
        {
            ColorPresets colorPresets = Load(colorPresetsFilePath);

            result = colorPresets?.AvailableSchemes.FirstOrDefault(x => x.Name.Equals("Default", StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error loading the default scheme.");
            throw;
        }

        return result;
    }
}
