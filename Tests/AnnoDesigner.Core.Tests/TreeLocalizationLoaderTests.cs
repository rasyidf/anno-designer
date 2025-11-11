using AnnoDesigner.Core.Presets.Loader;
using AnnoDesigner.Core.Presets.Models;
using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xunit;

namespace AnnoDesigner.Core.Tests
{
    public class TreeLocalizationLoaderTests
    {
        private readonly IFileSystem _mockedFileSystem;

        private static readonly string testData_1language_2translations;
        private static readonly string pathToPresetLocalization;

        static TreeLocalizationLoaderTests()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            testData_1language_2translations = System.IO.File.ReadAllText(System.IO.Path.Combine(basePath, "Testdata", "TreeLocalization", "1language_2translations.json"), Encoding.UTF8);
            pathToPresetLocalization = System.IO.Path.Combine(basePath, "Testdata", "TreeLocalization", CoreConstants.PresetsFiles.TreeLocalizationFile);
        }

        public TreeLocalizationLoaderTests()
        {
            _mockedFileSystem = new MockFileSystem();
        }

        [Fact]
        public void Load_CanLoadPresetFile()
        {
            // Arrange
            TreeLocalizationLoader loader = new(new FileSystem());

            // Act
            TreeLocalizationContainer result = loader.LoadFromFile(pathToPresetLocalization);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Languages.Count > 1);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Load_FilePathIsNullOrWhiteSpace_ShouldThrow(string filePath)
        {
            // Arrange
            TreeLocalizationLoader loader = new(_mockedFileSystem);

            // Act/Assert
            _ = Assert.Throws<ArgumentNullException>(() => loader.LoadFromFile(filePath));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Load_ParameterIsNullOrWhiteSpace_ShouldThrowArgumentException(string jsonString)
        {
            // Arrange
            TreeLocalizationLoader loader = new(_mockedFileSystem);

            // Act/Assert
            _ = Assert.Throws<ArgumentNullException>(() => loader.Load(jsonString));
        }

        [Fact]
        public void Load_ParameterContainsOnlyWhiteSpaceChararcters_ShouldThrow()
        {
            // Arrange
            string jsonString = @"\t\t\t    \t";
            TreeLocalizationLoader loader = new(_mockedFileSystem);

            // Act/Assert
            _ = Assert.ThrowsAny<Exception>(() => loader.Load(jsonString));
        }

        [Fact]
        public void Load_FileHas1LanguageAnd2Translations_ShouldReturnCorrectContainer()
        {
            // Arrange
            string filePath = @"C:\test\dummyFile.json";
            MockFileSystem fileSystem = new();
            fileSystem.AddFile(filePath, new MockFileData(testData_1language_2translations, Encoding.UTF8));

            TreeLocalizationLoader loader = new(fileSystem);

            // Act
            TreeLocalizationContainer result = loader.LoadFromFile(filePath);

            // Assert
            _ = Assert.Single(result.Languages);
            Assert.Equal(2, result.Languages[0].Translations.Count);
            Assert.Equal("1.0.0.0", result.Version);
        }

        [Fact]
        public void Load_StringHas1LanguageAnd2Translations_ShouldReturnCorrectContainer()
        {
            // Arrange
            TreeLocalizationLoader loader = new(_mockedFileSystem);

            // Act
            TreeLocalizationContainer result = loader.Load(testData_1language_2translations);

            // Assert
            _ = Assert.Single(result.Languages);
            Assert.Equal(2, result.Languages[0].Translations.Count);
            Assert.Equal("1.0.0.0", result.Version);
        }
    }
}
