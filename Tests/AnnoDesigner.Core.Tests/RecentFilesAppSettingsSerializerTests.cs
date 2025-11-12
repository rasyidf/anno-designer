using AnnoDesigner.Core.Models;
using AnnoDesigner.Core.RecentFiles;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace AnnoDesigner.Core.Tests
{
    public class RecentFilesAppSettingsSerializerTests
    {
        private IRecentFilesSerializer GetSerializer(IAppSettings appSettingsToUse = null)
        {
            Mock<IAppSettings> mockedSettings = new();
            _ = mockedSettings.SetupAllProperties();

            IAppSettings settings = appSettingsToUse ?? mockedSettings.Object;

            return new RecentFilesAppSettingsSerializer(settings);
        }

        #region Deserialize tests

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Deserialize_SavedListIsIsNullOrWhiteSpace_ShouldReturnEmptyList(string savedList)
        {
            // Arrange
            Mock<IAppSettings> mockedSettings = new();
            _ = mockedSettings.SetupAllProperties();
            _ = mockedSettings.SetupGet(x => x.RecentFiles).Returns(() => savedList);

            IRecentFilesSerializer serializer = GetSerializer(mockedSettings.Object);

            // Act
            List<RecentFile> result = serializer.Deserialize();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Deserialize_CanNotDeserialize_ShouldReturnEmptyList()
        {
            // Arrange
            Mock<IAppSettings> mockedSettings = new();
            _ = mockedSettings.SetupAllProperties();
            _ = mockedSettings.SetupGet(x => x.RecentFiles).Returns(() => "[{\"myPath\":\"dummyPath\"}]");

            IRecentFilesSerializer serializer = GetSerializer(mockedSettings.Object);

            // Act
            List<RecentFile> result = serializer.Deserialize();

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region Serialize tests

        [Fact]
        public void Serialize_ParameterIsNull_ShouldNotThrow()
        {
            // Arrange
            IRecentFilesSerializer serializer = GetSerializer();

            // Act
            Exception ex = Record.Exception(() => serializer.Serialize(null));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void Serialize_ParameterIsNotNull_ShouldCallSaveOnAppSettings()
        {
            // Arrange
            Mock<IAppSettings> mockedSettings = new();
            _ = mockedSettings.SetupAllProperties();
            _ = mockedSettings.SetupGet(x => x.RecentFiles).Returns(() => string.Empty);

            IRecentFilesSerializer serializer = GetSerializer(mockedSettings.Object);

            // Act
            serializer.Serialize([]);

            // Assert
            mockedSettings.Verify(x => x.Save(), Times.Once);
        }

        #endregion
    }
}
