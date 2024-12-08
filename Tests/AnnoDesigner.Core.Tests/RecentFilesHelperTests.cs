using System.IO.Abstractions.TestingHelpers;

namespace AnnoDesigner.Core.Tests
{
    public class RecentFilesHelperTests
    {
        private static readonly MockFileSystem fileSystemWithTestData;

        static RecentFilesHelperTests()
        {
            var mockedFileSystem = new MockFileSystem();
            mockedFileSystem.AddFile(@"C:\test\sub\file_01.ad", new MockFileData(string.Empty));
            mockedFileSystem.AddFile(@"C:\test\sub\file_02.ad", new MockFileData(string.Empty));
            mockedFileSystem.AddFile(@"C:\test\sub\file_03.ad", new MockFileData(string.Empty));
            mockedFileSystem.AddFile(@"C:\test\sub\file_04.ad", new MockFileData(string.Empty));
            mockedFileSystem.AddFile(@"C:\test\sub\file_05.ad", new MockFileData(string.Empty));
            mockedFileSystem.AddFile(@"C:\test\sub\file_06.ad", new MockFileData(string.Empty));
            mockedFileSystem.AddFile(@"C:\test\sub\file_07.ad", new MockFileData(string.Empty));
            mockedFileSystem.AddFile(@"C:\test\sub\file_08.ad", new MockFileData(string.Empty));
            mockedFileSystem.AddFile(@"C:\test\sub\file_09.ad", new MockFileData(string.Empty));
            mockedFileSystem.AddFile(@"C:\test\sub\file_10.ad", new MockFileData(string.Empty));
            mockedFileSystem.AddFile(@"C:\test\sub\file_11.ad", new MockFileData(string.Empty));

            fileSystemWithTestData = mockedFileSystem;
        }

        // Rest of the code remains unchanged
    }
}
