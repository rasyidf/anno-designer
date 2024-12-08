using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using AnnoDesigner.Core.Models;
using PresetParser.Anno1404_Anno2070.Models;
using PresetParser.Models;

namespace PresetParser.Anno1404_Anno2070
{
    public class LocalizationHelper
    {
        private static readonly string[] localizationFiles;
        private readonly IFileSystem _fileSystem;

        static LocalizationHelper()
        {
            localizationFiles = new string[] { "icons.txt", "guids.txt", "addon/addontexte.txt", "dlc01.txt", "dlc02.txt", "dlc03.txt", "dlc04.txt", "addon/texts.txt" };
        }

        public LocalizationHelper(IFileSystem fileSystemToUse)
        {
            _fileSystem = fileSystemToUse ?? throw new ArgumentNullException(nameof(fileSystemToUse));
        }

        public Dictionary<string, SerializableDictionary<string>> GetLocalization(string annoVersion,
            bool addPrefix,
            Dictionary<string, Dictionary<string, PathRef[]>> versionSpecificPaths,
            string[] languages,
            string basePath)
        {
            if (languages == null)
            {
                throw new ArgumentNullException(nameof(languages));
            }

            return versionSpecificPaths == null
                ? throw new ArgumentNullException(nameof(versionSpecificPaths))
                : annoVersion is not Constants.ANNO_VERSION_1404 and
                not Constants.ANNO_VERSION_2070
                ? null
                : !versionSpecificPaths.ContainsKey(annoVersion)
                ? throw new ArgumentException($"{nameof(annoVersion)} was not found in path list.", nameof(versionSpecificPaths))
                : GetLocalizationsforAnno1404AndAnno2070(annoVersion, addPrefix, versionSpecificPaths, languages, basePath);
        }

        private Dictionary<string, SerializableDictionary<string>> GetLocalizationsforAnno1404AndAnno2070(string annoVersion,
            bool addPrefix,
            Dictionary<string, Dictionary<string, PathRef[]>> versionSpecificPaths,
            string[] languages,
            string basePathToUse)
        {
            var localizations = new Dictionary<string, SerializableDictionary<string>>();

            var references = new List<GuidRef>();
            foreach (var language in languages)
            {
                foreach (var p in versionSpecificPaths[annoVersion]["localisation"])
                {
                    var basePath = _fileSystem.Path.Combine(basePathToUse, p.Path, language, "txt");
                    foreach (var path in localizationFiles.Select(_ => _fileSystem.Path.Combine(basePath, _)))
                    {
                        if (!_fileSystem.File.Exists(path))
                        {
                            continue;
                        }

                        foreach (var curLine in _fileSystem.File.ReadLines(path))
                        {
                            // skip commentary and empty lines
                            if (string.IsNullOrWhiteSpace(curLine) || curLine.StartsWith("#"))
                            {
                                continue;
                            }

                            // split lines and skip invalid results
                            var separatorIndex = curLine.IndexOf('=');
                            if (separatorIndex == -1)
                            {
                                continue;
                            }

                            //get values
                            var guid = curLine[..separatorIndex];
                            var translation = curLine[(separatorIndex + 1)..];

                            // add new entry if needed
                            if (!localizations.TryGetValue(guid, out var value))
                            {
                                value = new SerializableDictionary<string>();
                                localizations.Add(guid, value);
                            }

                            // add localization string
                            // Translation of GUID 10239 (Anno 2070) is needed, else it will be named as Metal Converter, witch it is not.
                            if (annoVersion == Constants.ANNO_VERSION_2070 && guid == "10239" && !addPrefix)
                            {
                                switch (language)
                                {
                                    case "eng": translation = "Black Smoker"; break;
                                    case "ger": translation = "Black Smoker"; break;
                                    case "fra": translation = "Convertisseur de métal"; break;
                                    case "pol": translation = "Komin hydrotermalny"; break;
                                    case "rus": translation = "Черный курильщик"; break;
                                    case "esp": translation = "Fumador Negro"; break;
                                    default: break;
                                }
                            }
                            //Extra handling for Icon.json. Add an extra number for icon selection.
                            else if (annoVersion == Constants.ANNO_VERSION_1404 && addPrefix)
                            {
                                translation = "A4_" + translation;
                            }
                            else if (annoVersion == Constants.ANNO_VERSION_2070 && addPrefix)
                            {
                                translation = "A5_" + translation;
                            }

                            value[language] = translation;

                            // remember entry if guid is a reference to another guid
                            if (translation.StartsWith("[GUIDNAME"))
                            {
                                references.Add(new GuidRef
                                {
                                    Language = language,
                                    Guid = guid,
                                    GuidReference = translation[10..^1]
                                });
                            }
                            else if (translation.StartsWith("A4_[GUIDNAME"))
                            {
                                references.Add(new GuidRef
                                {
                                    Language = language,
                                    Guid = guid,
                                    GuidReference = "A4_" + translation[13..^1]
                                });
                            }
                            else if (translation.StartsWith("A5_[GUIDNAME"))
                            {
                                references.Add(new GuidRef
                                {
                                    Language = language,
                                    Guid = guid,
                                    GuidReference = "A5_" + translation[13..^1]
                                });
                            }
                        }
                    }
                }
            }

            // copy over references
            foreach (var reference in references)
            {
                if (localizations.TryGetValue(reference.GuidReference, out var value))
                {
                    localizations[reference.Guid][reference.Language] = value[reference.Language];
                }
                else
                {
                    _ = localizations.Remove(reference.Guid);
                }
            }

            return localizations;
        }
    }
}
