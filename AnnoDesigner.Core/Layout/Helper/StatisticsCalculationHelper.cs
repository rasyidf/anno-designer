using AnnoDesigner.Core.Extensions;
using AnnoDesigner.Core.Layout.Models;
using AnnoDesigner.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AnnoDesigner.Core.Layout.Helper;

public class StatisticsCalculationHelper
{
    /// <summary>
    /// Calculates various statistics for a given collection of <see cref="AnnoObject"/>.
    /// </summary>
    /// <param name="objects">The collection to calculate the statistics for.</param>
    /// <param name="includeRoads">Should roads be included in calculation?</param>
    /// <param name="includeIgnoredObjects">Should ignored objects be included in calculation?</param>
    /// <returns>A <see cref="StatisticsCalculationResult"/> with all calculated statistics.</returns>
    public StatisticsCalculationResult CalculateStatistics(IEnumerable<AnnoObject> objects, bool includeRoads = false, bool includeIgnoredObjects = false)
    {
        if (objects == null)
        {
            return null;
        }

        IEnumerable<AnnoObject> localObjects = includeIgnoredObjects ? objects : objects.WithoutIgnoredObjects();

        if (localObjects.Count() == 0)
        {
            return StatisticsCalculationResult.Empty;
        }

        /* old logic is easier to understand, but slower
         // calculate bouding box
         var boxX = placedObjects.Max(_ => _.Position.X + _.Size.Width) - placedObjects.Min(_ => _.Position.X);
         var boxY = placedObjects.Max(_ => _.Position.Y + _.Size.Height) - placedObjects.Min(_ => _.Position.Y);
         // calculate area of all buildings
         var minTiles = placedObjects.Where(_ => !_.Road).Sum(_ => _.Size.Width * _.Size.Height);
        */

        double maxX = double.MinValue;
        double maxY = double.MinValue;
        double minX = double.MaxValue;
        double minY = double.MaxValue;
        double sum = 0d;
        foreach (AnnoObject curObject in localObjects)
        {
            double curPosX = curObject.Position.X;
            double curPosY = curObject.Position.Y;
            Size curSize = curObject.Size;

            double curMaxX = curPosX + curSize.Width;
            double curMaxY = curPosY + curSize.Height;

            if (curMaxX > maxX)
            {
                maxX = curMaxX;
            }

            if (curPosX < minX)
            {
                minX = curPosX;
            }

            if (curMaxY > maxY)
            {
                maxY = curMaxY;
            }

            if (curPosY < minY)
            {
                minY = curPosY;
            }

            if (includeRoads || !curObject.Road)
            {
                sum += curSize.Width * curSize.Height;
            }
        }

        // calculate bouding box
        double boxX = maxX - minX;
        double boxY = maxY - minY;
        // calculate area of all buildings
        double minTiles = sum;

        double usedTiles = boxX * boxY;
        double efficiency = Math.Round(minTiles / boxX / boxY * 100);

        return new StatisticsCalculationResult(minX, minY, maxX, maxY, boxX, boxY, usedTiles, minTiles, efficiency);
    }
}
