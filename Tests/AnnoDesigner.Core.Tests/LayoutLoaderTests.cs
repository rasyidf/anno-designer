using AnnoDesigner.Core.Layout;
using AnnoDesigner.Core.Layout.Exceptions;
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
    public class LayoutLoaderTests
    {
        private readonly IFileSystem _fileSystem;
        #region testdata

        private static readonly string testData_v3_LayoutWithVersionAndObjects;
        private static readonly string testData_v4_LayoutWithVersionAndObjects;
        private static readonly string testData_LayoutWithNoVersionAndObjects;
        #endregion
        static LayoutLoaderTests()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            testData_v3_LayoutWithVersionAndObjects = _fileSystem.File.ReadAllText(Path.Combine(basePath, "Testdata", "Layout", "v3_layoutWithVersionAndObjects.ad"), Encoding.UTF8);
            testData_v4_LayoutWithVersionAndObjects = _fileSystem.File.ReadAllText(Path.Combine(basePath, "Testdata", "Layout", "v4_layoutWithVersionAndObjects.ad"), Encoding.UTF8);
            testData_LayoutWithNoVersionAndObjects = _fileSystem.File.ReadAllText(Path.Combine(basePath, "Testdata", "Layout", "layoutWithNoVersionAndObjects.ad"), Encoding.UTF8);
            _fileSystem = new FileSystem();
        }
        [Fact]
        public void LoadLayout_StreamIsNull_ShouldThrow()
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();

            // Act and Assert
            _ = Assert.Throws<ArgumentNullException>(() => loader.LoadLayout((Stream)null));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void LoadLayout_FilePathIsNullOrWhiteSpace_ShouldThrow(string filePath)
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();

            // Act and Assert
            _ = Assert.Throws<ArgumentNullException>(() => loader.LoadLayout(filePath));
        }

        [Fact]
        public void LoadLayout_LayoutHasOlderSupportedVersion_ShouldReturnListWithObjects()
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();

            string layoutContent = $"{{\"FileVersion\":{CoreConstants.LayoutFileVersionSupportedMinimum},\"Objects\":[]}}";
            using MemoryStream streamWithLayout = new(Encoding.UTF8.GetBytes(layoutContent));

            // Act
            LayoutFile result = loader.LoadLayout(streamWithLayout);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void LoadLayout_LayoutHasOlderUnsupportedVersion_ShouldThrow()
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();

            string layoutContent = $"{{\"FileVersion\":{CoreConstants.LayoutFileVersionSupportedMinimum - 1},\"Objects\":[]}}";
            using MemoryStream streamWithLayout = new(Encoding.UTF8.GetBytes(layoutContent));

            // Act and Assert
            _ = Assert.Throws<LayoutFileUnsupportedFormatException>(() => loader.LoadLayout(streamWithLayout));
        }

        [Fact]
        public void LoadLayout_LayoutHasNewerVersion_ShouldThrow()
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();

            string layoutContent = $"{{\"FileVersion\":{CoreConstants.LayoutFileVersion + 1},\"Objects\":[]}}";
            using MemoryStream streamWithLayout = new(Encoding.UTF8.GetBytes(layoutContent));

            // Act and Assert
            _ = Assert.Throws<LayoutFileUnsupportedFormatException>(() => loader.LoadLayout(streamWithLayout));
        }

        [Fact]
        public void LoadLayout_LayoutHasVersionInfo_ShouldReturnListWithObjects()
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();

            using MemoryStream streamWithLayout = new(Encoding.UTF8.GetBytes(testData_v4_LayoutWithVersionAndObjects));

            // Act
            LayoutFile result = loader.LoadLayout(streamWithLayout);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void LoadLayout_LayoutHasNoVersionInfoAndIsForcedLoad_ShouldReturnListWithObjects()
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();

            using MemoryStream streamWithLayout = new(Encoding.UTF8.GetBytes(testData_LayoutWithNoVersionAndObjects));

            // Act
            LayoutFile result = loader.LoadLayout(streamWithLayout, true);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void LoadLayout_LayoutHasNewerVersionAndIsForcedToLoad_ShouldReturnListWithObjects()
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();

            string layoutContent = $"{{\"FileVersion\":{CoreConstants.LayoutFileVersion + 1},\"Objects\":[{{\"Identifier\":\"Lorem\"}}]}}";
            using MemoryStream streamWithLayout = new(Encoding.UTF8.GetBytes(layoutContent));

            // Act
            LayoutFile result = loader.LoadLayout(streamWithLayout, true);

            // Assert
            _ = Assert.Single(result.Objects);
        }

        [Fact]
        public void LoadLayout_LayoutHasOlderVersionAndIsForcedToLoad_ShouldReturnListWithObjects()
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();
            using MemoryStream streamWithLayout = new(Encoding.UTF8.GetBytes(testData_v3_LayoutWithVersionAndObjects));

            // Act
            LayoutFile result = loader.LoadLayout(streamWithLayout, true);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void LoadLayout_LayoutHasBuildingWithTransparentColor_ShouldReturnListWithObjectAndTransparentColor()
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();

            int expectedA = 92;
            int expectedR = 0;
            int expectedG = 242;
            int expectedB = 63;
            string layoutContent = $"{{\"FileVersion\":{CoreConstants.LayoutFileVersion},\"Objects\":[{{\"Identifier\":\"Lorem\",\"Color\":{{\"A\":{expectedA},\"R\":{expectedR},\"G\":{expectedG},\"B\":{expectedB}}}}}]}}";
            using MemoryStream streamWithLayout = new(Encoding.UTF8.GetBytes(layoutContent));

            // Act
            List<AnnoObject> result = loader.LoadLayout(streamWithLayout, true).Objects;

            // Assert
            _ = Assert.Single(result);
            Assert.Equal(expectedA, result[0].Color.A);
            Assert.Equal(expectedR, result[0].Color.R);
            Assert.Equal(expectedG, result[0].Color.G);
            Assert.Equal(expectedB, result[0].Color.B);
        }

        [Fact]
        public void SaveLayout_LayoutHasOneObject_ShouldReturnListWithOneObject()
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();

            using MemoryStream savedStream = new();

            List<AnnoObject> listToSave = [new() { Identifier = "Lorem" }];

            // Act
            loader.SaveLayout(new LayoutFile(listToSave), savedStream);

            savedStream.Position = 0;

            LayoutFile result = loader.LoadLayout(savedStream);

            // Assert
            _ = Assert.Single(result.Objects);
        }

        [Fact]
        public void SaveLayout_LayoutHasBuildingWithTransparentColor_ShouldReturnListWithOneObjectAndTransparentColor()
        {
            // Arrange
            ILayoutLoader loader = new LayoutLoader();

            byte expectedA = 92;
            byte expectedR = 0;
            byte expectedG = 242;
            byte expectedB = 63;

            using MemoryStream savedStream = new();

            List<AnnoObject> listToSave = [new() { Identifier = "Lorem", Color = new SerializableColor(expectedA, expectedR, expectedG, expectedB) }];

            // Act
            loader.SaveLayout(new LayoutFile(listToSave), savedStream);

            savedStream.Position = 0;

            List<AnnoObject> result = loader.LoadLayout(savedStream).Objects;

            // Assert
            _ = Assert.Single(result);
            Assert.Equal(expectedA, result[0].Color.A);
            Assert.Equal(expectedR, result[0].Color.R);
            Assert.Equal(expectedG, result[0].Color.G);
            Assert.Equal(expectedB, result[0].Color.B);
        }
    }
}
