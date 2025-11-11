using AnnoDesigner.Core.Helper;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Core.RecentFiles;
using Moq;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace AnnoDesigner.Core.Tests
{
    public class RecentFilesHelperTests
    {
        private static readonly MockFileSystem fileSystemWithTestData;

        static RecentFilesHelperTests()
        {
            MockFileSystem mockedFileSystem = new();
            mockedFileSystem.AddFile(@"C:\test\sub\file_01.ad", MockFileData.NullObject);
            mockedFileSystem.AddFile(@"C:\test\sub\file_02.ad", MockFileData.NullObject);
            mockedFileSystem.AddFile(@"C:\test\sub\file_03.ad", MockFileData.NullObject);
            mockedFileSystem.AddFile(@"C:\test\sub\file_04.ad", MockFileData.NullObject);
            mockedFileSystem.AddFile(@"C:\test\sub\file_05.ad", MockFileData.NullObject);
            mockedFileSystem.AddFile(@"C:\test\sub\file_06.ad", MockFileData.NullObject);
            mockedFileSystem.AddFile(@"C:\test\sub\file_07.ad", MockFileData.NullObject);
            mockedFileSystem.AddFile(@"C:\test\sub\file_08.ad", MockFileData.NullObject);
            mockedFileSystem.AddFile(@"C:\test\sub\file_09.ad", MockFileData.NullObject);
            mockedFileSystem.AddFile(@"C:\test\sub\file_10.ad", MockFileData.NullObject);
            mockedFileSystem.AddFile(@"C:\test\sub\file_11.ad", MockFileData.NullObject);

            fileSystemWithTestData = mockedFileSystem;
        }


        #region test data

        private IRecentFilesHelper GetHelper(IRecentFilesSerializer serializerToUse = null,
            IFileSystem fileSystemToUse = null,
            int? maxItemCountToUse = null)
        {
            IRecentFilesSerializer serializer = serializerToUse ?? new RecentFilesInMemorySerializer();
            IFileSystem fileSystem = fileSystemToUse ?? new MockFileSystem();
            int maxItemCount = maxItemCountToUse ?? 10;

            if (serializerToUse is null)
            {
                serializer.Serialize([.. fileSystemWithTestData.AllFiles.Select(path => new RecentFile(path, DateTime.UtcNow))]);
            }

            return new RecentFilesHelper(serializer, fileSystem, maxItemCount);
        }

        #endregion

        #region ctor tests

        [Fact]
        public void Ctor_SerializerIsNull_ShouldThrow()
        {
            // Arrange/Act
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new RecentFilesHelper(null, new MockFileSystem()));

            // Assert
            Assert.NotNull(ex);
        }

        [Fact]
        public void Ctor_FileSystemIsNull_ShouldThrow()
        {
            // Arrange/Act
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new RecentFilesHelper(new RecentFilesInMemorySerializer(), null));

            // Assert
            Assert.NotNull(ex);
        }

        [Fact]
        public void Ctor_Defaults_Set()
        {
            // Arrange/Act
            RecentFilesHelper helper = new(new RecentFilesInMemorySerializer(), new MockFileSystem());

            // Assert
            Assert.Empty(helper.RecentFiles);
            Assert.Equal(10, helper.MaximumItemCount);
        }

        [Fact]
        public void Ctor_Defaults_ShouldCallDeserialize()
        {
            // Arrange
            Mock<IRecentFilesSerializer> mockedSerializer = new();
            _ = mockedSerializer.Setup(_ => _.Deserialize()).Returns(() => []);

            // Arrange/Act
            RecentFilesHelper helper = new(mockedSerializer.Object, new MockFileSystem());

            // Assert
            mockedSerializer.Verify(_ => _.Deserialize(), Times.Once);
        }

        #endregion

        #region AddFile tests

        [Fact]
        public void AddFile_ParameterIsNull_ShouldNotThrow()
        {
            // Arrange
            IRecentFilesHelper helper = GetHelper();

            // Act            
            Exception ex = Record.Exception(() => helper.AddFile(null));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void AddFile_FileExists_ShouldPlaceItOnTop()
        {
            // Arrange
            IRecentFilesHelper helper = GetHelper(fileSystemToUse: fileSystemWithTestData);
            RecentFile fileToAdd = helper.RecentFiles.Last();

            // Act
            helper.AddFile(fileToAdd);

            // Assert
            Assert.Equal(fileToAdd, helper.RecentFiles.First());
        }

        [Fact]
        public void AddFile_MaximumItemCountExceeded_ShouldEnsureMaximumItemCount()
        {
            // Arrange
            int maximumItemCountToSet = 5;
            IRecentFilesSerializer serializer = new RecentFilesInMemorySerializer();
            serializer.Serialize([.. fileSystemWithTestData.AllFiles.Select(path => new RecentFile(path, DateTime.UtcNow))]);

            IRecentFilesHelper helper = GetHelper(fileSystemToUse: fileSystemWithTestData, serializerToUse: serializer);
            helper.MaximumItemCount = maximumItemCountToSet;

            RecentFile fileToAdd = new(@"C:\test\dummyFile.ad", DateTime.UtcNow);

            // Act
            helper.AddFile(fileToAdd);

            // Assert
            Assert.Equal(maximumItemCountToSet, helper.RecentFiles.Count);
        }

        [Fact]
        public void AddFile_FileNotNull_ShouldCallSerialize()
        {
            // Arrange
            Mock<IRecentFilesSerializer> mockedSerializer = new();
            _ = mockedSerializer.Setup(_ => _.Deserialize()).Returns(() => []);

            IRecentFilesHelper helper = GetHelper(serializerToUse: mockedSerializer.Object, fileSystemToUse: fileSystemWithTestData);
            RecentFile fileToAdd = new(fileSystemWithTestData.AllFiles.Last(), DateTime.UtcNow);

            // Act
            helper.AddFile(fileToAdd);

            // Assert
            mockedSerializer.Verify(_ => _.Serialize(It.IsAny<List<RecentFile>>()), Times.Once);
        }

        [Fact]
        public void AddFile_FileNotNull_ShouldRaiseUpdatedEvent()
        {
            // Arrange
            Mock<IRecentFilesSerializer> mockedSerializer = new();
            _ = mockedSerializer.Setup(_ => _.Deserialize()).Returns(() => []);

            IRecentFilesHelper helper = GetHelper(serializerToUse: mockedSerializer.Object, fileSystemToUse: fileSystemWithTestData);
            bool updatedCalled = false;
            helper.Updated += (s, e) => updatedCalled = true;
            RecentFile fileToAdd = new(fileSystemWithTestData.AllFiles.Last(), DateTime.UtcNow);

            // Act
            helper.AddFile(fileToAdd);

            // Assert
            Assert.True(updatedCalled);
        }

        #endregion

        #region RemoveFile tests

        [Fact]
        public void RemoveFile_ParameterIsNull_ShouldNotThrow()
        {
            // Arrange
            IRecentFilesHelper helper = GetHelper();

            // Act
            Exception ex = Record.Exception(() => helper.RemoveFile(null));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void RemoveFile_FileExists_ShouldRemoveItemFromList()
        {
            // Arrange
            IRecentFilesHelper helper = GetHelper(fileSystemToUse: fileSystemWithTestData);
            RecentFile fileToRemove = helper.RecentFiles.Last();

            // Act
            helper.RemoveFile(fileToRemove);

            // Assert
            Assert.DoesNotContain(fileToRemove, helper.RecentFiles);
        }

        [Fact]
        public void RemoveFile_FileNotNull_ShouldCallSerialize()
        {
            // Arrange
            Mock<IRecentFilesSerializer> mockedSerializer = new();
            _ = mockedSerializer.Setup(_ => _.Deserialize()).Returns(() => []);

            IRecentFilesHelper helper = GetHelper(serializerToUse: mockedSerializer.Object, fileSystemToUse: fileSystemWithTestData);
            RecentFile fileToRemove = new(fileSystemWithTestData.AllFiles.Last(), DateTime.UtcNow);

            // Act
            helper.RemoveFile(fileToRemove);

            // Assert
            mockedSerializer.Verify(_ => _.Serialize(It.IsAny<List<RecentFile>>()), Times.Once);
        }

        [Fact]
        public void RemoveFile_FileNotNull_ShouldRaiseUpdatedEvent()
        {
            // Arrange
            Mock<IRecentFilesSerializer> mockedSerializer = new();
            _ = mockedSerializer.Setup(_ => _.Deserialize()).Returns(() => []);

            IRecentFilesHelper helper = GetHelper(serializerToUse: mockedSerializer.Object, fileSystemToUse: fileSystemWithTestData);
            bool updatedCalled = false;
            helper.Updated += (s, e) => updatedCalled = true;
            RecentFile fileToRemove = new(fileSystemWithTestData.AllFiles.Last(), DateTime.UtcNow);

            // Act
            helper.RemoveFile(fileToRemove);

            // Assert
            Assert.True(updatedCalled);
        }

        #endregion

        #region ClearRecentFiles tests

        [Fact]
        public void ClearRecentFiles_HasRecentFiles_ShouldClearListOfRecentFiles()
        {
            // Arrange
            IRecentFilesHelper helper = GetHelper(fileSystemToUse: fileSystemWithTestData);

            Assert.NotEmpty(helper.RecentFiles);

            // Act
            helper.ClearRecentFiles();

            // Assert
            Assert.Empty(helper.RecentFiles);
        }

        [Fact]
        public void ClearRecentFiles_HasRecentFiles_ShouldCallSerialize()
        {
            // Arrange
            Mock<IRecentFilesSerializer> mockedSerializer = new();
            _ = mockedSerializer.Setup(_ => _.Deserialize()).Returns(() => []);

            IRecentFilesHelper helper = GetHelper(serializerToUse: mockedSerializer.Object, fileSystemToUse: fileSystemWithTestData);

            // Act
            helper.ClearRecentFiles();

            // Assert
            mockedSerializer.Verify(_ => _.Serialize(It.IsAny<List<RecentFile>>()), Times.Once);
        }

        [Fact]
        public void ClearRecentFiles_HasRecentFiles_ShouldRaiseUpdatedEvent()
        {
            // Arrange
            Mock<IRecentFilesSerializer> mockedSerializer = new();
            _ = mockedSerializer.Setup(_ => _.Deserialize()).Returns(() => []);

            IRecentFilesHelper helper = GetHelper(serializerToUse: mockedSerializer.Object, fileSystemToUse: fileSystemWithTestData);
            bool updatedCalled = false;
            helper.Updated += (s, e) => updatedCalled = true;

            // Act
            helper.ClearRecentFiles();

            // Assert
            Assert.True(updatedCalled);
        }

        #endregion

        #region MaximumItemCount tests

        [Fact]
        public void MaximumItemCount_DifferentValue_ShouldSetNewValue()
        {
            // Arrange
            int maxItemCount = 5;
            IRecentFilesHelper helper = GetHelper(fileSystemToUse: fileSystemWithTestData);

            Assert.True(helper.RecentFiles.Count > maxItemCount);
            Assert.True(helper.MaximumItemCount > maxItemCount);

            // Act
            helper.MaximumItemCount = maxItemCount;

            // Assert
            Assert.Equal(maxItemCount, helper.MaximumItemCount);
        }

        [Fact]
        public void MaximumItemCount_DifferentValue_ShouldCallSerialize()
        {
            // Arrange
            int maxItemCount = 5;
            Mock<IRecentFilesSerializer> mockedSerializer = new();
            _ = mockedSerializer.Setup(_ => _.Deserialize()).Returns(() => [.. fileSystemWithTestData.AllFiles.Select(path => new RecentFile(path, DateTime.UtcNow))]);
            IRecentFilesHelper helper = GetHelper(serializerToUse: mockedSerializer.Object, fileSystemToUse: fileSystemWithTestData);

            Assert.True(helper.RecentFiles.Count > maxItemCount);
            Assert.True(helper.MaximumItemCount > maxItemCount);

            // Act
            helper.MaximumItemCount = maxItemCount;

            // Assert
            mockedSerializer.Verify(_ => _.Serialize(It.IsAny<List<RecentFile>>()), Times.Once);
        }

        [Fact]
        public void MaximumItemCount_DifferentValue_ShouldRaiseUpdatedEvent()
        {
            // Arrange
            int maxItemCount = 5;
            Mock<IRecentFilesSerializer> mockedSerializer = new();
            _ = mockedSerializer.Setup(_ => _.Deserialize()).Returns(() => [.. fileSystemWithTestData.AllFiles.Select(path => new RecentFile(path, DateTime.UtcNow))]);
            IRecentFilesHelper helper = GetHelper(serializerToUse: mockedSerializer.Object, fileSystemToUse: fileSystemWithTestData);

            Assert.True(helper.RecentFiles.Count > maxItemCount);
            Assert.True(helper.MaximumItemCount > maxItemCount);

            bool updatedCalled = false;
            helper.Updated += (s, e) => updatedCalled = true;

            // Act
            helper.MaximumItemCount = maxItemCount;

            // Assert
            Assert.True(updatedCalled);
        }

        #endregion
    }
}
