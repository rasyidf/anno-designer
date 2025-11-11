using AnnoDesigner.Core;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Core.Presets.Loader;
using AnnoDesigner.Core.Presets.Models;
using AnnoDesigner.Extensions;
using AnnoDesigner.Models;
using AnnoDesigner.Models.PresetsTree;
using AnnoDesigner.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace AnnoDesigner.Tests
{
    public class PresetsTreeViewModelTests
    {
        private readonly IFileSystem _fileSystem;
        private static readonly BuildingPresets _subsetFromPresetsFile;
        private static readonly BuildingPresets _subsetForFiltering;
        private static readonly ILocalizationHelper _mockedTreeLocalization;
        private readonly ICommons _mockedCommons;
        public PresetsTreeViewModelTests()
        {
            Mock<ICommons> commonsMock = new();
            _ = commonsMock.SetupGet(x => x.CurrentLanguage).Returns(() => "English");
            _ = commonsMock.SetupGet(x => x.CurrentLanguageCode).Returns(() => "eng");
            _mockedCommons = commonsMock.Object;
            _fileSystem = new FileSystem();
        }
        static PresetsTreeViewModelTests()
        {
            _subsetFromPresetsFile = InitSubsetFromPresetsFile();
            _subsetForFiltering = InitSubsetForFiltering();

            Mock<ILocalizationHelper> mockedLocalizationHelper = new();
            _ = mockedLocalizationHelper.Setup(x => x.GetLocalization(It.IsAny<string>())).Returns<string>(x => x);
            _ = mockedLocalizationHelper.Setup(x => x.GetLocalization(It.IsAny<string>(), It.IsAny<string>())).Returns((string value, string langauge) => value);
            _mockedTreeLocalization = mockedLocalizationHelper.Object;
        }

        #region test data        

        private (List<GenericTreeItem> items, Dictionary<int, bool> expectedState) GetTreeAndState(bool expandLastMainNode = true)
        {
            //tree state:
            //-item 1       //is expanded, but has no children -> test unexpected behaviour [do not add to list]
            //-item 2       //is not expanded and has no children -> test normal behaviour [do not add to list]
            //-item 3       //is not expanded and has empty children list -> test default behaviour [do not add to list]
            //-item 4       //is expanded and has children -> test condensed behaviour [add true to list]
            // -item 41     //is expanded, but has no children -> test unexpected behaviour [do not add to list]
            //-item 5       //is expanded and has children -> test condensed behaviour [add true to list]
            // -item 51     //is not expanded, but has children -> test normal behaviour [add false to list]
            // -item 52     //is expanded and has children -> test condensed behaviour [add true to list]
            //  -item 521   //is not expanded and has no children -> test normal behaviour [do not add to list]

            _ = new            //tree state:
            //-item 1       //is expanded, but has no children -> test unexpected behaviour [do not add to list]
            //-item 2       //is not expanded and has no children -> test normal behaviour [do not add to list]
            //-item 3       //is not expanded and has empty children list -> test default behaviour [do not add to list]
            //-item 4       //is expanded and has children -> test condensed behaviour [add true to list]
            // -item 41     //is expanded, but has no children -> test unexpected behaviour [do not add to list]
            //-item 5       //is expanded and has children -> test condensed behaviour [add true to list]
            // -item 51     //is not expanded, but has children -> test normal behaviour [add false to list]
            // -item 52     //is expanded and has children -> test condensed behaviour [add true to list]
            //  -item 521   //is not expanded and has no children -> test normal behaviour [do not add to list]

            Dictionary<int, bool>();
            Dictionary<int, bool> expectedState = expandLastMainNode
                ? new Dictionary<int, bool>
                {
                    { 4, true},
                    { 5, true},
                    { 51, false},
                    { 52, true},
                }
                : new Dictionary<int, bool>
                {
                    { 4, true},
                    { 5, false},
                };

            GenericTreeItem item4 = new(null) { Id = 4, Header = "item 4", IsExpanded = true };
            GenericTreeItem item41 = new(item4) { Id = 41, Header = "item 41", IsExpanded = true };
            item4.Children.Add(item41);

            GenericTreeItem item5 = new(null) { Id = 5, Header = "item 5", IsExpanded = expandLastMainNode };
            GenericTreeItem item51 = new(item5) { Id = 51, Header = "item 51", IsExpanded = false };
            GenericTreeItem item511 = new(item51) { Id = 511, Header = "item 511", IsExpanded = false };
            item51.Children.Add(item511);
            GenericTreeItem item52 = new(item5) { Id = 52, Header = "item 52", IsExpanded = expandLastMainNode };
            GenericTreeItem item521 = new(item52) { Id = 521, Header = "item 521", IsExpanded = false };
            item52.Children.Add(item521);
            item5.Children.Add(item51);
            item5.Children.Add(item52);

            List<GenericTreeItem> items =
            [
                new GenericTreeItem(null) { Id = 1, Header = "item 1", IsExpanded = true },
                new GenericTreeItem(null) { Id = 2, Header = "item 2", IsExpanded = false },
                new GenericTreeItem(null) { Id = 3, Header = "item 3", Children = [] },
                item4,
                item5
            ];

            return (items, expectedState);
        }

        /// <summary>
        /// Load a subset of current presets. Is only called once.
        /// </summary>
        /// <returns>A subset of the current presets.</returns>
        private static BuildingPresets InitSubsetFromPresetsFile()
        {
            BuildingPresetsLoader loader = new();
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            BuildingPresets buildingPresets = loader.Load(Path.Combine(basePath, CoreConstants.PresetsFiles.BuildingPresetsFile));

            List<BuildingInfo> buildings_1404 = buildingPresets.Buildings.Where(x => x.Header.StartsWith("(A4")).OrderByDescending(x => x.GetOrderParameter("eng")).Take(10).ToList();
            List<BuildingInfo> buildings_2070 = buildingPresets.Buildings.Where(x => x.Header.StartsWith("(A5")).OrderByDescending(x => x.GetOrderParameter("eng")).Take(10).ToList();
            List<BuildingInfo> buildings_2205 = buildingPresets.Buildings.Where(x => x.Header.StartsWith("(A6")).OrderByDescending(x => x.GetOrderParameter("eng")).Take(10).ToList();
            List<BuildingInfo> buildings_1800 = buildingPresets.Buildings.Where(x => x.Header.StartsWith("(A7")).OrderByDescending(x => x.GetOrderParameter("eng")).Take(10).ToList();

            List<BuildingInfo> filteredBuildings =
            [
                .. buildings_1404,
                .. buildings_2070,
                .. buildings_2205,
                .. buildings_1800,
            ];

            BuildingPresets presets = new()
            {
                Version = buildingPresets.Version,
                Buildings = filteredBuildings
            };

            return presets;
        }

        private static BuildingPresets InitSubsetForFiltering()
        {
            SerializableDictionary<string> locFireStation = new();
            locFireStation.Dict.Add("eng", "Fire Station");

            SerializableDictionary<string> locPoliceStation = new();
            locPoliceStation.Dict.Add("eng", "Police Station");

            SerializableDictionary<string> locBakery = new();
            locBakery.Dict.Add("eng", "Bakery");

            SerializableDictionary<string> locRiceFarm = new();
            locRiceFarm.Dict.Add("eng", "Rice Farm");

            SerializableDictionary<string> locRiceField = new();
            locRiceField.Dict.Add("eng", "Rice Field");

            List<BuildingInfo> buildings =
            [
                //Fire Station
                new BuildingInfo
                {
                    Header = "(A4) Anno 1404",
                    Faction = "Public",
                    Group = "Special",
                    Identifier = "FireStation",
                    Template = "SimpleBuilding",
                    Localization = locFireStation
                },
                //Police Station
                new BuildingInfo
                {
                    Header = "(A5) Anno 2070",
                    Faction = "Others",
                    Group = "Special",
                    Identifier = "police_station",
                    Template = "SupportBuilding",
                    Localization = locPoliceStation
                },
                new BuildingInfo
                {
                    Header = "(A6) Anno 2205",
                    Faction = "(1) Earth",
                    Group = "Public Buildings",
                    Identifier = "metro police",
                    Template = "CityInstitutionBuilding",
                    Localization = locPoliceStation
                },
                new BuildingInfo
                {
                    Header = "(A7) Anno 1800",
                    Faction = "(2) Workers",
                    Group = "Public Buildings",
                    Identifier = "Institution_01 (Police)",
                    Template = "CityInstitutionBuilding",
                    Localization = locPoliceStation
                },
                //Bakery
                new BuildingInfo
                {
                    Header = "(A4) Anno 1404",
                    Faction = "Production",
                    Group = "Factory",
                    Identifier = "Bakery",
                    Template = "FactoryBuilding",
                    Localization = locBakery
                },
                new BuildingInfo
                {
                    Header = "(A7) Anno 1800",
                    Faction = "(2) Workers",
                    Group = "Production Buildings",
                    Identifier = "Food_01 (Bread Maker)",
                    Template = "FactoryBuilding7",
                    Localization = locBakery
                },
                //Rice                
                new BuildingInfo
                {
                    Header = "(A6) Anno 2205",
                    Faction = "Facilities",
                    Group = "Agriculture",
                    Identifier = "production agriculture earth facility 01",
                    Template = "FactoryBuilding",
                    Localization = locRiceFarm
                },
                new BuildingInfo
                {
                    Header = "(A6) Anno 2205",
                    Faction = "Facility Modules",
                    Group = "Agriculture",
                    Identifier = "production agriculture earth facility module 01 tier 01",
                    Template = "BuildingModule",
                    Localization = locRiceField
                }
            ];

            BuildingPresets presets = new()
            {
                Version = "0.1",
                Buildings = buildings
            };

            return presets;
        }

        #endregion

        #region ctor tests

        [Fact]
        public void Ctor_ShouldSetDefaultValues()
        {
            // Arrange/Act
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            // Assert
            Assert.NotNull(viewModel.Items);
            Assert.NotNull(viewModel.FilteredItems);
            Assert.NotNull(viewModel.DoubleClickCommand);
            Assert.NotNull(viewModel.ReturnKeyPressedCommand);
            Assert.Equal(string.Empty, viewModel.BuildingPresetsVersion);
            Assert.Equal(CoreConstants.GameVersion.All, viewModel.FilterGameVersion);
            Assert.Equal(string.Empty, viewModel.FilterText);
        }

        #endregion

        #region LoadItems tests

        [Fact]
        public void LoadItems_BuildingPresetsIsNull_ShouldThrow()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            // Act/Assert
            _ = Assert.Throws<ArgumentNullException>(() => viewModel.LoadItems(null));
        }

        [Fact]
        public void LoadItems_BuildingPresetsContainsNoBuildings_ShouldLoadTwoRoadItems()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            BuildingPresets buildingPresets = new()
            {
                Buildings = []
            };

            // Act
            viewModel.LoadItems(buildingPresets);

            // Assert
            Assert.Equal(2, viewModel.Items.Count);
            Assert.True(viewModel.Items[0].AnnoObject.Road);
            Assert.True(viewModel.Items[1].AnnoObject.Road);
        }

        [Fact]
        public void LoadItems_ViewModelHasItems_ShouldClearItemsBeforeLoad()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            IEnumerable<GenericTreeItem> itemsToAdd = Enumerable.Repeat(new GenericTreeItem(null), 10);
            foreach (GenericTreeItem curItem in itemsToAdd)
            {
                viewModel.Items.Add(curItem);
            }

            BuildingPresets buildingPresets = new()
            {
                Buildings = []
            };

            // Act
            viewModel.LoadItems(buildingPresets);

            // Assert
            Assert.Equal(2, viewModel.Items.Count);
        }

        [Theory]
        [InlineData("1.2.5")]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("dummy")]
        public void LoadItems_BuildingPresetsPassed_ShouldSetVersionOfBuildingPresets(string versionToSet)
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            BuildingPresets buildingPresets = new()
            {
                Buildings = [],
                Version = versionToSet
            };

            // Act
            viewModel.LoadItems(buildingPresets);

            // Assert
            Assert.Equal(versionToSet, viewModel.BuildingPresetsVersion);
        }

        [Fact]
        public void LoadItems_VersionOfBuildingPresetsIsSet_ShouldRaisePropertyChangedEvent()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            BuildingPresets buildingPresets = new()
            {
                Buildings = []
            };

            // Act/Assert            
            Assert.PropertyChanged(viewModel, nameof(viewModel.BuildingPresetsVersion), () => viewModel.LoadItems(buildingPresets));
        }

        [Theory]
        [InlineData("", CoreConstants.GameVersion.Unknown)]
        [InlineData(" ", CoreConstants.GameVersion.Unknown)]
        [InlineData(null, CoreConstants.GameVersion.Unknown)]
        [InlineData("dummy", CoreConstants.GameVersion.Unknown)]
        [InlineData("(A4) Anno 1404", CoreConstants.GameVersion.Anno1404)]
        [InlineData("(A5) Anno 2070", CoreConstants.GameVersion.Anno2070)]
        [InlineData("(A6) Anno 2205", CoreConstants.GameVersion.Anno2205)]
        [InlineData("(A7) Anno 1800", CoreConstants.GameVersion.Anno1800)]
        public void LoadItems_BuildingsHaveHeader_ShouldSetCorrectGameVersion(string headerToSet, CoreConstants.GameVersion expectedGameVersion)
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            List<BuildingInfo> buildings =
            [
                new BuildingInfo
                {
                    Header = headerToSet
                }
            ];

            BuildingPresets buildingPresets = new()
            {
                Buildings = buildings
            };

            // Act
            viewModel.LoadItems(buildingPresets);

            // Assert
            //first 2 items always the road items
            Assert.Equal(expectedGameVersion, (viewModel.Items[2] as GameHeaderTreeItem).GameVersion);
        }

        [Fact]
        public void LoadItems_BuildingHeaderIsUnknownGameVersion_ShouldGetLocalizationForHeader()
        {
            // Arrange
            string expectedLocalization = "localized dummy";

            Mock<ILocalizationHelper> mockedLocalizationHelper = new();
            _ = mockedLocalizationHelper.Setup(x => x.GetLocalization(It.IsAny<string>())).Returns<string>(x => expectedLocalization);
            _ = mockedLocalizationHelper.Setup(x => x.GetLocalization(It.IsAny<string>(), It.IsAny<string>())).Returns((string value, string langauge) => expectedLocalization);

            PresetsTreeViewModel viewModel = new(mockedLocalizationHelper.Object, _mockedCommons);

            List<BuildingInfo> buildings =
            [
                new BuildingInfo
                {
                    Header = "dummy"
                }
            ];

            BuildingPresets buildingPresets = new()
            {
                Buildings = buildings
            };

            // Act
            viewModel.LoadItems(buildingPresets);

            // Assert
            //first 2 items always the road items
            Assert.Equal(expectedLocalization, (viewModel.Items[2] as GameHeaderTreeItem).Header);
        }

        [Theory]
        [InlineData("(A4) Anno 1404")]
        [InlineData("(A5) Anno 2070")]
        [InlineData("(A6) Anno 2205")]
        [InlineData("(A7) Anno 1800")]
        public void LoadItems_BuildingHeaderIsKnownGameVersion_ShouldNotGetLocalizationForHeader(string headerToSet)
        {
            // Arrange
            Mock<ILocalizationHelper> mockedLocalizationHelper = new();
            _ = mockedLocalizationHelper.Setup(x => x.GetLocalization(headerToSet)).Returns<string>(x => x);
            _ = mockedLocalizationHelper.Setup(x => x.GetLocalization(headerToSet, It.IsAny<string>())).Returns((string value, string langauge) => value);

            PresetsTreeViewModel viewModel = new(mockedLocalizationHelper.Object, _mockedCommons);

            List<BuildingInfo> buildings =
            [
                new BuildingInfo
                {
                    Header = headerToSet
                }
            ];

            BuildingPresets buildingPresets = new()
            {
                Buildings = buildings
            };

            // Act
            viewModel.LoadItems(buildingPresets);

            // Assert
            //first 2 items always the road items
            Assert.Equal(headerToSet, (viewModel.Items[2] as GameHeaderTreeItem).Header);
            mockedLocalizationHelper.Verify(x => x.GetLocalization(headerToSet), Times.Never());
            mockedLocalizationHelper.Verify(x => x.GetLocalization(headerToSet, It.IsAny<string>()), Times.Never());
        }

        [Theory]
        [InlineData("Ark")]
        [InlineData("DefColFace")]
        [InlineData("RoofColDef")]
        [InlineData("RoofColFace")]
        public void LoadItems_BuildingsHaveSpecialTemplate_ShouldNotLoadBuildings(string templateToSet)
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            List<BuildingInfo> buildings =
            [
                new BuildingInfo
                {
                    Header = "(A4) Anno 1404",
                    Template = templateToSet
                }
            ];

            BuildingPresets buildingPresets = new()
            {
                Buildings = buildings
            };

            // Act
            viewModel.LoadItems(buildingPresets);

            // Assert
            //there are always 2 road items
            Assert.Equal(2, viewModel.Items.Count);
        }

        [Theory]
        [InlineData("third party")]
        [InlineData("Facility Modules")]
        public void LoadItems_BuildingsHaveSpecialFaction_ShouldNotLoadBuildings(string factionToSet)
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            List<BuildingInfo> buildings =
            [
                new BuildingInfo
                {
                    Header = "(A4) Anno 1404",
                    Faction = factionToSet
                }
            ];

            BuildingPresets buildingPresets = new()
            {
                Buildings = buildings
            };

            // Act
            viewModel.LoadItems(buildingPresets);

            // Assert
            //there are always 2 road items
            Assert.Equal(2, viewModel.Items.Count);
        }

        [Fact]
        public void LoadItems_BuildingsHaveFaction_ShouldLoadBuildings()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            List<BuildingInfo> buildings =
            [
                new BuildingInfo
                {
                    Header = "(A4) Anno 1404",
                    Faction = "Workers",
                    Identifier = "A4_house"//required for GetOrderParameter
                }
            ];

            BuildingPresets buildingPresets = new()
            {
                Buildings = buildings
            };

            // Act
            viewModel.LoadItems(buildingPresets);

            // Assert
            //first 2 items always the road items
            _ = Assert.Single((viewModel.Items[2] as GameHeaderTreeItem).Children);
            Assert.Equal(buildings[0].Faction, (viewModel.Items[2] as GameHeaderTreeItem).Children[0].Header);
        }

        [Fact]
        public void LoadItems_Subset_ShouldLoadBuildings()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            // Act
            viewModel.LoadItems(_subsetFromPresetsFile);

            // Assert
            //2 road buildings + 4 game versions
            Assert.Equal(6, viewModel.Items.Count);
        }

        [Fact]
        public void LoadItems_SubsetForFiltering_ShouldLoadAllItemsAndAddExtraNodeForModulesIn2205()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            // Act
            viewModel.LoadItems(_subsetForFiltering);

            // Assert
            Assert.True(viewModel.Items[0].IsVisible);//first road tile
            Assert.True(viewModel.Items[1].IsVisible);//second road tile

            GenericTreeItem anno1404Node = viewModel.Items[2];
            Assert.Equal("(A4) Anno 1404", anno1404Node.Header);
            Assert.True(anno1404Node.IsVisible);
            GenericTreeItem productionNode = anno1404Node.Children.Single(x => x.Header.StartsWith("Production"));
            Assert.True(productionNode.IsVisible);
            GenericTreeItem factoryNode = productionNode.Children.Single(x => x.Header.StartsWith("Factory"));
            Assert.True(factoryNode.IsVisible);
            GenericTreeItem bakeryNode = factoryNode.Children.Single(x => x.Header.StartsWith("Bakery"));
            Assert.True(bakeryNode.IsVisible);
            GenericTreeItem publicNode = anno1404Node.Children.Single(x => x.Header.StartsWith("Public"));
            Assert.True(publicNode.IsVisible);
            GenericTreeItem specialNode = publicNode.Children.Single(x => x.Header.StartsWith("Special"));
            Assert.True(specialNode.IsVisible);
            GenericTreeItem fireStationNode = specialNode.Children.First(x => x.Header.StartsWith("Fire"));
            Assert.True(fireStationNode.IsVisible);

            GenericTreeItem anno2070Node = viewModel.Items[3];
            Assert.Equal("(A5) Anno 2070", anno2070Node.Header);
            Assert.True(anno2070Node.IsVisible);
            GenericTreeItem othersNode = anno2070Node.Children.Single(x => x.Header.StartsWith("Others"));
            Assert.True(othersNode.IsVisible);
            specialNode = othersNode.Children.Single(x => x.Header.StartsWith("Special"));
            Assert.True(specialNode.IsVisible);
            GenericTreeItem policeStationNode = specialNode.Children.First(x => x.Header.StartsWith("Police"));
            Assert.True(policeStationNode.IsVisible);

            GenericTreeItem anno2205Node = viewModel.Items[4];
            Assert.Equal("(A6) Anno 2205", anno2205Node.Header);
            Assert.True(anno2205Node.IsVisible);
            GenericTreeItem facilityNode = anno2205Node.Children.Single(x => x.Header.StartsWith("Facilities"));
            Assert.True(facilityNode.IsVisible);
            GenericTreeItem agricutureNode = facilityNode.Children.Single(x => x.Header.StartsWith("Agriculture"));
            Assert.True(agricutureNode.IsVisible);
            GenericTreeItem riceFarmNode = agricutureNode.Children.Single(x => x.Header.StartsWith("Rice Farm"));
            Assert.True(riceFarmNode.IsVisible);
            GenericTreeItem moduleNode = agricutureNode.Children.Single(x => x.Header.StartsWith("Agriculture Modules"));
            Assert.True(moduleNode.IsVisible);
            GenericTreeItem riceFieldNode = moduleNode.Children.Single(x => x.Header.StartsWith("Rice Field"));
            Assert.True(riceFieldNode.IsVisible);
            GenericTreeItem earthNode = anno2205Node.Children.Single(x => x.Header.StartsWith("(1) Earth"));
            Assert.True(earthNode.IsVisible);
            publicNode = earthNode.Children.Single(x => x.Header.StartsWith("Public"));
            Assert.True(publicNode.IsVisible);
            policeStationNode = publicNode.Children.First(x => x.Header.StartsWith("Police"));
            Assert.True(policeStationNode.IsVisible);

            GenericTreeItem anno1800Node = viewModel.Items[5];
            Assert.Equal("(A7) Anno 1800", anno1800Node.Header);
            Assert.True(anno1800Node.IsVisible);
            GenericTreeItem workersNode = anno1800Node.Children.Single(x => x.Header.StartsWith("(2) Workers"));
            Assert.True(workersNode.IsVisible);
            productionNode = workersNode.Children.Single(x => x.Header.StartsWith("Production"));
            Assert.True(productionNode.IsVisible);
            bakeryNode = productionNode.Children.Single(x => x.Header.StartsWith("Bakery"));
            Assert.True(bakeryNode.IsVisible);
            publicNode = workersNode.Children.Single(x => x.Header.StartsWith("Public"));
            Assert.True(publicNode.IsVisible);
            policeStationNode = publicNode.Children.First(x => x.Header.StartsWith("Police"));
            Assert.True(policeStationNode.IsVisible);
        }

        #endregion

        #region DoubleClick tests

        [Fact]
        public void DoubleClick_CommandParameterIsNull_ShouldNotSetSelectedItem()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            // Act
            viewModel.DoubleClickCommand.Execute(null);

            // Assert
            Assert.Null(viewModel.SelectedItem);
        }

        [Fact]
        public void DoubleClick_CommandParameterIsNull_ShouldNotRaiseApplySelectedItemEvent()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            // Act
            Exception ex = Record.Exception(() => Assert.Raises<EventArgs>(
                  x => viewModel.ApplySelectedItem += x,
                  x => viewModel.ApplySelectedItem -= x,
                  () => viewModel.DoubleClickCommand.Execute(null)));

            // Assert
            RaisesException exception = Assert.IsType<RaisesException>(ex);
            Assert.Equal("(No event was raised)", exception.Actual);
            Assert.Equal("EventArgs", exception.Expected);
        }

        [Fact]
        public void DoubleClick_CommandParameterHasNoAnnoObject_ShouldNotSetSelectedItem()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            GenericTreeItem commandParameter = new(null);

            // Act
            viewModel.DoubleClickCommand.Execute(commandParameter);

            // Assert
            Assert.Null(viewModel.SelectedItem);
        }

        [Fact]
        public void DoubleClick_CommandParameterHasNoAnnoObject_ShouldNotRaiseApplySelectedItemEvent()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            GenericTreeItem commandParameter = new(null);

            // Act
            Exception ex = Record.Exception(() => Assert.Raises<EventArgs>(
                  x => viewModel.ApplySelectedItem += x,
                  x => viewModel.ApplySelectedItem -= x,
                  () => viewModel.DoubleClickCommand.Execute(commandParameter)));

            // Assert
            RaisesException exception = Assert.IsType<RaisesException>(ex);
            Assert.Equal("(No event was raised)", exception.Actual);
            Assert.Equal("EventArgs", exception.Expected);
        }

        [Fact]
        public void DoubleClick_CommandParameterIsUnknownObject_ShouldNotSetSelectedItem()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            object commandParameter = new();

            // Act
            viewModel.DoubleClickCommand.Execute(commandParameter);

            // Assert
            Assert.Null(viewModel.SelectedItem);
        }

        [Fact]
        public void DoubleClick_CommandParameterIsUnknownObject_ShouldNotRaiseApplySelectedItemEvent()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            object commandParameter = new();

            // Act
            Exception ex = Record.Exception(() => Assert.Raises<EventArgs>(
                  x => viewModel.ApplySelectedItem += x,
                  x => viewModel.ApplySelectedItem -= x,
                  () => viewModel.DoubleClickCommand.Execute(commandParameter)));

            // Assert
            RaisesException exception = Assert.IsType<RaisesException>(ex);
            Assert.Equal("(No event was raised)", exception.Actual);
            Assert.Equal("EventArgs", exception.Expected);
        }

        [Fact]
        public void DoubleClick_CommandParameterHasAnnoObject_ShouldSetSelectedItem()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);
            AnnoObject annoObjectToSet = new();

            GenericTreeItem commandParameter = new(null)
            {
                AnnoObject = annoObjectToSet
            };

            // Act
            viewModel.DoubleClickCommand.Execute(commandParameter);

            // Assert
            Assert.Equal(commandParameter, viewModel.SelectedItem);
        }

        [Fact]
        public void DoubleClick_CommandParameterHasAnnoObject_ShouldRaiseApplySelectedItemEvent()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            AnnoObject annoObjectToSet = new();

            GenericTreeItem commandParameter = new(null)
            {
                AnnoObject = annoObjectToSet
            };

            // Act/Assert
            _ = Assert.Raises<EventArgs>(
                x => viewModel.ApplySelectedItem += x,
                x => viewModel.ApplySelectedItem -= x,
                () => viewModel.DoubleClickCommand.Execute(commandParameter));
        }

        #endregion

        #region ReturnKeyPressed tests

        [Fact]
        public void ReturnKeyPressed_CommandParameterIsNull_ShouldNotSetSelectedItem()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            // Act
            viewModel.ReturnKeyPressedCommand.Execute(null);

            // Assert
            Assert.Null(viewModel.SelectedItem);
        }

        [Fact]
        public void ReturnKeyPressed_CommandParameterIsNull_ShouldNotRaiseApplySelectedItemEvent()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            // Act
            Exception ex = Record.Exception(() => Assert.Raises<EventArgs>(
                  x => viewModel.ApplySelectedItem += x,
                  x => viewModel.ApplySelectedItem -= x,
                  () => viewModel.ReturnKeyPressedCommand.Execute(null)));

            // Assert
            RaisesException exception = Assert.IsType<RaisesException>(ex);
            Assert.Equal("(No event was raised)", exception.Actual);
            Assert.Equal("EventArgs", exception.Expected);
        }

        [Fact]
        public void ReturnKeyPressed_CommandParameterHasNoAnnoObject_ShouldNotSetSelectedItem()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            GenericTreeItem commandParameter = new(null);

            // Act
            viewModel.ReturnKeyPressedCommand.Execute(commandParameter);

            // Assert
            Assert.Null(viewModel.SelectedItem);
        }

        [Fact]
        public void ReturnKeyPressed_CommandParameterHasNoAnnoObject_ShouldNotRaiseApplySelectedItemEvent()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            GenericTreeItem commandParameter = new(null);

            // Act
            Exception ex = Record.Exception(() => Assert.Raises<EventArgs>(
                  x => viewModel.ApplySelectedItem += x,
                  x => viewModel.ApplySelectedItem -= x,
                  () => viewModel.ReturnKeyPressedCommand.Execute(commandParameter)));

            // Assert
            RaisesException exception = Assert.IsType<RaisesException>(ex);
            Assert.Equal("(No event was raised)", exception.Actual);
            Assert.Equal("EventArgs", exception.Expected);
        }

        [Fact]
        public void ReturnKeyPressed_CommandParameterIsUnknownObject_ShouldNotSetSelectedItem()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            object commandParameter = new();

            // Act
            viewModel.ReturnKeyPressedCommand.Execute(commandParameter);

            // Assert
            Assert.Null(viewModel.SelectedItem);
        }

        [Fact]
        public void ReturnKeyPressed_CommandParameterIsUnknownObject_ShouldNotRaiseApplySelectedItemEvent()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            object commandParameter = new();

            // Act
            Exception ex = Record.Exception(() => Assert.Raises<EventArgs>(
                  x => viewModel.ApplySelectedItem += x,
                  x => viewModel.ApplySelectedItem -= x,
                  () => viewModel.ReturnKeyPressedCommand.Execute(commandParameter)));

            // Assert
            RaisesException exception = Assert.IsType<RaisesException>(ex);
            Assert.Equal("(No event was raised)", exception.Actual);
            Assert.Equal("EventArgs", exception.Expected);
        }

        [Fact]
        public void ReturnKeyPressed_CommandParameterHasAnnoObject_ShouldSetSelectedItem()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);
            AnnoObject annoObjectToSet = new();

            GenericTreeItem commandParameter = new(null)
            {
                AnnoObject = annoObjectToSet
            };

            // Act
            viewModel.ReturnKeyPressedCommand.Execute(commandParameter);

            // Assert
            Assert.Equal(commandParameter, viewModel.SelectedItem);
        }

        [Fact]
        public void ReturnKeyPressed_CommandParameterHasAnnoObject_ShouldRaiseApplySelectedItemEvent()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            AnnoObject annoObjectToSet = new();

            GenericTreeItem commandParameter = new(null)
            {
                AnnoObject = annoObjectToSet
            };

            // Act/Assert
            _ = Assert.Raises<EventArgs>(
                x => viewModel.ApplySelectedItem += x,
                x => viewModel.ApplySelectedItem -= x,
                () => viewModel.ReturnKeyPressedCommand.Execute(commandParameter));
        }

        #endregion

        #region GetCondensedTreeState tests

        [Fact]
        public void GetCondensedTreeState_ItemsAreEmpty_ShouldReturnEmptyList()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            // Act
            Dictionary<int, bool> result = viewModel.GetCondensedTreeState();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetCondensedTreeState_ItemsExpanded_ShouldReturnCorrectList()
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            (List<GenericTreeItem> items, Dictionary<int, bool> expectedState) = GetTreeAndState();

            viewModel.Items = new ObservableCollection<GenericTreeItem>(items);

            // Act
            Dictionary<int, bool> result = viewModel.GetCondensedTreeState();

            // Assert
            Assert.Equal(expectedState, result);
        }

        #endregion

        #region SetCondensedTreeState tests

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void SetCondensedTreeState_LastPresetsVersionIsNullOrWhiteSpace_ShouldNotSetAnyStateAndNotThrow(string lastBuildingPresetsVersionToSet)
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);

            (List<GenericTreeItem> items, Dictionary<int, bool> expectedState) = GetTreeAndState();
            viewModel.Items = new ObservableCollection<GenericTreeItem>(items);

            // Act
            viewModel.SetCondensedTreeState([], lastBuildingPresetsVersionToSet);

            // Assert
            Assert.Equal(expectedState, viewModel.GetCondensedTreeState());
        }

        [Fact]
        public void SetCondensedTreeState_LastPresetsVersionIsDifferent_ShouldNotSetAnyStateAndNotThrow()
        {
            // Arrange
            string buildingPresetsVersion = "1.0";

            BuildingPresets buildingPresets = new()
            {
                Buildings = [],
                Version = buildingPresetsVersion
            };

            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);
            viewModel.LoadItems(buildingPresets);

            (List<GenericTreeItem> items, Dictionary<int, bool> expectedState) = GetTreeAndState();
            viewModel.Items = new ObservableCollection<GenericTreeItem>(items);

            // Act
            viewModel.SetCondensedTreeState([], "2.0");

            // Assert
            Assert.Equal(expectedState, viewModel.GetCondensedTreeState());
        }

        [Fact]
        public void SetCondensedTreeState_SavedTreeStateIsNull_ShouldNotSetAnyStateAndNotThrow()
        {
            // Arrange
            string buildingPresetsVersion = "1.0";

            BuildingPresets buildingPresets = new()
            {
                Buildings = [],
                Version = buildingPresetsVersion
            };

            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);
            viewModel.LoadItems(buildingPresets);

            (List<GenericTreeItem> items, Dictionary<int, bool> expectedState) = GetTreeAndState();
            viewModel.Items = new ObservableCollection<GenericTreeItem>(items);

            // Act
            viewModel.SetCondensedTreeState(null, buildingPresetsVersion);

            // Assert
            Assert.Equal(expectedState, viewModel.GetCondensedTreeState());
        }

        [Fact]
        public void SetCondensedTreeState_SavedTreeStateIsEmpty_ShouldNotSetAnyStateAndNotThrow()
        {
            // Arrange
            string buildingPresetsVersion = "1.0";

            BuildingPresets buildingPresets = new()
            {
                Buildings = [],
                Version = buildingPresetsVersion
            };

            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);
            viewModel.LoadItems(buildingPresets);

            (List<GenericTreeItem> items, Dictionary<int, bool> expectedState) = GetTreeAndState();
            viewModel.Items = new ObservableCollection<GenericTreeItem>(items);

            // Act
            viewModel.SetCondensedTreeState([], buildingPresetsVersion);

            // Assert
            Assert.Equal(expectedState, viewModel.GetCondensedTreeState());
        }

        [Fact]
        public void SetCondensedTreeState_SavedTreeStateIsCompatible_ShouldSetTreeState()
        {
            // Arrange
            string buildingPresetsVersion = "1.0";

            BuildingPresets buildingPresets = new()
            {
                Buildings = [],
                Version = buildingPresetsVersion
            };

            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);
            viewModel.LoadItems(buildingPresets);
            (List<GenericTreeItem> items, Dictionary<int, bool> _) = GetTreeAndState(expandLastMainNode: false);
            viewModel.Items.Clear();
            foreach (GenericTreeItem curItem in items)
            {
                viewModel.Items.Add(curItem);
            }

            Dictionary<int, bool> savedTreeState = new()
            {
                { 4, true },
                { 5, false }
            };

            // Act
            viewModel.SetCondensedTreeState(savedTreeState, buildingPresetsVersion);

            // Assert
            Dictionary<int, bool> currentState = viewModel.GetCondensedTreeState();
            Assert.Equal(savedTreeState, currentState);
        }

        #endregion

        #region FilterGameVersion tests

        [Theory]
        [InlineData(CoreConstants.GameVersion.Anno1404, 1)]
        [InlineData(CoreConstants.GameVersion.Anno2070, 1)]
        [InlineData(CoreConstants.GameVersion.Anno2205, 1)]
        [InlineData(CoreConstants.GameVersion.Anno1800, 1)]
        [InlineData(CoreConstants.GameVersion.Anno1404 | CoreConstants.GameVersion.Anno1800, 2)]
        [InlineData(CoreConstants.GameVersion.Anno1404 | CoreConstants.GameVersion.Anno1800 | CoreConstants.GameVersion.Anno2205, 3)]
        [InlineData(CoreConstants.GameVersion.All, 4)]
        [InlineData(CoreConstants.GameVersion.Unknown, 0)]
        public void FilterGameVersion_SubsetIsLoadedAndFilterTextIsEmpty_ShouldFilterByGameVersion(CoreConstants.GameVersion gameVersionsToFilter, int expectedMainNodeCount)
        {
            // Arrange
            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);
            viewModel.LoadItems(_subsetFromPresetsFile);
            viewModel.FilterText = string.Empty;

            //make sure to really trigger the filter method
            viewModel.FilterGameVersion = viewModel.FilterGameVersion != CoreConstants.GameVersion.Unknown
                ? CoreConstants.GameVersion.Unknown
                : CoreConstants.GameVersion.All;

            // Act
            viewModel.FilterGameVersion = gameVersionsToFilter;

            // Assert
            //+ 2 road buildings
            Assert.Equal(2 + expectedMainNodeCount, viewModel.Items.Where(x => x.IsVisible).Count());
        }

        #endregion

        #region FilterText tests

        [Fact]
        public void FilterText_SearchForFireAndFilterGameVersionIsAll_ShouldOnlyShowFireStationAndExpandParents()
        {
            // Arrange
            string filterText = "Fire";

            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);
            viewModel.LoadItems(_subsetForFiltering);
            viewModel.FilterText = string.Empty;
            viewModel.FilterGameVersion = CoreConstants.GameVersion.All;

            // Act
            viewModel.FilterText = filterText;

            // Assert
            Assert.False(viewModel.Items[0].IsVisible);//first road tile
            Assert.False(viewModel.Items[1].IsVisible);//second road tile

            GenericTreeItem anno1404Node = viewModel.Items[2];
            Assert.Equal("(A4) Anno 1404", anno1404Node.Header);
            Assert.True(anno1404Node.IsVisible);
            Assert.True(anno1404Node.IsExpanded);
            Assert.False(anno1404Node.Children.Single(x => x.Header.StartsWith("Production")).IsVisible);
            GenericTreeItem publicNode = anno1404Node.Children.Single(x => x.Header.StartsWith("Public"));
            Assert.True(publicNode.IsVisible);
            Assert.True(publicNode.IsExpanded);
            GenericTreeItem specialNode = publicNode.Children.Single(x => x.Header.StartsWith("Special"));
            Assert.True(specialNode.IsVisible);
            Assert.True(specialNode.IsExpanded);
            GenericTreeItem fireStationNode = specialNode.Children.First(x => x.Header.StartsWith("Fire"));
            Assert.True(fireStationNode.IsVisible);

            //all other nodes should not be visible
            GenericTreeItem anno2070Node = viewModel.Items[3];
            Assert.Equal("(A5) Anno 2070", anno2070Node.Header);
            Assert.False(anno2070Node.IsVisible);
            GenericTreeItem anno2205Node = viewModel.Items[4];
            Assert.Equal("(A6) Anno 2205", anno2205Node.Header);
            Assert.False(anno2205Node.IsVisible);
            GenericTreeItem anno1800Node = viewModel.Items[5];
            Assert.Equal("(A7) Anno 1800", anno1800Node.Header);
            Assert.False(anno1800Node.IsVisible);
        }

        [Fact]
        public void FilterText_SearchForPoliceAndFilterGameVersionIsAll_ShouldOnlyShowPoliceStationAndExpandParents()
        {
            // Arrange
            string filterText = "Police";

            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);
            viewModel.LoadItems(_subsetForFiltering);
            viewModel.FilterText = string.Empty;
            viewModel.FilterGameVersion = CoreConstants.GameVersion.All;

            // Act
            viewModel.FilterText = filterText;

            // Assert
            Assert.False(viewModel.Items[0].IsVisible);//first road tile
            Assert.False(viewModel.Items[1].IsVisible);//second road tile

            GenericTreeItem anno1404Node = viewModel.Items[2];
            Assert.Equal("(A4) Anno 1404", anno1404Node.Header);
            Assert.False(anno1404Node.IsVisible);

            GenericTreeItem anno2070Node = viewModel.Items[3];
            Assert.Equal("(A5) Anno 2070", anno2070Node.Header);
            Assert.True(anno2070Node.IsVisible);
            Assert.True(anno2070Node.IsExpanded);
            GenericTreeItem othersNode = anno2070Node.Children.Single(x => x.Header.StartsWith("Others"));
            Assert.True(othersNode.IsVisible);
            Assert.True(othersNode.IsExpanded);
            GenericTreeItem specialNode = othersNode.Children.Single(x => x.Header.StartsWith("Special"));
            Assert.True(specialNode.IsVisible);
            Assert.True(specialNode.IsExpanded);
            GenericTreeItem policeStationNode = specialNode.Children.First(x => x.Header.StartsWith("Police"));
            Assert.True(policeStationNode.IsVisible);

            GenericTreeItem anno2205Node = viewModel.Items[4];
            Assert.Equal("(A6) Anno 2205", anno2205Node.Header);
            Assert.True(anno2205Node.IsVisible);
            Assert.True(anno2205Node.IsExpanded);
            Assert.False(anno2205Node.Children.Single(x => x.Header.StartsWith("Facilities")).IsVisible);
            GenericTreeItem earthNode = anno2205Node.Children.Single(x => x.Header.StartsWith("(1) Earth"));
            Assert.True(earthNode.IsVisible);
            Assert.True(earthNode.IsExpanded);
            GenericTreeItem publicNode = earthNode.Children.Single(x => x.Header.StartsWith("Public"));
            Assert.True(publicNode.IsVisible);
            Assert.True(publicNode.IsExpanded);
            policeStationNode = publicNode.Children.First(x => x.Header.StartsWith("Police"));
            Assert.True(policeStationNode.IsVisible);

            GenericTreeItem anno1800Node = viewModel.Items[5];
            Assert.Equal("(A7) Anno 1800", anno1800Node.Header);
            Assert.True(anno1800Node.IsVisible);
            Assert.True(anno1800Node.IsExpanded);
            GenericTreeItem workersNode = anno1800Node.Children.Single(x => x.Header.StartsWith("(2) Workers"));
            Assert.True(workersNode.IsVisible);
            Assert.True(workersNode.IsExpanded);
            Assert.False(workersNode.Children.Single(x => x.Header.StartsWith("Production")).IsVisible);
            publicNode = workersNode.Children.Single(x => x.Header.StartsWith("Public"));
            Assert.True(publicNode.IsVisible);
            Assert.True(publicNode.IsExpanded);
            policeStationNode = publicNode.Children.First(x => x.Header.StartsWith("Police"));
            Assert.True(policeStationNode.IsVisible);
        }

        [Fact]
        public void FilterText_SearchForPoliceAndFilterGameVersionIs2070_ShouldOnlyShowPoliceStationAndExpandParentsForAnno2070()
        {
            // Arrange
            string filterText = "Police";

            PresetsTreeViewModel viewModel = new(_mockedTreeLocalization, _mockedCommons);
            viewModel.LoadItems(_subsetForFiltering);
            viewModel.FilterText = string.Empty;
            viewModel.FilterGameVersion = CoreConstants.GameVersion.Anno2070;

            // Act
            viewModel.FilterText = filterText;

            // Assert
            Assert.False(viewModel.Items[0].IsVisible);//first road tile
            Assert.False(viewModel.Items[1].IsVisible);//second road tile

            GenericTreeItem anno1404Node = viewModel.Items[2];
            Assert.Equal("(A4) Anno 1404", anno1404Node.Header);
            Assert.False(anno1404Node.IsVisible);

            GenericTreeItem anno2070Node = viewModel.Items[3];
            Assert.Equal("(A5) Anno 2070", anno2070Node.Header);
            Assert.True(anno2070Node.IsVisible);
            Assert.True(anno2070Node.IsExpanded);
            GenericTreeItem othersNode = anno2070Node.Children.Single(x => x.Header.StartsWith("Others"));
            Assert.True(othersNode.IsVisible);
            Assert.True(othersNode.IsExpanded);
            GenericTreeItem specialNode = othersNode.Children.Single(x => x.Header.StartsWith("Special"));
            Assert.True(specialNode.IsVisible);
            Assert.True(specialNode.IsExpanded);
            GenericTreeItem policeStationNode = specialNode.Children.First(x => x.Header.StartsWith("Police"));
            Assert.True(policeStationNode.IsVisible);

            //all other nodes should not be visible
            GenericTreeItem anno2205Node = viewModel.Items[4];
            Assert.Equal("(A6) Anno 2205", anno2205Node.Header);
            Assert.False(anno2205Node.IsVisible);
            GenericTreeItem anno1800Node = viewModel.Items[5];
            Assert.Equal("(A7) Anno 1800", anno1800Node.Header);
            Assert.False(anno1800Node.IsVisible);
        }

        #endregion
    }
}
