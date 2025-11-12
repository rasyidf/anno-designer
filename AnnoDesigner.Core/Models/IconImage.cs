using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace AnnoDesigner.Core.Models;

[DebuggerDisplay("{" + nameof(Name) + "}")]
public class IconImage
{

    #region ctor

    public IconImage(string name)
    {
        Name = name;
        Localizations = null;
    }

    public IconImage(string name, Dictionary<string, string> localizations, string iconPath) : this(name)
    {
        Localizations = localizations;
        IconPath = iconPath;
    }

    #endregion        

    public string Name { get; }

    public Dictionary<string, string> Localizations { get; set; }

    public string DisplayName => NameForLanguage("eng");

    public string NameForLanguage(string languageCode)
    {
        return Localizations is null || !Localizations.TryGetValue(languageCode, out string translation) ? Name : translation;
    }

    public BitmapImage Icon
    {
        get
        {
            if (field == null && !string.IsNullOrWhiteSpace(IconPath))
            {
                field = new BitmapImage(new Uri(IconPath));
                if (field.CanFreeze)
                {
                    field.Freeze();
                }
            }

            return field;
        }
    }

    public string IconPath { get; }
}