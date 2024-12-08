using System.Collections.Generic;
using AnnoDesigner.Core.Presets.Models;
using Moq;
using PresetParser.Extensions;
using Xunit;

namespace PresetParser.Tests
{
    public class StringExtensionsTests
    {
        private const string DUMMY = "Dummy";

        #region helper methods

        private IBuildingInfo GetBuilding(string identifierToUse)
        {
            var mockedBuilding = new Mock<IBuildingInfo>();
            _ = mockedBuilding.Setup(x => x.Identifier).Returns(identifierToUse);

            return mockedBuilding.Object;
        }

        #endregion

        #region IsMatch - buildings

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void IsMatch_Buildings_IdentifierIsNullOrWhitespace_ShouldReturnFalse(string identifierToCheck)
        {
            // Arrange            
            var listToCheck = new List<IBuildingInfo>
            {
                GetBuilding(DUMMY)
            };

            // Act
            var result = StringExtensions.IsMatch(identifierToCheck, listToCheck);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMatch_Buildings_ListIsEmpty_ShouldReturnFalse()
        {
            // Arrange            
            var listToCheck = new List<IBuildingInfo>();

            // Act
            var result = StringExtensions.IsMatch(DUMMY, listToCheck);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMatch_Buildings_ListIsNull_ShouldReturnFalse()
        {
            // Arrange            
            List<IBuildingInfo> listToCheck = null;

            // Act
            var result = StringExtensions.IsMatch(DUMMY, listToCheck);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region IsMatch - strings

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void IsMatch_Strings_IdentifierIsNullOrWhitespace_ShouldReturnFalse(string identifierToCheck)
        {
            // Arrange            
            var listToCheck = new List<string>
            {
                DUMMY
            };

            // Act
            var result = StringExtensions.IsMatch(identifierToCheck, listToCheck);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMatch_Strings_ListIsEmpty_ShouldReturnFalse()
        {
            // Arrange            
            var listToCheck = new List<string>();

            // Act
            var result = StringExtensions.IsMatch(DUMMY, listToCheck);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsMatch_Strings_ListIsNull_ShouldReturnFalse()
        {
            // Arrange            
            List<string> listToCheck = null;

            // Act
            var result = StringExtensions.IsMatch(DUMMY, listToCheck);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}
