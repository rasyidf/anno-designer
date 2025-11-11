using AnnoDesigner.Core.Presets.Comparer;
using AnnoDesigner.Core.Presets.Models;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AnnoDesigner.Core.Tests
{
    public class BuildingInfoComparerTests
    {
        private const string GROUP_FIRST = "Production";
        private const string GROUP_SECOND = "Public Service";

        #region Equals tests

        [Fact]
        public void Implements_Interface_IEqualityComparer()
        {
            Assert.True(typeof(IEqualityComparer<IBuildingInfo>).IsAssignableFrom(typeof(BuildingInfoComparer)));
        }

        [Fact]
        public void Equals_BothEqual_ShouldReturnTrue()
        {
            // Arrange
            Mock<IBuildingInfo> mockedElement1 = new();
            _ = mockedElement1.SetupGet(x => x.Group).Returns(GROUP_FIRST);

            Mock<IBuildingInfo> mockedElement2 = new();
            _ = mockedElement2.SetupGet(x => x.Group).Returns(GROUP_FIRST);

            BuildingInfoComparer comparer = new();

            // Act/Assert
            Assert.True(comparer.Equals(mockedElement1.Object, mockedElement2.Object));
        }

        [Fact]
        public void Equals_OneIsNull_ShouldReturnFalse()
        {
            // Arrange
            Mock<IBuildingInfo> mockedElement = new();
            _ = mockedElement.SetupGet(x => x.Group).Returns(GROUP_FIRST);

            BuildingInfoComparer comparer = new();

            // Act/Assert
            Assert.False(comparer.Equals(mockedElement.Object, null));
            Assert.False(comparer.Equals(null, mockedElement.Object));
        }

        [Fact]
        public void Equals_BothAreNull_ShouldReturnTrue()
        {
            // Arrange
            BuildingInfoComparer comparer = new();

            // Act/Assert
            Assert.True(comparer.Equals(null, null));
        }

        [Fact]
        public void Equals_DifferentGroup_ShouldReturnFalse()
        {
            // Arrange
            Mock<IBuildingInfo> mockedElement1 = new();
            _ = mockedElement1.SetupGet(x => x.Group).Returns(GROUP_FIRST);

            Mock<IBuildingInfo> mockedElement2 = new();
            _ = mockedElement2.SetupGet(x => x.Group).Returns(GROUP_SECOND);

            BuildingInfoComparer comparer = new();

            // Act/Assert
            Assert.False(comparer.Equals(mockedElement1.Object, mockedElement2.Object));
        }

        #endregion

        #region GetHashCode tests

        [Fact]
        public void GetHashCode_BothEqual_ShouldBeEqual()
        {
            // Arrange
            Mock<IBuildingInfo> mockedElement1 = new();
            _ = mockedElement1.SetupGet(x => x.Group).Returns(GROUP_FIRST);

            Mock<IBuildingInfo> mockedElement2 = new();
            _ = mockedElement2.SetupGet(x => x.Group).Returns(GROUP_FIRST);

            BuildingInfoComparer comparer = new();

            // Act
            int hashCode1 = comparer.GetHashCode(mockedElement1.Object);
            int hashCode2 = comparer.GetHashCode(mockedElement2.Object);

            // Assert
            Assert.Equal(hashCode1, hashCode2);
        }

        [Fact]
        public void GetHashCode_DifferentGroup_ShouldNotBeEqual()
        {
            // Arrange
            Mock<IBuildingInfo> mockedElement1 = new();
            _ = mockedElement1.SetupGet(x => x.Group).Returns(GROUP_FIRST);

            Mock<IBuildingInfo> mockedElement2 = new();
            _ = mockedElement2.SetupGet(x => x.Group).Returns(GROUP_SECOND);

            BuildingInfoComparer comparer = new();

            // Act
            int hashCode1 = comparer.GetHashCode(mockedElement1.Object);
            int hashCode2 = comparer.GetHashCode(mockedElement2.Object);

            // Assert
            Assert.NotEqual(hashCode1, hashCode2);
        }

        [Fact]
        public void GetHashCode_ElementIsNull_ShouldNotThrow()
        {
            // Arrange
            BuildingInfoComparer comparer = new();

            // Act
            int hashCode = comparer.GetHashCode(null);

            // Assert
            Assert.Equal(-1, hashCode);
        }

        #endregion
    }
}
