using AnnoDesigner.Core.Layout;
using AnnoDesigner.Core.Layout.Helper;
using AnnoDesigner.Core.Layout.Models;
using AnnoDesigner.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using Xunit;

namespace AnnoDesigner.Core.Tests
{
    public class StatisticsCalculationHelperTests
    {
        private readonly IFileSystem _fileSystem;
        #region testdata

        private static readonly string testData_layout_with_blocking_tiles;
        #endregion
        static StatisticsCalculationHelperTests()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            testData_layout_with_blocking_tiles = _fileSystem.File.ReadAllText(Path.Combine(basePath, "Testdata", "StatisticsCalculation", "layout_with_block_tiles.ad"), Encoding.UTF8);
            _fileSystem = new FileSystem();
        }
        private StatisticsCalculationHelper GetHelper()
        {
            return new StatisticsCalculationHelper();
        }

        #region CalculateStatistics tests

        [Fact]
        public void CalculateStatistics_IsCalledWithNull_ShouldNotThrowAndReturnNull()
        {
            // Arrange            
            StatisticsCalculationHelper helper = GetHelper();

            StatisticsCalculationResult result = StatisticsCalculationResult.Empty;

            // Act
            Exception ex = Record.Exception(() => result = helper.CalculateStatistics(null));

            // Assert
            Assert.Null(ex);
            Assert.Null(result);
        }

        [Fact]
        public void CalculateStatistics_IsCalledWithEmptyList_ShouldNotThrowAndNotReturnNull()
        {
            // Arrange            
            StatisticsCalculationHelper helper = GetHelper();

            StatisticsCalculationResult result = StatisticsCalculationResult.Empty;

            // Act
            Exception ex = Record.Exception(() => result = helper.CalculateStatistics([]));

            // Assert
            Assert.Null(ex);
            Assert.NotNull(result);
        }

        [Fact]
        public void CalculateStatistics_IsCalledWithOnlyBlockTiles_ShouldReturnEmptyResult()
        {
            // Arrange            
            StatisticsCalculationHelper helper = GetHelper();

            List<AnnoObject> objects =
            [
                new() {
                    Template = "Blocker"
                }
            ];

            // Act
            StatisticsCalculationResult result = helper.CalculateStatistics(objects);

            // Assert
            Assert.Equal(StatisticsCalculationResult.Empty, result);
        }

        [Fact]
        public void CalculateStatistics_IsCalledWithAlsoBlockTiles_ShouldIgnoreBlockTilesInCalculation()
        {
            // Arrange            
            StatisticsCalculationHelper helper = GetHelper();

            StatisticsCalculationResult expected = new(minX: 42, minY: 42, maxX: 45, maxY: 45, usedAreaWidth: 3, usedAreaHeight: 3, usedTiles: 9, minTiles: 9, efficiency: 100);

            List<AnnoObject> objects =
            [
                new() {
                    Template = "Blocker"
                },
                new() {
                    Template = "Dummy",
                    Position = new System.Windows.Point(42,42),
                    Size = new System.Windows.Size(3,3),
                    Road = false
                }
            ];

            // Act
            StatisticsCalculationResult result = helper.CalculateStatistics(objects);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateStatistics_IsCalledWithKnownLayout_IgnoreRoadsAndBlockTiles_ShouldReturnCorrectResult()
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();
            using MemoryStream streamWithLayout = new(Encoding.UTF8.GetBytes(testData_layout_with_blocking_tiles));
            LayoutFile loadedLayout = loader.LoadLayout(streamWithLayout, true);

            StatisticsCalculationHelper helper = GetHelper();

            StatisticsCalculationResult expected = new(minX: 3, minY: 3, maxX: 36, maxY: 31, usedAreaWidth: 33, usedAreaHeight: 28, usedTiles: 924, minTiles: 736, efficiency: 80);

            // Act
            StatisticsCalculationResult result = helper.CalculateStatistics(loadedLayout.Objects);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateStatistics_IsCalledWithKnownLayout_IgnoreRoadsButNotBlockTiles_ShouldReturnCorrectResult()
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();
            using MemoryStream streamWithLayout = new(Encoding.UTF8.GetBytes(testData_layout_with_blocking_tiles));
            LayoutFile loadedLayout = loader.LoadLayout(streamWithLayout, true);

            StatisticsCalculationHelper helper = GetHelper();

            StatisticsCalculationResult expected = new(minX: 1, minY: 1, maxX: 38, maxY: 33, usedAreaWidth: 37, usedAreaHeight: 32, usedTiles: 1184, minTiles: 870, efficiency: 73);

            // Act
            StatisticsCalculationResult result = helper.CalculateStatistics(loadedLayout.Objects, includeIgnoredObjects: true);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateStatistics_IsCalledWithKnownLayout_IgnoreBlockTilesButNotRoads_ShouldReturnCorrectResult()
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();
            using MemoryStream streamWithLayout = new(Encoding.UTF8.GetBytes(testData_layout_with_blocking_tiles));
            LayoutFile loadedLayout = loader.LoadLayout(streamWithLayout, true);

            StatisticsCalculationHelper helper = GetHelper();

            StatisticsCalculationResult expected = new(minX: 3, minY: 3, maxX: 36, maxY: 31, usedAreaWidth: 33, usedAreaHeight: 28, usedTiles: 924, minTiles: 923, efficiency: 100);

            // Act
            StatisticsCalculationResult result = helper.CalculateStatistics(loadedLayout.Objects, includeRoads: true);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateStatistics_IsCalledWithKnownLayout_IgnoreNothing_ShouldReturnCorrectResult()
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();
            using MemoryStream streamWithLayout = new(Encoding.UTF8.GetBytes(testData_layout_with_blocking_tiles));
            LayoutFile loadedLayout = loader.LoadLayout(streamWithLayout, true);

            StatisticsCalculationHelper helper = GetHelper();

            StatisticsCalculationResult expected = new(minX: 1, minY: 1, maxX: 38, maxY: 33, usedAreaWidth: 37, usedAreaHeight: 32, usedTiles: 1184, minTiles: 1057, efficiency: 89);

            // Act
            StatisticsCalculationResult result = helper.CalculateStatistics(loadedLayout.Objects, includeRoads: true, includeIgnoredObjects: true);

            // Assert
            Assert.Equal(expected, result);
        }

        #endregion
    }
}
