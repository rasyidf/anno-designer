using AnnoDesigner.Core.Presets.Models;
using Moq;
using PresetParser.Extensions;
using System.Collections.Generic;
using Xunit;

namespace PresetParser.Tests
{
    public class StringExtensionsTests
    {
        private const string DUMMY = "Dummy";

        #region helper methods

        private static IBuildingInfo GetBuilding(string identifierToUse)
        {
            Mock<IBuildingInfo> mockedBuilding = new();
            _ = mockedBuilding.Setup(x => x.Identifier).Returns(identifierToUse);

            return mockedBuilding.Object;
        }

        #endregion

        #region IsMatch - buildings

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void IsMatchBuildingsIdentifierIsNullOrWhitespaceShouldReturnFalse(string identifierToCheck)
        {
            // Arrange            
            List<IBuildingInfo> listToCheck =
            [
                GetBuilding(DUMMY)
            ];

            // Act
            bool result = StringExtensions.IsMatch(identifierToCheck, listToCheck);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMatchBuildingsListIsEmptyShouldReturnFalse()
        {
            // Arrange            
            List<IBuildingInfo> listToCheck = [];

            // Act
            bool result = StringExtensions.IsMatch(DUMMY, listToCheck);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMatchBuildingsListIsNullShouldReturnFalse()
        {
            // Arrange            
            List<IBuildingInfo> listToCheck = null;

            // Act
            bool result = StringExtensions.IsMatch(DUMMY, listToCheck);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region IsMatch - strings

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void IsMatchStringsIdentifierIsNullOrWhitespaceShouldReturnFalse(string identifierToCheck)
        {
            // Arrange            
            List<string> listToCheck =
            [
                DUMMY
            ];

            // Act
            bool result = StringExtensions.IsMatch(identifierToCheck, listToCheck);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMatchStringsListIsEmptyShouldReturnFalse()
        {
            // Arrange            
            List<string> listToCheck = [];

            // Act
            bool result = StringExtensions.IsMatch(DUMMY, listToCheck);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMatchStringsListIsNullShouldReturnFalse()
        {
            // Arrange            
            List<string> listToCheck = null;

            // Act
            bool result = StringExtensions.IsMatch(DUMMY, listToCheck);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}
