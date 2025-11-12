using AnnoDesigner.Core.Presets.Loader;
using AnnoDesigner.Core.Presets.Models;
using System;
using Xunit;

namespace AnnoDesigner.Core.Tests
{
    public class IconMappingPresetsLoaderTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Load_ParameterIsNullOrWhiteSpace_ShouldThrowArgumentException(string jsonString)
        {
            // Arrange
            IconMappingPresetsLoader loader = new();

            // Act/Assert
            _ = Assert.Throws<ArgumentNullException>(() => loader.Load(jsonString));
        }

        [Fact]
        public void Load_ParameterContainsOnlyWhiteSpaceChararcters_ShouldThrow()
        {
            // Arrange
            string jsonString = @"\t\t\t    \t";
            IconMappingPresetsLoader loader = new();

            // Act/Assert
            _ = Assert.ThrowsAny<Exception>(() => loader.Load(jsonString));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Load_FilePathIsNullOrWhiteSpace_ShouldThrow(string filePath)
        {
            // Arrange
            IconMappingPresetsLoader loader = new();

            // Act/Assert
            _ = Assert.Throws<ArgumentNullException>(() => loader.LoadFromFile(filePath));
        }

        [Fact]
        public void Load_FileHasNoVersionAndOneMapping_ShouldReturnListWithOneMapping()
        {
            // Arrange
            IconMappingPresetsLoader loader = new();
            string content = "[{\"IconFilename\":\"icon.png\",\"Localizations\":{\"eng\":\"mapped name\"}}]";

            // Act
            IconMappingPresets result = loader.Load(content);

            // Assert
            _ = Assert.Single(result.IconNameMappings);
            Assert.Equal(string.Empty, result.Version);
        }

        [Fact]
        public void Load_FileHasVersionAndOneMapping_ShouldReturnListWithOneMapping()
        {
            // Arrange
            IconMappingPresetsLoader loader = new();
            string content = "{\"Version\":\"0.1\",\"IconNameMappings\":[{\"IconFilename\":\"icon.png\",\"Localizations\":{\"eng\":\"mapped name\"}}]}";

            // Act
            IconMappingPresets result = loader.Load(content);

            // Assert
            _ = Assert.Single(result.IconNameMappings);
            Assert.Equal("0.1", result.Version);
        }
    }
}
