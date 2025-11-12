using System.Xml;
using Xunit;

namespace PresetParser.Tests
{
    public class IconFileNameHelperTests
    {
        [Fact]
        public void GetIconFilenameAnnoVersionIs1404ShouldReturnFileNameWithPrefix()
        {
            // Arrange
            IconFileNameHelper helper = new();

            XmlDocument doc = new();
            doc.LoadXml("<root><IconFileID>myFileId</IconFileID><IconIndex>42</IconIndex></root>");
            XmlElement rootNode = doc["root"];

            // Act
            string result = helper.GetIconFilename(rootNode, Constants.ANNO_VERSION_1404);

            // Assert
            Assert.StartsWith("A4_", result);
        }

        [Fact]
        public void GetIconFilenameAnnoVersionIs1404AndNoIconIndexShouldReturnFileNameWithPrefixAndIconIndexZero()
        {
            // Arrange
            IconFileNameHelper helper = new();

            XmlDocument doc = new();
            doc.LoadXml("<root><IconFileID>myFileId</IconFileID></root>");
            XmlElement rootNode = doc["root"];

            // Act
            string result = helper.GetIconFilename(rootNode, Constants.ANNO_VERSION_1404);

            // Assert
            Assert.Equal("A4_icon_myFileId_0.png", result);
        }

        [Fact]
        public void GetIconFilenameAnnoVersionIs1404AndIconIndexShouldReturnFileNameWithPrefixAndIconIndex()
        {
            // Arrange
            IconFileNameHelper helper = new();

            XmlDocument doc = new();
            doc.LoadXml("<root><IconFileID>myFileId</IconFileID><IconIndex>42</IconIndex></root>");
            XmlElement rootNode = doc["root"];

            // Act
            string result = helper.GetIconFilename(rootNode, Constants.ANNO_VERSION_1404);

            // Assert
            Assert.Equal("A4_icon_myFileId_42.png", result);
        }

        [Fact]
        public void GetIconFilenameAnnoVersionIsNot1404AndNoIconIndexShouldReturnFileNameWithIconIndexZero()
        {
            // Arrange
            IconFileNameHelper helper = new();

            XmlDocument doc = new();
            doc.LoadXml("<root><IconFileID>myFileId</IconFileID></root>");
            XmlElement rootNode = doc["root"];

            // Act
            string result = helper.GetIconFilename(rootNode, Constants.ANNO_VERSION_1800);

            // Assert
            Assert.Equal("icon_myFileId_0.png", result);
        }

        [Fact]
        public void GetIconFilenameAnnoVersionIsNot1404AndIconIndexShouldReturnFileNameWithIconIndex()
        {
            // Arrange
            IconFileNameHelper helper = new();

            XmlDocument doc = new();
            doc.LoadXml("<root><IconFileID>myFileId</IconFileID><IconIndex>42</IconIndex></root>");
            XmlElement rootNode = doc["root"];

            // Act
            string result = helper.GetIconFilename(rootNode, Constants.ANNO_VERSION_2205);

            // Assert
            Assert.Equal("icon_myFileId_42.png", result);
        }
    }
}
