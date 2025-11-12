using AnnoDesigner.Core.Models;
using AnnoDesigner.Core.Presets.Loader;
using AnnoDesigner.Core.Presets.Models;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace AnnoDesigner.Core.Presets.Helper;

public class ColorPresetsHelper
{
    private static IFileSystem _fileSystem = new FileSystem();
    private readonly ColorPresetsLoader _colorPresetsLoader;
    private readonly BuildingPresetsLoader _buildingPresetsLoader;

    #region ctor

    private static readonly Lazy<ColorPresetsHelper> lazy = new(() => new ColorPresetsHelper());

    public static ColorPresetsHelper Instance => lazy.Value;
    private ColorPresetsHelper()
    {
        _colorPresetsLoader = new ColorPresetsLoader();
        _buildingPresetsLoader = new BuildingPresetsLoader();
        _fileSystem = new FileSystem();
    }
    #endregion

    private ColorPresets LoadedColorPresets => field ??= _colorPresetsLoader.Load(_fileSystem.Path.Combine(_fileSystem.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), CoreConstants.PresetsFiles.ColorPresetsFile));

    private ColorScheme LoadedDefaultColorScheme => field ??= _colorPresetsLoader.LoadDefaultScheme(_fileSystem.Path.Combine(_fileSystem.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), CoreConstants.PresetsFiles.ColorPresetsFile));

    private BuildingPresets LoadedBuildingPresets => field ??= _buildingPresetsLoader.Load(_fileSystem.Path.Combine(_fileSystem.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), CoreConstants.PresetsFiles.BuildingPresetsFile));

    public string PresetsVersion => LoadedColorPresets.Version;

    public Color? GetPredefinedColor(AnnoObject annoObject)
    {
        Color? result = null;

        string templateName = annoObject.Template;

        //template name defined?
        if (string.IsNullOrWhiteSpace(annoObject.Template))
        {
            string foundTemplate = FindTemplateByIdentifier(annoObject.Identifier);
            if (string.IsNullOrWhiteSpace(foundTemplate))
            {
                return result;
            }

            templateName = foundTemplate;
            //set template so it is saved when the layout is saved again
            annoObject.Template = templateName;
        }

        //colors for template defined?
        List<PredefinedColor> colorsForTemplate = LoadedDefaultColorScheme.Colors.Where(x => x.TargetTemplate.Equals(templateName, StringComparison.OrdinalIgnoreCase)).ToList();
        if (!colorsForTemplate.Any())
        {
            return result;
        }

        //specific color for identifier defined?
        PredefinedColor colorForTemplateContainingIdentifier = colorsForTemplate.FirstOrDefault(x => x.TargetIdentifiers.Contains(annoObject.Identifier, StringComparer.OrdinalIgnoreCase));
        result = colorForTemplateContainingIdentifier != null
            ? (Color?)colorForTemplateContainingIdentifier.Color
            //specific color for template but without identifier defined?
            : colorsForTemplate.FirstOrDefault(x => x.TargetIdentifiers.Count == 0) != null
                ? (Color?)colorsForTemplate.FirstOrDefault(x => x.TargetIdentifiers.Count == 0).Color
                //use first found defined color
                : (Color?)colorsForTemplate.First().Color;

        return result;
    }

    private string FindTemplateByIdentifier(string identifier)
    {
        string result = null;

        result = LoadedBuildingPresets.Buildings.FirstOrDefault(x => x.Identifier.Equals(identifier, StringComparison.OrdinalIgnoreCase))?.Template;

        return result;
    }
    /// <summary>
    /// Allows setting a custom IFileSystem implementation for testing purposes.
    /// </summary>
    /// <param name="fileSystem">The IFileSystem implementation to use.</param>
    public static void SetFileSystem(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }
}
