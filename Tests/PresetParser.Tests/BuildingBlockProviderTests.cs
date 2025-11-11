using AnnoDesigner.Core.Presets.Models;
using Moq;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Xml;
using Xunit;
using Xunit.Abstractions;

namespace PresetParser.Tests
{
    //disable parallel execution of tests, because of culture awareness in some of the tests
    [CollectionDefinition(nameof(BuildingBlockProviderTests), DisableParallelization = true)]
    public class BuildingBlockProviderTests
    {
        private readonly IFileSystem _fileSystem;
        #region testdata

        private static readonly string testData_Bakery;
        private readonly ITestOutputHelper _out;
        #endregion
        static BuildingBlockProviderTests()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            testData_Bakery = _fileSystem.File.ReadAllText(Path.Combine(basePath, "Testdata", "1404_Bakery.txt"), Encoding.UTF8);
            _fileSystem = new FileSystem();
        }
        public BuildingBlockProviderTests(ITestOutputHelper outputHelper)
        {
            _out = outputHelper;
        }

        #region ctor tests

        [Fact]
        public void ctorIfoProviderIsNullShouldThrow()
        {
            // Arrange/Act
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() =>
            {
                BuildingBlockProvider provider = new(null);
            });

            // Assert
            Assert.NotNull(ex);
        }

        #endregion

        #region GetBuildingBlocker Anno1404 tests

        [Fact]
        public void GetBuildingBlockerAnno1404BuildBlockerNotFoundShouldReturnFalse()
        {
            // Arrange
            XmlDocument mockedDocument = new();
            mockedDocument.LoadXml("<Info><Dummy></Dummy></Info>");

            Mock<IIfoFileProvider> mockedIfoProvider = new();
            _ = mockedIfoProvider.Setup(x => x.GetIfoFileContent(It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockedDocument);

            BuildingBlockProvider provider = new(mockedIfoProvider.Object);

            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.SetupAllProperties();

            // Act
            bool result = provider.GetBuildingBlocker("basePath", mockedBuilding.Object, "variationFilename", Constants.ANNO_VERSION_1404);

            // Assert
            Assert.False(result);
            Assert.NotNull(mockedBuilding.Object.BuildBlocker);
        }

        [Fact]
        public void GetBuildingBlockerAnno1404BuildBlockerHasNoChildNodeShouldReturnFalse()
        {
            // Arrange
            XmlDocument mockedDocument = new();
            mockedDocument.LoadXml("<Info><BuildBlocker></BuildBlocker></Info>");

            Mock<IIfoFileProvider> mockedIfoProvider = new();
            _ = mockedIfoProvider.Setup(x => x.GetIfoFileContent(It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockedDocument);

            BuildingBlockProvider provider = new(mockedIfoProvider.Object);

            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.SetupAllProperties();

            // Act
            bool result = provider.GetBuildingBlocker("basePath", mockedBuilding.Object, "variationFilename", Constants.ANNO_VERSION_1404);

            // Assert
            Assert.False(result);
            Assert.NotNull(mockedBuilding.Object.BuildBlocker);
        }

        [Fact]
        public void GetBuildingBlockerAnno1404BothValuesZeroShouldReturnFalse()
        {
            // Arrange
            XmlDocument mockedDocument = new();
            mockedDocument.LoadXml("<Info><BuildBlocker><Position><x>300</x><z>300</z></Position></BuildBlocker></Info>");

            Mock<IIfoFileProvider> mockedIfoProvider = new();
            _ = mockedIfoProvider.Setup(x => x.GetIfoFileContent(It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockedDocument);

            BuildingBlockProvider provider = new(mockedIfoProvider.Object);

            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.SetupAllProperties();

            // Act
            bool result = provider.GetBuildingBlocker("basePath", mockedBuilding.Object, "variationFilename", Constants.ANNO_VERSION_1404);

            // Assert
            Assert.False(result);
            Assert.NotNull(mockedBuilding.Object.BuildBlocker);
            Assert.Empty(mockedBuilding.Object.BuildBlocker.Dict);
        }

        [Fact]
        public void GetBuildingBlockerAnno1404ValueForxIsZeroShouldSetxToOne()
        {
            // Arrange
            XmlDocument mockedDocument = new();
            mockedDocument.LoadXml("<Info><BuildBlocker><Position><x>300</x><z>8192</z></Position></BuildBlocker></Info>");

            Mock<IIfoFileProvider> mockedIfoProvider = new();
            _ = mockedIfoProvider.Setup(x => x.GetIfoFileContent(It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockedDocument);

            BuildingBlockProvider provider = new(mockedIfoProvider.Object);

            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.SetupAllProperties();

            // Act
            bool result = provider.GetBuildingBlocker("basePath", mockedBuilding.Object, "variationFilename", Constants.ANNO_VERSION_1404);

            // Assert
            Assert.True(result);
            Assert.NotNull(mockedBuilding.Object.BuildBlocker);
            Assert.Equal(1, mockedBuilding.Object.BuildBlocker["x"]);
            Assert.Equal(4, mockedBuilding.Object.BuildBlocker["z"]);
        }

        [Fact]
        public void GetBuildingBlockerAnno1404ValueForzIsZeroShouldSetzToOne()
        {
            // Arrange
            XmlDocument mockedDocument = new();
            mockedDocument.LoadXml("<Info><BuildBlocker><Position><x>8192</x><z>300</z></Position></BuildBlocker></Info>");

            Mock<IIfoFileProvider> mockedIfoProvider = new();
            _ = mockedIfoProvider.Setup(x => x.GetIfoFileContent(It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockedDocument);

            BuildingBlockProvider provider = new(mockedIfoProvider.Object);

            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.SetupAllProperties();

            // Act
            bool result = provider.GetBuildingBlocker("basePath", mockedBuilding.Object, "variationFilename", Constants.ANNO_VERSION_1404);

            // Assert
            Assert.True(result);
            Assert.NotNull(mockedBuilding.Object.BuildBlocker);
            Assert.Equal(4, mockedBuilding.Object.BuildBlocker["x"]);
            Assert.Equal(1, mockedBuilding.Object.BuildBlocker["z"]);
        }

        [Fact]
        public void GetBuildingBlockerAnno1404VariationWaterMillEcosShouldReturnCorrectValue()
        {
            // Arrange
            XmlDocument mockedDocument = new();
            mockedDocument.LoadXml("<Info><BuildBlocker><Position><x>-8192</x><z>8192</z></Position></BuildBlocker></Info>");

            Mock<IIfoFileProvider> mockedIfoProvider = new();
            _ = mockedIfoProvider.Setup(x => x.GetIfoFileContent(It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockedDocument);

            BuildingBlockProvider provider = new(mockedIfoProvider.Object);

            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.SetupAllProperties();

            // Act
            bool result = provider.GetBuildingBlocker("basePath", mockedBuilding.Object, "water_mill_ecos.txt", Constants.ANNO_VERSION_1404);

            // Assert
            Assert.True(result);
            Assert.NotNull(mockedBuilding.Object.BuildBlocker);
            Assert.Equal(3, mockedBuilding.Object.BuildBlocker["x"]);
            Assert.Equal(7, mockedBuilding.Object.BuildBlocker["z"]);
        }

        [Fact]
        public void GetBuildingBlockerAnno1404VariationOrnamentalPost09ShouldReturnCorrectValue()
        {
            // Arrange
            XmlDocument mockedDocument = new();
            mockedDocument.LoadXml("<Info><BuildBlocker><Position><x>-8192</x><z>8192</z></Position></BuildBlocker></Info>");

            Mock<IIfoFileProvider> mockedIfoProvider = new();
            _ = mockedIfoProvider.Setup(x => x.GetIfoFileContent(It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockedDocument);

            BuildingBlockProvider provider = new(mockedIfoProvider.Object);

            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.SetupAllProperties();

            // Act
            bool result = provider.GetBuildingBlocker("basePath", mockedBuilding.Object, "ornamental_post_09.txt", Constants.ANNO_VERSION_1404);

            // Assert
            Assert.True(result);
            Assert.NotNull(mockedBuilding.Object.BuildBlocker);
            Assert.Equal(7, mockedBuilding.Object.BuildBlocker["x"]);
            Assert.Equal(7, mockedBuilding.Object.BuildBlocker["z"]);
        }

        [Fact]
        public void GetBuildingBlockerAnno1404BakeryShouldReturnCorrectValue()
        {
            // Arrange
            XmlDocument mockedDocument = new();
            mockedDocument.LoadXml(testData_Bakery);

            Mock<IIfoFileProvider> mockedIfoProvider = new();
            _ = mockedIfoProvider.Setup(x => x.GetIfoFileContent(It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockedDocument);

            BuildingBlockProvider provider = new(mockedIfoProvider.Object);

            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.SetupAllProperties();

            // Act
            bool result = provider.GetBuildingBlocker("basePath", mockedBuilding.Object, "variationFilename", Constants.ANNO_VERSION_1404);

            // Assert
            Assert.True(result);
            Assert.NotNull(mockedBuilding.Object.BuildBlocker);
            Assert.Equal(3, mockedBuilding.Object.BuildBlocker["x"]);
            Assert.Equal(3, mockedBuilding.Object.BuildBlocker["z"]);
        }

        #endregion

        #region GetBuildingBlocker Anno1800 tests

        [Theory]
        [InlineData("<Info><Dummy></Dummy></Info>")]
        [InlineData("<Info></Info>")]
        public void GetBuildingBlockerAnno1800BuildBlockerNotFoundShouldReturnFalse(string ifodocument)
        {
            // Arrange
            XmlDocument mockedDocument = new();
            mockedDocument.LoadXml(ifodocument);

            Mock<IIfoFileProvider> mockedIfoProvider = new();
            _ = mockedIfoProvider.Setup(x => x.GetIfoFileContent(It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockedDocument);

            BuildingBlockProvider provider = new(mockedIfoProvider.Object);

            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.SetupAllProperties();

            // Act
            bool result = provider.GetBuildingBlocker("basePath", mockedBuilding.Object, "variationFilename", Constants.ANNO_VERSION_1800);

            // Assert
            Assert.False(result);
            Assert.Null(mockedBuilding.Object.BuildBlocker);
        }

        [Fact]
        public void GetBuildingBlockerAnno1800BlockerFoundWitEmptyChildShouldReturnFalse()
        {
            // Arrange
            XmlDocument mockedDocument = new();
            mockedDocument.LoadXml("<Info><BuildBlocker><Position></Position><Position></Position><Position></Position><Position></Position></BuildBlocker></Info>");

            Mock<IIfoFileProvider> mockedIfoProvider = new();
            _ = mockedIfoProvider.Setup(x => x.GetIfoFileContent(It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockedDocument);

            BuildingBlockProvider provider = new(mockedIfoProvider.Object);

            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.SetupAllProperties();

            // Act
            bool result = provider.GetBuildingBlocker("basePath", mockedBuilding.Object, "variationFilename", Constants.ANNO_VERSION_1800);

            // Assert
            Assert.False(result);
            Assert.Null(mockedBuilding.Object.BuildBlocker);
        }

        [Fact]
        public void GetBuildingBlockerAnno1800BlockerFoundButEmptyShouldReturnFalse()
        {
            // Arrange
            XmlDocument mockedDocument = new();
            mockedDocument.LoadXml("<Info><BuildBlocker></BuildBlocker></Info>");

            Mock<IIfoFileProvider> mockedIfoProvider = new();
            _ = mockedIfoProvider.Setup(x => x.GetIfoFileContent(It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockedDocument);

            BuildingBlockProvider provider = new(mockedIfoProvider.Object);

            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.SetupAllProperties();

            // Act
            bool result = provider.GetBuildingBlocker("basePath", mockedBuilding.Object, "variationFilename", Constants.ANNO_VERSION_1800);

            // Assert
            Assert.False(result);
            Assert.Null(mockedBuilding.Object.BuildBlocker);
        }

        [Fact]
        public void GetBuildingBlockerAnno1800BothValuesZeroShouldReturnFalse()
        {
            // Arrange
            XmlDocument mockedDocument = new();
            mockedDocument.LoadXml("<Info><BuildBlocker><Position><xf>0.2</xf><zf>0.2</zf></Position><Position><xf>0.2</xf><zf>-0.2</zf></Position><Position><xf>-0.2</xf><zf>0.2</zf></Position><Position><xf>-0.2</xf><zf>-0.2</zf></Position></BuildBlocker></Info>");

            Mock<IIfoFileProvider> mockedIfoProvider = new();
            _ = mockedIfoProvider.Setup(x => x.GetIfoFileContent(It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockedDocument);

            BuildingBlockProvider provider = new(mockedIfoProvider.Object);

            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.SetupAllProperties();

            // Act
            bool result = provider.GetBuildingBlocker("basePath", mockedBuilding.Object, "variationFilename", Constants.ANNO_VERSION_1800);

            // Assert
            Assert.False(result);
            Assert.NotNull(mockedBuilding.Object.BuildBlocker);
            Assert.Empty(mockedBuilding.Object.BuildBlocker.Dict);
        }

        [CulturedFact]
        [Fact]
        public static void GetBuildingBlockerAnno1800ValueForxIsZeroShouldSetxToOne()
        {
            // Arrange
            XmlDocument mockedDocument = new();
            mockedDocument.LoadXml("<Info><BuildBlocker><Position><xf>0.2</xf><zf>2</zf></Position><Position><xf>0.2</xf><zf>-2</zf></Position><Position><xf>-0.2</xf><zf>2</zf></Position><Position><xf>-0.2</xf><zf>-2</zf></Position></BuildBlocker></Info>");

            Mock<IIfoFileProvider> mockedIfoProvider = new();
            _ = mockedIfoProvider.Setup(x => x.GetIfoFileContent(It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockedDocument);

            BuildingBlockProvider provider = new(mockedIfoProvider.Object);

            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.SetupAllProperties();

            IBuildingInfo building = mockedBuilding.Object;

            // Act
            bool result = provider.GetBuildingBlocker("basePath", building, "variationFilename", Constants.ANNO_VERSION_1800);

            // Assert
            Assert.True(result);
            Assert.NotNull(building.BuildBlocker);
            Assert.Equal(1, building.BuildBlocker["x"]);
            Assert.Equal(4, building.BuildBlocker["z"]);
        }

        [CulturedFact]
        [Fact]
        public static void GetBuildingBlockerAnno1800ValueForzIsZeroShouldSetzToOne()
        {
            // Arrange
            XmlDocument mockedDocument = new();
            mockedDocument.LoadXml("<Info><BuildBlocker><Position><xf>2</xf><zf>0.2</zf></Position><Position><xf>2</xf><zf>-0.2</zf></Position><Position><xf>-2</xf><zf>0.2</zf></Position><Position><xf>-2</xf><zf>-0.2</zf></Position></BuildBlocker></Info>");

            Mock<IIfoFileProvider> mockedIfoProvider = new();
            _ = mockedIfoProvider.Setup(x => x.GetIfoFileContent(It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockedDocument);

            BuildingBlockProvider provider = new(mockedIfoProvider.Object);

            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.SetupAllProperties();

            // Act
            bool result = provider.GetBuildingBlocker("basePath", mockedBuilding.Object, "variationFilename", Constants.ANNO_VERSION_1800);

            // Assert
            Assert.True(result);
            Assert.NotNull(mockedBuilding.Object.BuildBlocker);
            Assert.Equal(4, mockedBuilding.Object.BuildBlocker["x"]);
            Assert.Equal(1, mockedBuilding.Object.BuildBlocker["z"]);
        }

        [CulturedFact]
        [Fact]
        public static void GetBuildingBlockerAnno1800BuildingIsPalaceGateShouldSetCorrectSize()
        {
            // Arrange
            XmlDocument mockedDocument = new();
            mockedDocument.LoadXml("<Info><BuildBlocker><Position><xf>1.5</xf><zf>1.5</zf></Position><Position><xf>1.5</xf><zf>-1.5</zf></Position><Position><xf>-1.5</xf><zf>0.5</zf></Position><Position><xf>-1.5</xf><zf>-1.5</zf></Position></BuildBlocker></Info>");

            Mock<IIfoFileProvider> mockedIfoProvider = new();
            _ = mockedIfoProvider.Setup(x => x.GetIfoFileContent(It.IsAny<string>(), It.IsAny<string>())).Returns(() => mockedDocument);

            BuildingBlockProvider provider = new(mockedIfoProvider.Object);

            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.SetupAllProperties();
            _ = mockedBuilding.SetupGet(x => x.Identifier).Returns("Palace_Module_05 (gate)");

            // Act
            bool result = provider.GetBuildingBlocker("basePath", mockedBuilding.Object, "variationFilename", Constants.ANNO_VERSION_1800);

            // Assert
            Assert.True(result);
            Assert.NotNull(mockedBuilding.Object.BuildBlocker);
            Assert.Equal(3, mockedBuilding.Object.BuildBlocker["x"]);
            Assert.Equal(3, mockedBuilding.Object.BuildBlocker["z"]);
        }

        #endregion
    }
}
