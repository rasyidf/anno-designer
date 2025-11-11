using AnnoDesigner.Core.Presets.Models;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace PresetParser.Tests
{
    public class ValidatorTests
    {
        private readonly ITestOutputHelper _out;

        private readonly Validator _validator;
        private readonly List<IBuildingInfo> _testData_valid_buildingInfo;
        private readonly List<IBuildingInfo> _testData_invalid_buildingInfo;

        public ValidatorTests(ITestOutputHelper outputHelperToUse)
        {
            _out = outputHelperToUse;
            _validator = new Validator();

            _testData_valid_buildingInfo =
            [
                new BuildingInfo { Identifier = "A4_building 1" },
                new BuildingInfo { Identifier = "A4_building 2" },
                new BuildingInfo { Identifier = "A4_building 3" },
                new BuildingInfo { Identifier = "A7_building 3" },
            ];

            _testData_invalid_buildingInfo =
            [
                new BuildingInfo { Identifier = "A4_building 1" },
                new BuildingInfo { Identifier = "A4_building 2" },
                new BuildingInfo { Identifier = "A4_building 1" },
                new BuildingInfo { Identifier = "A4_building 3" },
            ];
        }

        [Fact]
        public void CheckForUniqueIdentifiersKnownDuplicatesIsNullShouldNotThrow()
        {
            // Arrange/Act
            (bool isValid, List<string> duplicateIdentifiers) = _validator.CheckForUniqueIdentifiers(_testData_valid_buildingInfo, null);

            // Assert
            Assert.True(isValid);
            Assert.Empty(duplicateIdentifiers);
        }

        [Fact]
        public void CheckForUniqueIdentifiersKnownDuplicatesIsEmptyShouldNotThrow()
        {
            // Arrange/Act
            (bool isValid, List<string> duplicateIdentifiers) = _validator.CheckForUniqueIdentifiers(_testData_valid_buildingInfo, []);

            // Assert
            Assert.True(isValid);
            Assert.Empty(duplicateIdentifiers);
        }

        [Fact]
        public void CheckForUniqueIdentifiersNoDuplicateIdentifiersShouldReturnIsValid()
        {
            // Arrange/Act
            (bool isValid, List<string> duplicateIdentifiers) = _validator.CheckForUniqueIdentifiers(_testData_valid_buildingInfo, []);

            // Assert
            Assert.True(isValid);
            Assert.Empty(duplicateIdentifiers);
        }

        [Fact]
        public void CheckForUniqueIdentifiersDuplicateIdentifiersShouldReturnNotValidAndListOfIdentifiers()
        {
            // Arrange/Act
            (bool isValid, List<string> duplicateIdentifiers) = _validator.CheckForUniqueIdentifiers(_testData_invalid_buildingInfo, []);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(duplicateIdentifiers);
            _out.WriteLine(string.Join(" ,", duplicateIdentifiers));
        }

        [Fact]
        public void CheckForUniqueIdentifiersDuplicateIdentifiersAndKnownDuplicatesShouldReturnNotValidAndListOfIdentifiers()
        {
            // Arrange
            string knownDuplicate = "A99_known duplicate";
            _testData_invalid_buildingInfo.Add(new BuildingInfo { Identifier = knownDuplicate });
            _testData_invalid_buildingInfo.Insert(0, new BuildingInfo { Identifier = knownDuplicate });

            // Arrange
            (bool isValid, List<string> duplicateIdentifiers) = _validator.CheckForUniqueIdentifiers(_testData_invalid_buildingInfo, [knownDuplicate]);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(duplicateIdentifiers);
            _out.WriteLine(string.Join(" ,", duplicateIdentifiers));
        }

        [Fact]
        public void CheckForUniqueIdentifiersKnownDuplicatesFoundShouldReturnIsValid()
        {
            // Arrange
            string knownDuplicate = "A99_known duplicate";
            _testData_valid_buildingInfo.Add(new BuildingInfo { Identifier = knownDuplicate });
            _testData_valid_buildingInfo.Insert(0, new BuildingInfo { Identifier = knownDuplicate });

            // Act
            (bool isValid, List<string> duplicateIdentifiers) = _validator.CheckForUniqueIdentifiers(_testData_valid_buildingInfo, [knownDuplicate]);

            // Assert
            Assert.True(isValid);
            Assert.Empty(duplicateIdentifiers);
        }
    }
}
