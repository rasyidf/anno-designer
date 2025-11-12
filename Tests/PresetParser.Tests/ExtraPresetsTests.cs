using PresetParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PresetParser.Tests
{
    public class ExtraPresetsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("")]
        public void GetExtraPresetsAnnoVersionIsNullOrWhiteSpaceShouldReturnEmptyList(string annoVersion)
        {
            // Arrange/Act
            IEnumerable<ExtraPreset> result = ExtraPresets.GetExtraPresets(annoVersion);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(Constants.ANNO_VERSION_1404, 4)]
        [InlineData(Constants.ANNO_VERSION_2070, 5)]
        [InlineData(Constants.ANNO_VERSION_1800, 64)]
        [InlineData(Constants.ANNO_VERSION_2205, 0)]
        public void GetExtraPresetsAnnoVersionIsKnownShouldReturnCorrectExtraPresetsCount(string annoVersion, int expectedCount)
        {
            // Arrange/Act
            List<ExtraPreset> result = ExtraPresets.GetExtraPresets(annoVersion).ToList();

            // Assert
            Assert.Equal(expectedCount, result.Count);
        }

        [Theory]
        [InlineData(Constants.ANNO_VERSION_1404)]
        [InlineData(Constants.ANNO_VERSION_2070)]
        [InlineData(Constants.ANNO_VERSION_1800)]
        [InlineData(Constants.ANNO_VERSION_2205)]
        public void GetExtraPresetsEveryElementShouldContainValuesForAllLocalizations(string annoVersion)
        {
            // Arrange/Act
            List<ExtraPreset> result = ExtraPresets.GetExtraPresets(annoVersion).ToList();

            // Assert
            Assert.All(result, x =>
            {
                Assert.False(string.IsNullOrWhiteSpace(x.LocaEng));
                Assert.False(string.IsNullOrWhiteSpace(x.LocaEsp));
                Assert.False(string.IsNullOrWhiteSpace(x.LocaFra));
                Assert.False(string.IsNullOrWhiteSpace(x.LocaGer));
                Assert.False(string.IsNullOrWhiteSpace(x.LocaPol));
                Assert.False(string.IsNullOrWhiteSpace(x.LocaRus));
            });
        }

        #region ExtraRoads tests

        [Fact]
        public void GetExtraRoadsShouldReturnCorrectCount()
        {
            // Arrange
            int expectedCount = 17;

            // Act
            List<ExtraRoads> result = ExtraPresets.GetExtraRoads().ToList();

            // Assert
            Assert.Equal(expectedCount, result.Count);
        }

        [Fact]
        public void GetExtraRoadsEveryElementShouldContainValuesForAllLocalizations()
        {
            // Arrange/Act
            List<ExtraRoads> result = ExtraPresets.GetExtraRoads().ToList();

            // Assert
            Assert.All(result, x =>
            {
                Assert.False(string.IsNullOrWhiteSpace(x.LocaEng));
                Assert.False(string.IsNullOrWhiteSpace(x.LocaEsp));
                Assert.False(string.IsNullOrWhiteSpace(x.LocaFra));
                Assert.False(string.IsNullOrWhiteSpace(x.LocaGer));
                Assert.False(string.IsNullOrWhiteSpace(x.LocaPol));
                Assert.False(string.IsNullOrWhiteSpace(x.LocaRus));
            });
        }

        [Fact]
        public void GetExtraRoadsEveryElementShouldCorrectCommonValues()
        {
            // Arrange/Act
            List<ExtraRoads> result = ExtraPresets.GetExtraRoads().ToList();

            // Assert
            Assert.All(result, x =>
            {
                Assert.True(x.Road);
                Assert.True(x.Borderless);
                Assert.Null(x.Group);
                Assert.Equal("- Road Presets", x.Header);
                Assert.Null(x.IconFileName);
                Assert.Equal("Road", x.Template);
            });
        }

        [Fact]
        public void GetExtraRoadsEveryElementShouldHaveCorrectIdentifier()
        {
            // Arrange/Act
            List<ExtraRoads> result = ExtraPresets.GetExtraRoads().ToList();

            // Assert
            Assert.All(result, x =>
            {
                Assert.StartsWith("Street_", x.Identifier, StringComparison.Ordinal);
            });
        }

        #endregion

        #region BlockingTile tests

        [Fact]
        public void GetBlockingTilesShouldReturnCorrectCount()
        {
            // Arrange
            int expectedCount = 1;

            // Act
            List<BlockingTile> result = ExtraPresets.GetBlockingTiles().ToList();

            // Assert
            Assert.Equal(expectedCount, result.Count);
        }

        [Fact]
        public void GetBlockingTilesEveryElementShouldContainValuesForAllLocalizations()
        {
            // Arrange/Act
            List<BlockingTile> result = ExtraPresets.GetBlockingTiles().ToList();

            // Assert
            Assert.All(result, x =>
            {
                Assert.False(string.IsNullOrWhiteSpace(x.LocaEng));
                Assert.False(string.IsNullOrWhiteSpace(x.LocaEsp));
                Assert.False(string.IsNullOrWhiteSpace(x.LocaFra));
                Assert.False(string.IsNullOrWhiteSpace(x.LocaGer));
                Assert.False(string.IsNullOrWhiteSpace(x.LocaPol));
                Assert.False(string.IsNullOrWhiteSpace(x.LocaRus));
            });
        }

        [Fact]
        public void GetBlockingTilesEveryElementShouldCorrectCommonValues()
        {
            // Arrange/Act
            List<BlockingTile> result = ExtraPresets.GetBlockingTiles().ToList();

            // Assert
            Assert.All(result, x =>
            {
                Assert.False(x.Road);
                Assert.True(x.Borderless);
                Assert.Null(x.Group);
                Assert.Null(x.IconFileName);
                Assert.Equal("Common", x.Faction);
                Assert.Equal("(a0)- Blocking Presets", x.Header);
                Assert.Equal("Blocker", x.Template);
            });
        }

        [Fact]
        public void GetBlockingTilesEveryElementShouldHaveCorrectIdentifier()
        {
            // Arrange/Act
            List<BlockingTile> result = ExtraPresets.GetBlockingTiles().ToList();

            // Assert
            Assert.All(result, x =>
            {
                Assert.StartsWith("BlockTile_", x.Identifier, StringComparison.Ordinal);
            });
        }

        #endregion
    }
}
