using AnnoDesigner.Core.Models;
using PresetParser.Anno1404_Anno2070;
using PresetParser.Models;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace PresetParser.Tests
{
    public class LocalizationHelperTests
    {
        #region testdata

        private static readonly Dictionary<string, Dictionary<string, PathRef[]>> testData_versionSpecificPaths_Anno1404;
        private static readonly Dictionary<string, Dictionary<string, PathRef[]>> testData_versionSpecificPaths_Anno2070;

        static LocalizationHelperTests()
        {
            testData_versionSpecificPaths_Anno1404 = new Dictionary<string, Dictionary<string, PathRef[]>>
            {
                { Constants.ANNO_VERSION_1404, [] }
            };
            testData_versionSpecificPaths_Anno1404[Constants.ANNO_VERSION_1404].Add("localisation", new PathRef[]
            {
                new("data/loca"),
                new("addondata/loca")
            });

            testData_versionSpecificPaths_Anno2070 = new Dictionary<string, Dictionary<string, PathRef[]>>
            {
                { Constants.ANNO_VERSION_2070, [] }
            };
            testData_versionSpecificPaths_Anno2070[Constants.ANNO_VERSION_2070].Add("localisation", new PathRef[]
            {
                new("data/loca"),
            });
        }

        #endregion

        [Fact]
        public void CtorFileSystemIsNullShouldThrow()
        {
            // Arrange
            LocalizationHelper helper;

            // Act/Assert
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => helper = new LocalizationHelper(null));
        }

        #region common tests

        [Theory]
        [InlineData(Constants.ANNO_VERSION_1800)]
        [InlineData(Constants.ANNO_VERSION_2205)]
        public void GetLocalizationAnnoVersionIsNot1404Or2070ShouldReturnNull(string annoVersionToTest)
        {
            // Arrange
            LocalizationHelper helper = new(new MockFileSystem());

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(annoVersionToTest,
                addPrefix: false,
                versionSpecificPaths: testData_versionSpecificPaths_Anno1404,
                languages: Array.Empty<string>(),
                basePath: "dummy");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetLocalizationVersionSpecificPathsAreNullShouldThrow()
        {
            // Arrange
            LocalizationHelper helper = new(new MockFileSystem());
            string[] languages = new string[] { "dummy" };

            // Act/Assert
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => helper.GetLocalization(Constants.ANNO_VERSION_1404, addPrefix: false, null, languages, basePath: "dummy"));

            // Assert
            Assert.NotNull(ex);
        }

        [Fact]
        public void GetLocalizationLanguagesAreNullShouldThrow()
        {
            // Arrange
            LocalizationHelper helper = new(new MockFileSystem());

            // Act/Assert
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => helper.GetLocalization(Constants.ANNO_VERSION_1404, addPrefix: false, testData_versionSpecificPaths_Anno1404, null, basePath: "dummy"));
        }

        [Fact]
        public void GetLocalizationVersionSpecificPathsDoesNotContainAnnoVersionShouldThrow()
        {
            // Arrange
            LocalizationHelper helper = new(new MockFileSystem());

            string[] languages = new string[] { "dummy" };

            Dictionary<string, Dictionary<string, PathRef[]>> versionSpecificPaths = new()
            {
                { "dummy", [] },
                { Constants.ANNO_VERSION_2205, [] }
            };

            // Act/Assert
            ArgumentException ex = Assert.Throws<ArgumentException>(() => helper.GetLocalization(Constants.ANNO_VERSION_1404, addPrefix: false, versionSpecificPaths, languages, basePath: "dummy"));
        }

        [Fact]
        public void GetLocalizationNoFilesFoundShouldReturnEmpty()
        {
            // Arrange
            LocalizationHelper helper = new(new MockFileSystem());
            string[] languages = new string[] { "dummy" };

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_1404, addPrefix: false, testData_versionSpecificPaths_Anno1404, languages, basePath: "dummy");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetLocalizationFilesContainOnlyCommentsShouldReturnEmpty()
        {
            // Arrange
            string basePath = "dummy";
            string[] languages = new string[] { "eng" };

            MockFileSystem mockedFileSystem = new(new Dictionary<string, MockFileData>
            {
                { $@"{basePath}\data\loca\{languages[0]}\txt\guids.txt", new MockFileData($"#first comment{Environment.NewLine}#second comment") },
                { $@"{basePath}\addondata\loca\{languages[0]}\txt\guids.txt", new MockFileData($"#first addon comment{Environment.NewLine}#second addon comment") },
            });

            LocalizationHelper helper = new(mockedFileSystem);

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_1404, addPrefix: false, testData_versionSpecificPaths_Anno1404, languages, basePath);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetLocalizationFilesContainOnlyEmptyLinesShouldReturnEmpty()
        {
            // Arrange
            string basePath = "dummy";
            string[] languages = new string[] { "eng" };

            MockFileSystem mockedFileSystem = new(new Dictionary<string, MockFileData>
            {
                { $@"{basePath}\data\loca\{languages[0]}\txt\guids.txt", new MockFileData($"{string.Empty}{Environment.NewLine}  ") },
                { $@"{basePath}\addondata\loca\{languages[0]}\txt\guids.txt", new MockFileData($"{string.Empty}{Environment.NewLine}  ") },
            });

            LocalizationHelper helper = new(mockedFileSystem);

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_1404, addPrefix: false, testData_versionSpecificPaths_Anno1404, languages, basePath);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetLocalizationLinesContainNoDelimiterShouldReturnEmpty()
        {
            // Arrange
            string basePath = "dummy";
            string[] languages = new string[] { "eng" };

            MockFileSystem mockedFileSystem = new(new Dictionary<string, MockFileData>
            {
                { $@"{basePath}\data\loca\{languages[0]}\txt\guids.txt", new MockFileData($"this is {Environment.NewLine} a test ") },
                { $@"{basePath}\addondata\loca\{languages[0]}\txt\guids.txt", new MockFileData($"this is {Environment.NewLine} a test ") },
            });

            LocalizationHelper helper = new(mockedFileSystem);

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_1404, addPrefix: false, testData_versionSpecificPaths_Anno1404, languages, basePath);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region Anno 1404 tests

        [Fact]
        public void GetLocalizationIsAnno1404AndLocalizationIsFoundAndNoPrefixShouldReturnCorrectResult()
        {
            // Arrange
            string basePath = "dummy";
            string[] languages = new string[] { "eng" };
            string expectedGuid1 = "12612";
            string expectedLocalization1 = "Knight";
            string expectedGuid2 = "15926";
            string expectedLocalization2 = "Warship";

            MockFileSystem mockedFileSystem = new(new Dictionary<string, MockFileData>
            {
                { $@"{basePath}\data\loca\{languages[0]}\txt\guids.txt", new MockFileData($"{expectedGuid1}={expectedLocalization1}{Environment.NewLine}{expectedGuid2}={expectedLocalization2}") },
            });

            Dictionary<string, SerializableDictionary<string>> expectedResult = new()
            {
                { expectedGuid1, new SerializableDictionary<string>() }
            };
            expectedResult[expectedGuid1][languages[0]] = expectedLocalization1;
            expectedResult.Add(expectedGuid2, new SerializableDictionary<string>());
            expectedResult[expectedGuid2][languages[0]] = expectedLocalization2;

            LocalizationHelper helper = new(mockedFileSystem);

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_1404, addPrefix: false, testData_versionSpecificPaths_Anno1404, languages, basePath);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(expectedResult[expectedGuid1][languages[0]], result[expectedGuid1][languages[0]]);
            Assert.Equal(expectedResult[expectedGuid2][languages[0]], result[expectedGuid2][languages[0]]);
        }

        [Fact]
        public void GetLocalizationIsAnno1404AndLocalizationIsFoundAndPrefixShouldReturnCorrectResult()
        {
            // Arrange
            string basePath = "dummy";
            string[] languages = new string[] { "eng" };
            string expectedGuid1 = "12612";
            string expectedLocalization1 = "Knight";
            string expectedGuid2 = "15926";
            string expectedLocalization2 = "Warship";
            string expectedPrefix = "A4_";

            MockFileSystem mockedFileSystem = new(new Dictionary<string, MockFileData>
            {
                { $@"{basePath}\data\loca\{languages[0]}\txt\guids.txt", new MockFileData($"{expectedGuid1}={expectedLocalization1}{Environment.NewLine}{expectedGuid2}={expectedLocalization2}") },
            });

            Dictionary<string, SerializableDictionary<string>> expectedResult = new()
            {
                { expectedGuid1, new SerializableDictionary<string>() }
            };
            expectedResult[expectedGuid1][languages[0]] = expectedLocalization1;
            expectedResult.Add(expectedGuid2, new SerializableDictionary<string>());
            expectedResult[expectedGuid2][languages[0]] = expectedLocalization2;

            LocalizationHelper helper = new(mockedFileSystem);

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_1404, addPrefix: true, testData_versionSpecificPaths_Anno1404, languages, basePath);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(expectedPrefix + expectedResult[expectedGuid1][languages[0]], result[expectedGuid1][languages[0]]);
            Assert.Equal(expectedPrefix + expectedResult[expectedGuid2][languages[0]], result[expectedGuid2][languages[0]]);
        }

        [Fact]
        public void GetLocalizationIsAnno1404AndNoPrefixAndIsReferenceWhichIsFoundShouldReturnCorrectResult()
        {
            // Arrange
            string basePath = "dummy";
            string[] languages = new string[] { "eng" };
            string guid1 = "54321";
            string localization1 = "building with spaces";
            string guid2 = "12345";
            string localization2 = $"[GUIDNAME {guid1}]";

            MockFileSystem mockedFileSystem = new(new Dictionary<string, MockFileData>
            {
                { $@"{basePath}\data\loca\{languages[0]}\txt\guids.txt", new MockFileData($"{guid1}={localization1}{Environment.NewLine}{guid2}={localization2}") },
            });

            Dictionary<string, SerializableDictionary<string>> expectedResult = new()
            {
                { guid1, new SerializableDictionary<string>() }
            };
            expectedResult[guid1][languages[0]] = localization1;
            expectedResult.Add(guid2, new SerializableDictionary<string>());
            expectedResult[guid2][languages[0]] = localization1;

            LocalizationHelper helper = new(mockedFileSystem);

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_1404, addPrefix: false, testData_versionSpecificPaths_Anno1404, languages, basePath);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(expectedResult[guid1][languages[0]], result[guid1][languages[0]]);
            Assert.Equal(expectedResult[guid2][languages[0]], result[guid2][languages[0]]);
        }

        [Fact]
        public void GetLocalizationIsAnno1404AndNoPrefixAndIsReferenceWhichIsNotFoundShouldReturnCorrectResult()
        {
            // Arrange
            string basePath = "dummy";
            string[] languages = new string[] { "eng" };
            string guid1 = "54321";
            string localization1 = "building with spaces";
            string guid2 = "12345";
            string localization2 = $"[GUIDNAME 98765]";

            MockFileSystem mockedFileSystem = new(new Dictionary<string, MockFileData>
            {
                { $@"{basePath}\data\loca\{languages[0]}\txt\guids.txt", new MockFileData($"{guid1}={localization1}{Environment.NewLine}{guid2}={localization2}") },
            });

            Dictionary<string, SerializableDictionary<string>> expectedResult = new()
            {
                { guid1, new SerializableDictionary<string>() }
            };
            expectedResult[guid1][languages[0]] = localization1;

            LocalizationHelper helper = new(mockedFileSystem);

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_1404, addPrefix: false, testData_versionSpecificPaths_Anno1404, languages, basePath);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(expectedResult[guid1][languages[0]], result[guid1][languages[0]]);
            _ = Assert.Single(result);
        }

        [Fact]
        public void GetLocalizationIsAnno1404AndPrefixAndIsReferenceWhichIsFoundShouldReturnCorrectResult()
        {
            // Arrange
            string basePath = "dummy";
            string[] languages = new string[] { "eng" };
            string guid1 = "54321";
            string localization1 = "building with spaces";
            string guid2 = "12345";
            string localization2 = $"[GUIDNAME {guid1}]";
            string expectedPrefix = "A4_";

            MockFileSystem mockedFileSystem = new(new Dictionary<string, MockFileData>
            {
                { $@"{basePath}\data\loca\{languages[0]}\txt\guids.txt", new MockFileData($"{guid1}={localization1}{Environment.NewLine}{guid2}={localization2}") },
            });

            Dictionary<string, SerializableDictionary<string>> expectedResult = new()
            {
                { guid1, new SerializableDictionary<string>() }
            };
            expectedResult[guid1][languages[0]] = localization1;

            LocalizationHelper helper = new(mockedFileSystem);

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_1404, addPrefix: true, testData_versionSpecificPaths_Anno1404, languages, basePath);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(expectedPrefix + expectedResult[guid1][languages[0]], result[guid1][languages[0]]);
        }

        #endregion

        #region Anno 2070 tests

        [Fact]
        public void GetLocalizationIsAnno2070AndLocalizationIsFoundAndNoPrefixShouldReturnCorrectResult()
        {
            // Arrange
            string basePath = "dummy";
            string[] languages = new string[] { "eng" };
            string expectedGuid1 = "12612";
            string expectedLocalization1 = "Knight";
            string expectedGuid2 = "15926";
            string expectedLocalization2 = "Warship";

            MockFileSystem mockedFileSystem = new(new Dictionary<string, MockFileData>
            {
                { $@"{basePath}\data\loca\{languages[0]}\txt\guids.txt", new MockFileData($"{expectedGuid1}={expectedLocalization1}{Environment.NewLine}{expectedGuid2}={expectedLocalization2}") },
            });

            Dictionary<string, SerializableDictionary<string>> expectedResult = new()
            {
                { expectedGuid1, new SerializableDictionary<string>() }
            };
            expectedResult[expectedGuid1][languages[0]] = expectedLocalization1;
            expectedResult.Add(expectedGuid2, new SerializableDictionary<string>());
            expectedResult[expectedGuid2][languages[0]] = expectedLocalization2;

            LocalizationHelper helper = new(mockedFileSystem);

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_2070, addPrefix: false, testData_versionSpecificPaths_Anno2070, languages, basePath);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(expectedResult[expectedGuid1][languages[0]], result[expectedGuid1][languages[0]]);
            Assert.Equal(expectedResult[expectedGuid2][languages[0]], result[expectedGuid2][languages[0]]);
        }

        [Fact]
        public void GetLocalizationIsAnno2070AndLocalizationIsFoundAndPrefixShouldReturnCorrectResult()
        {
            // Arrange
            string basePath = "dummy";
            string[] languages = new string[] { "eng" };
            string expectedGuid1 = "12612";
            string expectedLocalization1 = "Knight";
            string expectedGuid2 = "15926";
            string expectedLocalization2 = "Warship";
            string expectedPrefix = "A5_";

            MockFileSystem mockedFileSystem = new(new Dictionary<string, MockFileData>
            {
                { $@"{basePath}\data\loca\{languages[0]}\txt\guids.txt", new MockFileData($"{expectedGuid1}={expectedLocalization1}{Environment.NewLine}{expectedGuid2}={expectedLocalization2}") },
            });

            Dictionary<string, SerializableDictionary<string>> expectedResult = new()
            {
                { expectedGuid1, new SerializableDictionary<string>() }
            };
            expectedResult[expectedGuid1][languages[0]] = expectedLocalization1;
            expectedResult.Add(expectedGuid2, new SerializableDictionary<string>());
            expectedResult[expectedGuid2][languages[0]] = expectedLocalization2;

            LocalizationHelper helper = new(mockedFileSystem);

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_2070, addPrefix: true, testData_versionSpecificPaths_Anno2070, languages, basePath);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(expectedPrefix + expectedResult[expectedGuid1][languages[0]], result[expectedGuid1][languages[0]]);
            Assert.Equal(expectedPrefix + expectedResult[expectedGuid2][languages[0]], result[expectedGuid2][languages[0]]);
        }

        [Fact]
        public void GetLocalizationIsAnno2070AndIsSpecialGuidAndNoPrefixShouldReturnCorrectResult()
        {
            // Arrange
            string basePath = "dummy";
            string[] languages = new string[] { "eng", "ger", "fra", "pol", "rus", "esp", "non_existing" };
            string expectedGuid = "10239";
            string[] expectedLocalizations = new string[] { "Black Smoker", "Black Smoker", "Convertisseur de métal", "Komin hydrotermalny", "Черный курильщик", "Fumador Negro" };

            MockFileSystem mockedFileSystem = new(new Dictionary<string, MockFileData>
            {
                { $@"{basePath}\data\loca\{languages[0]}\txt\guids.txt", new MockFileData($"{expectedGuid}=dummy") },
                { $@"{basePath}\data\loca\{languages[1]}\txt\guids.txt", new MockFileData($"{expectedGuid}=dummy") },
                { $@"{basePath}\data\loca\{languages[2]}\txt\guids.txt", new MockFileData($"{expectedGuid}=dummy") },
                { $@"{basePath}\data\loca\{languages[3]}\txt\guids.txt", new MockFileData($"{expectedGuid}=dummy") },
                { $@"{basePath}\data\loca\{languages[4]}\txt\guids.txt", new MockFileData($"{expectedGuid}=dummy") },
                { $@"{basePath}\data\loca\{languages[5]}\txt\guids.txt", new MockFileData($"{expectedGuid}=dummy") },
                { $@"{basePath}\data\loca\{languages[6]}\txt\guids.txt", new MockFileData($"{expectedGuid}=dummy") },
            });

            Dictionary<string, SerializableDictionary<string>> expectedResult = new()
            {
                { expectedGuid, new SerializableDictionary<string>() }
            };
            expectedResult[expectedGuid][languages[0]] = expectedLocalizations[0];
            expectedResult[expectedGuid][languages[1]] = expectedLocalizations[1];
            expectedResult[expectedGuid][languages[2]] = expectedLocalizations[2];
            expectedResult[expectedGuid][languages[3]] = expectedLocalizations[3];
            expectedResult[expectedGuid][languages[4]] = expectedLocalizations[4];
            expectedResult[expectedGuid][languages[5]] = expectedLocalizations[5];
            expectedResult[expectedGuid][languages[6]] = "dummy";

            LocalizationHelper helper = new(mockedFileSystem);

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_2070, addPrefix: false, testData_versionSpecificPaths_Anno2070, languages, basePath);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(expectedResult[expectedGuid][languages[0]], result[expectedGuid][languages[0]]);
            Assert.Equal(expectedResult[expectedGuid][languages[1]], result[expectedGuid][languages[1]]);
            Assert.Equal(expectedResult[expectedGuid][languages[2]], result[expectedGuid][languages[2]]);
            Assert.Equal(expectedResult[expectedGuid][languages[3]], result[expectedGuid][languages[3]]);
            Assert.Equal(expectedResult[expectedGuid][languages[4]], result[expectedGuid][languages[4]]);
            Assert.Equal(expectedResult[expectedGuid][languages[5]], result[expectedGuid][languages[5]]);
            Assert.Equal(expectedResult[expectedGuid][languages[6]], result[expectedGuid][languages[6]]);
        }

        [Fact]
        public void GetLocalizationIsAnno2070AndIsSpecialGuidAndPrefixShouldReturnCorrectResult()
        {
            // Arrange
            string basePath = "dummy";
            string[] languages = new string[] { "eng" };
            string expectedGuid = "10239";
            string expectedLocalization = "dummy";
            string expectedPrefix = "A5_";

            MockFileSystem mockedFileSystem = new(new Dictionary<string, MockFileData>
            {
                { $@"{basePath}\data\loca\{languages[0]}\txt\guids.txt", new MockFileData($"{expectedGuid}={expectedLocalization}") },
            });

            Dictionary<string, SerializableDictionary<string>> expectedResult = new()
            {
                { expectedGuid, new SerializableDictionary<string>() }
            };
            expectedResult[expectedGuid][languages[0]] = expectedLocalization;

            LocalizationHelper helper = new(mockedFileSystem);

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_2070, addPrefix: true, testData_versionSpecificPaths_Anno2070, languages, basePath);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(expectedPrefix + expectedResult[expectedGuid][languages[0]], result[expectedGuid][languages[0]]);
        }

        [Fact]
        public void GetLocalizationIsAnno2070AndNoPrefixAndIsReferenceWhichIsFoundShouldReturnCorrectResult()
        {
            // Arrange
            string basePath = "dummy";
            string[] languages = new string[] { "eng" };
            string guid1 = "54321";
            string localization1 = "building with spaces";
            string guid2 = "12345";
            string localization2 = $"[GUIDNAME {guid1}]";

            MockFileSystem mockedFileSystem = new(new Dictionary<string, MockFileData>
            {
                { $@"{basePath}\data\loca\{languages[0]}\txt\guids.txt", new MockFileData($"{guid1}={localization1}{Environment.NewLine}{guid2}={localization2}") },
            });

            Dictionary<string, SerializableDictionary<string>> expectedResult = new()
            {
                { guid1, new SerializableDictionary<string>() }
            };
            expectedResult[guid1][languages[0]] = localization1;
            expectedResult.Add(guid2, new SerializableDictionary<string>());
            expectedResult[guid2][languages[0]] = localization1;

            LocalizationHelper helper = new(mockedFileSystem);

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_2070, addPrefix: false, testData_versionSpecificPaths_Anno2070, languages, basePath);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(expectedResult[guid1][languages[0]], result[guid1][languages[0]]);
            Assert.Equal(expectedResult[guid2][languages[0]], result[guid2][languages[0]]);
        }

        [Fact]
        public void GetLocalizationIsAnno2070AndNoPrefixAndIsReferenceWhichIsNotFoundShouldReturnCorrectResult()
        {
            // Arrange
            string basePath = "dummy";
            string[] languages = new string[] { "eng" };
            string guid1 = "54321";
            string localization1 = "building with spaces";
            string guid2 = "12345";
            string localization2 = $"[GUIDNAME 98765]";

            MockFileSystem mockedFileSystem = new(new Dictionary<string, MockFileData>
            {
                { $@"{basePath}\data\loca\{languages[0]}\txt\guids.txt", new MockFileData($"{guid1}={localization1}{Environment.NewLine}{guid2}={localization2}") },
            });

            Dictionary<string, SerializableDictionary<string>> expectedResult = new()
            {
                { guid1, new SerializableDictionary<string>() }
            };
            expectedResult[guid1][languages[0]] = localization1;

            LocalizationHelper helper = new(mockedFileSystem);

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_2070, addPrefix: false, testData_versionSpecificPaths_Anno2070, languages, basePath);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(expectedResult[guid1][languages[0]], result[guid1][languages[0]]);
            _ = Assert.Single(result);
        }

        [Fact]
        public void GetLocalizationIsAnno2070AndPrefixAndIsReferenceWhichIsFoundShouldReturnCorrectResult()
        {
            // Arrange
            string basePath = "dummy";
            string[] languages = new string[] { "eng" };
            string guid1 = "54321";
            string localization1 = "building with spaces";
            string guid2 = "12345";
            string localization2 = $"[GUIDNAME {guid1}]";
            string expectedPrefix = "A5_";

            MockFileSystem mockedFileSystem = new(new Dictionary<string, MockFileData>
            {
                { $@"{basePath}\data\loca\{languages[0]}\txt\guids.txt", new MockFileData($"{guid1}={localization1}{Environment.NewLine}{guid2}={localization2}") },
            });

            Dictionary<string, SerializableDictionary<string>> expectedResult = new()
            {
                { guid1, new SerializableDictionary<string>() }
            };
            expectedResult[guid1][languages[0]] = localization1;

            LocalizationHelper helper = new(mockedFileSystem);

            // Act
            Dictionary<string, SerializableDictionary<string>> result = helper.GetLocalization(Constants.ANNO_VERSION_2070, addPrefix: true, testData_versionSpecificPaths_Anno2070, languages, basePath);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(expectedPrefix + expectedResult[guid1][languages[0]], result[guid1][languages[0]]);
        }

        #endregion
    }
}
