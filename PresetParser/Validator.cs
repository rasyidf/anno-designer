using AnnoDesigner.Core.Presets.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PresetParser;

internal class Validator
{
    public (bool isValid, List<string> duplicateIdentifiers) CheckForUniqueIdentifiers(List<IBuildingInfo> buildingsToCheck, List<string> knownDuplicates)
    {
        (bool, List<string>) result = (true, []);

        knownDuplicates ??= [];

        List<string> duplicates = [.. buildingsToCheck.GroupBy(x => x.Identifier).Where(x => x.Count() > 1).Select(x => x.Key)];
        //remove known duplicates from result
        duplicates = [.. duplicates.Except(knownDuplicates, StringComparer.Ordinal)];

        if (duplicates.Count > 0)
        {
            result = (false, duplicates);
        }

        return result;
    }
}
