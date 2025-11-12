using AnnoDesigner.Core.Layout;
using AnnoDesigner.Core.Layout.Models;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Core.Services;
using AnnoDesigner.Core.Tests.Mocks;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace AnnoDesigner.Core.Tests
{
    public class ClipboardServiceTests
    {
        private readonly MockedClipboard _mockedClipboard;
        private readonly ILayoutLoader _mockedLayoutLoader;
        private static readonly string testData_v4_LayoutWithVersionAndObjects;

        static ClipboardServiceTests()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            testData_v4_LayoutWithVersionAndObjects = System.IO.File.ReadAllText(System.IO.Path.Combine(basePath, "Testdata", "Layout", "v4_layoutWithVersionAndObjects.ad"), Encoding.UTF8);
        }

        public ClipboardServiceTests()
        {
            _mockedClipboard = new MockedClipboard();
            _mockedLayoutLoader = new LayoutLoader();
        }

        private IClipboardService GetService(ILayoutLoader layoutLoaderToUse = null,
            IClipboard clipboardToUse = null)
        {
            return new ClipboardService(layoutLoaderToUse ?? _mockedLayoutLoader,
                clipboardToUse ?? _mockedClipboard);
        }

        private List<AnnoObject> GetListOfObjects()
        {
            return
            [
                new AnnoObject
                {
                    Identifier = "my dummy"
                }
            ];
        }

        #region Copy tests

        [Fact]
        public void Copy_ListIsEmpty_ShouldNotAddDataToClipboard()
        {
            // Arrange
            IClipboardService service = GetService();

            // Act
            service.Copy(Array.Empty<AnnoObject>());

            // Assert
            Assert.Null(_mockedClipboard.GetData(CoreConstants.AnnoDesignerClipboardFormat));
        }

        [Fact]
        public void Copy_ListIsNull_ShouldNotThrowAndNotAddDataToClipboard()
        {
            // Arrange
            IClipboardService service = GetService();

            // Act
            Exception ex = Record.Exception(() => service.Copy(null));

            // Assert
            Assert.Null(ex);
            Assert.Null(_mockedClipboard.GetData(CoreConstants.AnnoDesignerClipboardFormat));
        }

        [Fact]
        public void Copy_ListHasData_ShouldAddDataToClipboard()
        {
            // Arrange
            IClipboardService service = GetService();
            List<AnnoObject> data = GetListOfObjects();

            // Act
            service.Copy(data);

            // Assert
            Stream clipboardStream = _mockedClipboard.GetData(CoreConstants.AnnoDesignerClipboardFormat) as System.IO.Stream;
            List<AnnoObject> copiedObjects = _mockedLayoutLoader.LoadLayout(clipboardStream, forceLoad: true).Objects;

            Assert.Equal(data.Count, copiedObjects.Count);
            Assert.All(data, x =>
            {
                //Assert.Contains(x, copiedObjects); //not useable because of missing cutom comparer for AnnoObject
                Assert.Contains(copiedObjects, y => y.Identifier.Equals(x.Identifier, StringComparison.OrdinalIgnoreCase));
            });
        }

        [Fact]
        public void Copy_ListHasData_ShouldClearClipboardBeforeAddingData()
        {
            // Arrange
            Mock<IClipboard> mockedClipboard = new();
            int callOrder = 0;
            _ = mockedClipboard.Setup(x => x.Clear()).Callback(() => Assert.Equal(0, callOrder++));
            _ = mockedClipboard.Setup(x => x.SetData(It.IsAny<string>(), It.IsAny<object>())).Callback(() => Assert.Equal(1, callOrder++));

            IClipboardService service = GetService(clipboardToUse: mockedClipboard.Object);
            List<AnnoObject> data = GetListOfObjects();

            // Act
            service.Copy(data);

            // Assert
            mockedClipboard.Verify(x => x.Clear(), Times.Once);
        }

        [Fact]
        public void Copy_ListHasData_ShouldFlushClipboardAfterAddingData()
        {
            // Arrange
            Mock<IClipboard> mockedClipboard = new();
            int callOrder = 0;
            _ = mockedClipboard.Setup(x => x.SetData(It.IsAny<string>(), It.IsAny<object>())).Callback(() => Assert.Equal(0, callOrder++));
            _ = mockedClipboard.Setup(x => x.Flush()).Callback(() => Assert.Equal(1, callOrder++));

            IClipboardService service = GetService(clipboardToUse: mockedClipboard.Object);
            List<AnnoObject> data = GetListOfObjects();

            // Act
            service.Copy(data);

            // Assert
            mockedClipboard.Verify(x => x.Flush(), Times.Once);
        }

        #endregion

        #region Paste tests

        [Fact]
        public void Paste_ClipboardHasNoData_ShouldReturnEmpty()
        {
            // Arrange
            IClipboardService service = GetService();

            // Act
            ICollection<AnnoObject> result = service.Paste();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Paste_Files_ClipboardHasMultipleFiles_ShouldReturnEmpty()
        {
            // Arrange
            IClipboardService service = GetService();
            _mockedClipboard.AddFilesToClipboard(["first file path", "second file path"]);

            // Act
            ICollection<AnnoObject> result = service.Paste();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Paste_Files_ClipboardFileListReturnsNull_ShouldReturnEmpty()
        {
            // Arrange
            Mock<IClipboard> mockedClipboard = new();
            _ = mockedClipboard.Setup(x => x.GetFileDropList()).Returns(() => null);

            IClipboardService service = GetService(clipboardToUse: mockedClipboard.Object);

            // Act
            ICollection<AnnoObject> result = service.Paste();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Paste_Files_ClipboardHasSingleFile_ShouldReturnLayoutObjects()
        {
            // Arrange
            List<AnnoObject> data = GetListOfObjects();

            Mock<ILayoutLoader> mockedLayoutLoader = new();
            _ = mockedLayoutLoader.Setup(x => x.LoadLayout(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(() => new LayoutFile(data));

            IClipboardService service = GetService(layoutLoaderToUse: mockedLayoutLoader.Object);
            _mockedClipboard.AddFilesToClipboard(["first"]);

            // Act
            ICollection<AnnoObject> result = service.Paste();

            // Assert
            Assert.Equal(data, result);
        }

        [Fact]
        public void Paste_Files_ClipboardHasOnlySingleFileAndLayoutLoaderThrows_ShouldReturnEmpty()
        {
            // Arrange
            List<AnnoObject> data = GetListOfObjects();

            Mock<ILayoutLoader> mockedLayoutLoader = new();
            _ = mockedLayoutLoader.Setup(x => x.LoadLayout(It.IsAny<string>(), It.IsAny<bool>()))
                .Throws(new JsonReaderException());

            IClipboardService service = GetService(layoutLoaderToUse: mockedLayoutLoader.Object);
            _mockedClipboard.AddFilesToClipboard(["first"]);

            // Act
            ICollection<AnnoObject> result = service.Paste();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Paste_Data_ClipboardHasUnknownDataType_ShouldReturnEmpty()
        {
            // Arrange
            _mockedClipboard.SetData(CoreConstants.AnnoDesignerClipboardFormat, new { Id = "unknown object" });
            IClipboardService service = GetService();

            // Act
            ICollection<AnnoObject> result = service.Paste();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Paste_Data_ClipboardHasKnownDataType_ShouldReturnLayoutObjects()
        {
            // Arrange
            List<AnnoObject> data = GetListOfObjects();

            using MemoryStream memoryStream = new();
            _mockedLayoutLoader.SaveLayout(new LayoutFile(data), memoryStream);
            _ = memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            _mockedClipboard.SetData(CoreConstants.AnnoDesignerClipboardFormat, memoryStream);

            IClipboardService service = GetService();

            // Act
            ICollection<AnnoObject> result = service.Paste();

            // Assert
            Assert.Equal(data.Count, result.Count);
            Assert.All(data, x =>
            {
                //Assert.Contains(x, result); //not useable because of missing cutom comparer for AnnoObject
                Assert.Contains(result, y => y.Identifier.Equals(x.Identifier, StringComparison.OrdinalIgnoreCase));
            });
        }

        [Fact]
        public void Paste_Data_ClipboardHasKnownDataTypeAndLayoutLoaderThrows_ShouldReturnEmpty()
        {
            // Arrange
            List<AnnoObject> data = GetListOfObjects();

            Mock<ILayoutLoader> mockedLayoutLoader = new();
            _ = mockedLayoutLoader.Setup(x => x.LoadLayout(It.IsAny<System.IO.Stream>(), It.IsAny<bool>()))
                .Throws(new JsonReaderException());

            using MemoryStream memoryStream = new();
            _mockedLayoutLoader.SaveLayout(new LayoutFile(data), memoryStream);
            _ = memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            _mockedClipboard.SetData(CoreConstants.AnnoDesignerClipboardFormat, memoryStream);

            IClipboardService service = GetService(layoutLoaderToUse: mockedLayoutLoader.Object);

            // Act
            ICollection<AnnoObject> result = service.Paste();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Paste_Text_ClipboardHasKnownDataType_ShouldReturnLayoutObjects()
        {
            // Arrange
            _mockedClipboard.SetText(testData_v4_LayoutWithVersionAndObjects);

            using MemoryStream streamWithLayout = new(Encoding.UTF8.GetBytes(testData_v4_LayoutWithVersionAndObjects));
            LayoutFile expectedLayout = _mockedLayoutLoader.LoadLayout(streamWithLayout, forceLoad: true);

            IClipboardService service = GetService();

            // Act
            ICollection<AnnoObject> result = service.Paste();

            // Assert
            Assert.Equal(expectedLayout.Objects.Count, result.Count);
            Assert.All(expectedLayout.Objects, x =>
            {
                //Assert.Contains(x, result); //not useable because of missing cutom comparer for AnnoObject
                Assert.Contains(result, y => y.Identifier.Equals(x.Identifier, StringComparison.OrdinalIgnoreCase));
            });
        }

        [Fact]
        public void Paste_Text_ClipboardHasUnknownDataType_ShouldReturnEmpty()
        {
            // Arrange
            _mockedClipboard.SetText("not a layout");

            IClipboardService service = GetService();

            // Act
            ICollection<AnnoObject> result = service.Paste();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Paste_Text_ClipboardHasKnownDataTypeAndLayoutLoaderThrows_ShouldReturnEmpty()
        {
            // Arrange
            _mockedClipboard.SetText(testData_v4_LayoutWithVersionAndObjects);

            Mock<ILayoutLoader> mockedLayoutLoader = new();
            _ = mockedLayoutLoader.Setup(x => x.LoadLayout(It.IsAny<System.IO.Stream>(), It.IsAny<bool>()))
                .Throws(new JsonReaderException());

            IClipboardService service = GetService(layoutLoaderToUse: mockedLayoutLoader.Object);

            // Act
            ICollection<AnnoObject> result = service.Paste();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion
    }
}
