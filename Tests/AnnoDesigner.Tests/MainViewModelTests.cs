using AnnoDesigner.Core;
using AnnoDesigner.Core.Helper;
using AnnoDesigner.Core.Layout.Models;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Core.Presets.Models;
using AnnoDesigner.Core.RecentFiles;
using AnnoDesigner.Core.Services;
using AnnoDesigner.Helper;
using AnnoDesigner.Models;
using AnnoDesigner.Undo;
using AnnoDesigner.Undo.Operations;
using AnnoDesigner.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Xunit;

namespace AnnoDesigner.Tests
{
    public class MainViewModelTests
    {
        private readonly ICommons _mockedCommons;
        private readonly IAppSettings _mockedAppSettings;
        private readonly IAnnoCanvas _mockedAnnoCanvas;
        private readonly IRecentFilesHelper _inMemoryRecentFilesHelper;
        private readonly IMessageBoxService _mockedMessageBoxService;
        private readonly ILocalizationHelper _mockedLocalizationHelper;
        private readonly IUpdateHelper _mockedUpdateHelper;
        private readonly IFileSystem _mockedFileSystem;
        private readonly IAdjacentCellGrouper _mockedCellGrouper;

        public MainViewModelTests()
        {
            _mockedFileSystem = new MockFileSystem();

            Mock<ICommons> commonsMock = new();
            _ = commonsMock.SetupGet(x => x.CurrentLanguage).Returns(() => "English");
            _ = commonsMock.SetupGet(x => x.CurrentLanguageCode).Returns(() => "eng");
            _ = commonsMock.SetupGet(x => x.LanguageCodeMap).Returns(() => []);
            _mockedCommons = commonsMock.Object;

            Mock<ILocalizationHelper> mockedLocalizationHelper = new();
            _ = mockedLocalizationHelper.Setup(x => x.GetLocalization(It.IsAny<string>())).Returns<string>(x => x);
            _ = mockedLocalizationHelper.Setup(x => x.GetLocalization(It.IsAny<string>(), It.IsAny<string>())).Returns((string value, string langauge) => value);
            _mockedLocalizationHelper = mockedLocalizationHelper.Object;

            Localization.Localization.Init(_mockedCommons);

            _mockedAppSettings = new Mock<IAppSettings>().Object;

            Mock<IAnnoCanvas> annoCanvasMock = new();
            _ = annoCanvasMock.SetupAllProperties();
            //The QuadTree does not have a default constructor, so we need to explicitly set up the property
            _ = annoCanvasMock.SetupGet(x => x.UndoManager).Returns(Mock.Of<IUndoManager>());
            _ = annoCanvasMock.Setup(x => x.PlacedObjects).Returns(new Core.DataStructures.QuadTree<LayoutObject>(new Rect(-100, -100, 200, 200)));
            _mockedAnnoCanvas = annoCanvasMock.Object;

            _inMemoryRecentFilesHelper = new RecentFilesHelper(new RecentFilesInMemorySerializer(), new MockFileSystem());

            _mockedMessageBoxService = new Mock<IMessageBoxService>().Object;
            _mockedUpdateHelper = new Mock<IUpdateHelper>().Object;
            _mockedCellGrouper = Mock.Of<IAdjacentCellGrouper>();
        }

        private MainViewModel GetViewModel(ICommons commonsToUse = null,
            IAppSettings appSettingsToUse = null,
            IRecentFilesHelper recentFilesHelperToUse = null,
            IMessageBoxService messageBoxServiceToUse = null,
            IUpdateHelper updateHelperToUse = null,
            ILocalizationHelper localizationHelperToUse = null,
            IAnnoCanvas annoCanvasToUse = null,
            IFileSystem fileSystemToUse = null,
            IAdjacentCellGrouper adjacentCellGrouperToUse = null,
            ILayoutLoader layoutLoaderToUse = null)
        {
            return new MainViewModel(commonsToUse ?? _mockedCommons,
                appSettingsToUse ?? _mockedAppSettings,
                recentFilesHelperToUse ?? _inMemoryRecentFilesHelper,
                messageBoxServiceToUse ?? _mockedMessageBoxService,
                updateHelperToUse ?? _mockedUpdateHelper,
                localizationHelperToUse ?? _mockedLocalizationHelper,
                fileSystemToUse ?? _mockedFileSystem,
                adjacentCellGrouper: adjacentCellGrouperToUse ?? _mockedCellGrouper,
                layoutLoaderToUse: layoutLoaderToUse)
            {
                AnnoCanvas = annoCanvasToUse ?? _mockedAnnoCanvas
            };
        }

        #region ctor tests

        [Fact]
        public void Ctor_ShouldSetDefaultValues()
        {
            // Arrange/Act
            MainViewModel viewModel = GetViewModel();

            // Assert
            Assert.NotNull(viewModel.OpenProjectHomepageCommand);
            Assert.NotNull(viewModel.CloseWindowCommand);
            Assert.NotNull(viewModel.CanvasResetZoomCommand);
            Assert.NotNull(viewModel.CanvasNormalizeCommand);
            Assert.NotNull(viewModel.MergeRoadsCommand);
            Assert.NotNull(viewModel.LoadLayoutFromJsonCommand);
            Assert.NotNull(viewModel.UnregisterExtensionCommand);
            Assert.NotNull(viewModel.RegisterExtensionCommand);
            Assert.NotNull(viewModel.ExportImageCommand);
            Assert.NotNull(viewModel.CopyLayoutToClipboardCommand);
            Assert.NotNull(viewModel.LanguageSelectedCommand);
            Assert.NotNull(viewModel.ShowAboutWindowCommand);
            Assert.NotNull(viewModel.ShowWelcomeWindowCommand);
            Assert.NotNull(viewModel.PreferencesUpdateViewModel.CheckForUpdatesCommand);
            Assert.NotNull(viewModel.ShowStatisticsCommand);
            Assert.NotNull(viewModel.ShowStatisticsBuildingCountCommand);
            Assert.NotNull(viewModel.PlaceBuildingCommand);

            Assert.NotNull(viewModel.StatisticsViewModel);
            Assert.NotNull(viewModel.BuildingSettingsViewModel);
            Assert.NotNull(viewModel.PresetsTreeViewModel);
            Assert.NotNull(viewModel.PresetsTreeSearchViewModel);
            Assert.NotNull(viewModel.WelcomeViewModel);
            Assert.NotNull(viewModel.AboutViewModel);
            Assert.NotNull(viewModel.PreferencesUpdateViewModel);

            Assert.False(viewModel.CanvasShowGrid);
            Assert.False(viewModel.CanvasShowIcons);
            Assert.False(viewModel.CanvasShowLabels);
            Assert.False(viewModel.UseCurrentZoomOnExportedImageValue);
            Assert.False(viewModel.RenderSelectionHighlightsOnExportedImageValue);
            Assert.False(viewModel.IsLanguageChange);
            Assert.False(viewModel.IsBusy);

            Assert.Null(viewModel.StatusMessage);

            Assert.NotNull(viewModel.AvailableIcons);
            Assert.NotNull(viewModel.SelectedIcon);
            Assert.NotNull(viewModel.Languages);
            Assert.NotNull(viewModel.MainWindowTitle);
            Assert.NotNull(viewModel.PresetsSectionHeader);

            Assert.Equal(Constants.Version.ToString(), viewModel.PreferencesUpdateViewModel.VersionValue);
            Assert.Equal(CoreConstants.LayoutFileVersion.ToString("0.#", CultureInfo.InvariantCulture), viewModel.PreferencesUpdateViewModel.FileVersionValue);
        }

        #endregion

        #region CloseWindowCommand tests

        [Fact]
        public void CloseWindowCommand_ShouldCanExecute()
        {
            // Arrange
            MainViewModel viewModel = GetViewModel();

            // Act
            bool result = viewModel.CloseWindowCommand.CanExecute(null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CloseWindowCommand_IsExecutedWithNull_ShouldNotThrow()
        {
            // Arrange
            MainViewModel viewModel = GetViewModel();

            // Act
            Exception ex = Record.Exception(() => viewModel.CloseWindowCommand.Execute(null));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void CloseWindowCommand_IsExecutedWithICloseable_ShouldCallClose()
        {
            // Arrange
            MainViewModel viewModel = GetViewModel();
            Mock<ICloseable> mockedCloseable = new();

            // Act
            viewModel.CloseWindowCommand.Execute(mockedCloseable.Object);

            // Assert
            mockedCloseable.Verify(x => x.Close(), Times.Once);
        }

        #endregion

        #region SearchText tests

        [Fact]
        public void PresetsTreeSearchViewModelPropertyChanged_SearchTextChanged_ShouldSetFilterTextOnPresetsTreeViewModel()
        {
            // Arrange
            MainViewModel viewModel = GetViewModel();
            viewModel.PresetsTreeViewModel.FilterText = "Lorem";

            string textToSet = "dummy";

            // Act
            viewModel.PresetsTreeSearchViewModel.SearchText = textToSet;

            // Assert
            Assert.Equal(textToSet, viewModel.PresetsTreeViewModel.FilterText);
        }

        #endregion

        #region ShowStatisticsCommand tests

        [Fact]
        public void ShowStatisticsCommand_IsExecuted_ShouldRaiseShowStatisticsChangedEvent()
        {
            // Arrange
            MainViewModel viewModel = GetViewModel();

            // Act/Assert
            _ = Assert.Raises<EventArgs>(
                x => viewModel.ShowStatisticsChanged += x,
                x => viewModel.ShowStatisticsChanged -= x,
                () => viewModel.ShowStatisticsCommand.Execute(null));
        }

        #endregion

        #region SaveSettings tests

        [Fact]
        public void SaveSettings_IsCalled_ShouldSaveMainWindowHeight()
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            double expectedMainWindowHeight = 42.4;

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.MainWindowHeight = expectedMainWindowHeight;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedMainWindowHeight, appSettings.Object.MainWindowHeight);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Fact]
        public void SaveSettings_IsCalled_ShouldSaveMainWindowWidth()
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            double expectedMainMainWindowWidth = 42.4;

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.MainWindowWidth = expectedMainMainWindowWidth;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedMainMainWindowWidth, appSettings.Object.MainWindowWidth);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Fact]
        public void SaveSettings_IsCalled_ShouldSaveMainWindowLeft()
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            double expectedMainMainWindowLeft = 42.4;

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.MainWindowLeft = expectedMainMainWindowLeft;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedMainMainWindowLeft, appSettings.Object.MainWindowLeft);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Fact]
        public void SaveSettings_IsCalled_ShouldSaveMainWindowTop()
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            double expectedMainMainWindowTop = 42.4;

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.MainWindowTop = expectedMainMainWindowTop;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedMainMainWindowTop, appSettings.Object.MainWindowTop);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(System.Windows.WindowState.Maximized)]
        [InlineData(System.Windows.WindowState.Normal)]
        public void SaveSettings_IsCalled_ShouldSaveMainWindowWindowState(System.Windows.WindowState expectedMainMainWindowWindowState)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.MainWindowWindowState = expectedMainMainWindowWindowState;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedMainMainWindowWindowState, appSettings.Object.MainWindowWindowState);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SaveSettings_IsCalled_ShouldSaveShowGrid(bool expectedShowGrid)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.CanvasShowGrid = expectedShowGrid;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedShowGrid, appSettings.Object.ShowGrid);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SaveSettings_IsCalled_ShouldSaveShowIcons(bool expectedShowIcons)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.CanvasShowIcons = expectedShowIcons;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedShowIcons, appSettings.Object.ShowIcons);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SaveSettings_IsCalled_ShouldSaveShowLabels(bool expectedShowLabels)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.CanvasShowLabels = expectedShowLabels;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedShowLabels, appSettings.Object.ShowLabels);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SaveSettings_IsCalled_ShouldSaveShowInfluences(bool expectedShowInfluences)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.CanvasShowInfluences = expectedShowInfluences;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedShowInfluences, appSettings.Object.ShowInfluences);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SaveSettings_IsCalled_ShouldSaveShowTrueInfluenceRange(bool expectedShowTrueInfluenceRange)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.CanvasShowTrueInfluenceRange = expectedShowTrueInfluenceRange;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedShowTrueInfluenceRange, appSettings.Object.ShowTrueInfluenceRange);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SaveSettings_IsCalled_ShouldSaveEnableAutomaticUpdateCheck(bool expectedEnableAutomaticUpdateCheck)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.PreferencesUpdateViewModel.AutomaticUpdateCheck = expectedEnableAutomaticUpdateCheck;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedEnableAutomaticUpdateCheck, appSettings.Object.EnableAutomaticUpdateCheck);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SaveSettings_IsCalled_ShouldSaveUseCurrentZoomOnExportedImageValue(bool expectedUseCurrentZoomOnExportedImageValue)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.UseCurrentZoomOnExportedImageValue = expectedUseCurrentZoomOnExportedImageValue;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedUseCurrentZoomOnExportedImageValue, appSettings.Object.UseCurrentZoomOnExportedImageValue);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SaveSettings_IsCalled_ShouldSaveRenderSelectionHighlightsOnExportedImageValue(bool expectedRenderSelectionHighlightsOnExportedImageValue)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.RenderSelectionHighlightsOnExportedImageValue = expectedRenderSelectionHighlightsOnExportedImageValue;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedRenderSelectionHighlightsOnExportedImageValue, appSettings.Object.RenderSelectionHighlightsOnExportedImageValue);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SaveSettings_IsCalled_ShouldSaveRenderVersionOnExportedImageValue(bool expectedRenderVersionOnExportedImageValue)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.RenderVersionOnExportedImageValue = expectedRenderVersionOnExportedImageValue;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedRenderVersionOnExportedImageValue, appSettings.Object.RenderVersionOnExportedImageValue);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SaveSettings_IsCalled_ShouldSaveIsPavedStreet(bool expectedIsPavedStreet)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            BuildingPresets presets = new()
            {
                Buildings = []
            };

            Mock<IAnnoCanvas> canvas = new();
            _ = canvas.SetupGet(x => x.BuildingPresets).Returns(() => presets);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object, annoCanvasToUse: canvas.Object);
            viewModel.BuildingSettingsViewModel.IsPavedStreet = expectedIsPavedStreet;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedIsPavedStreet, appSettings.Object.IsPavedStreet);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SaveSettings_IsCalled_ShouldSaveStatsShowStats(bool expectedStatsShowStats)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.StatisticsViewModel.IsVisible = expectedStatsShowStats;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedStatsShowStats, appSettings.Object.StatsShowStats);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SaveSettings_IsCalled_ShouldSaveStatsShowBuildingCount(bool expectedStatsShowBuildingCount)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.StatisticsViewModel.ShowStatisticsBuildingCount = expectedStatsShowBuildingCount;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedStatsShowBuildingCount, appSettings.Object.StatsShowBuildingCount);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        //[InlineData("")] Skip = "needs abstraction of 'AnnoCanvas' in MainViewModel"
        //[InlineData(" ")] Skip = "needs abstraction of 'AnnoCanvas' in MainViewModel"
        //[InlineData(null)] Skip = "needs abstraction of 'AnnoCanvas' in MainViewModel"
        [InlineData("lorem")]
        public void SaveSettings_IsCalled_ShouldSaveTreeViewSearchText(string expectedTreeViewSearchText)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.PresetsTreeSearchViewModel.SearchText = expectedTreeViewSearchText;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedTreeViewSearchText, appSettings.Object.TreeViewSearchText);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(CoreConstants.GameVersion.All)]
        [InlineData(CoreConstants.GameVersion.Anno1404)]
        [InlineData(CoreConstants.GameVersion.Anno1800)]
        [InlineData(CoreConstants.GameVersion.Anno2070)]
        [InlineData(CoreConstants.GameVersion.Anno2205)]
        [InlineData(CoreConstants.GameVersion.Unknown)]
        [InlineData(CoreConstants.GameVersion.Anno1404 | CoreConstants.GameVersion.Anno1800)]
        [InlineData(CoreConstants.GameVersion.Anno2070 | CoreConstants.GameVersion.Anno2205 | CoreConstants.GameVersion.Anno1800)]
        public void SaveSettings_IsCalled_ShouldSavePresetsTreeGameVersionFilter(CoreConstants.GameVersion expectedPresetsTreeGameVersionFilter)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.PresetsTreeViewModel.FilterGameVersion = expectedPresetsTreeGameVersionFilter;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedPresetsTreeGameVersionFilter.ToString(), appSettings.Object.PresetsTreeGameVersionFilter);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData("id", Key.A, ModifierKeys.Alt, Key.S, ModifierKeys.Control | ModifierKeys.Shift, @"{""id"":{""Key"":""S"",""MouseAction"":""None"",""Modifiers"":""Control, Shift"",""Type"":""KeyGesture""}}")]
        [InlineData("id", Key.A, ModifierKeys.Alt, Key.A, ModifierKeys.Alt, "{}")]
        public void SaveSettings_IsCalled_ShouldSaveRemappedHotkeys(string id, Key key, ModifierKeys modifiers, Key newKey, ModifierKeys newModifiers, string expectedJsonString)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            ICommand command = Mock.Of<ICommand>(c => c.CanExecute(It.IsAny<object>()) == true);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.HotkeyCommandManager.AddHotkey(id, new InputBinding(command, new PolyGesture(key, modifiers)));
            Hotkey hotkey = viewModel.HotkeyCommandManager.GetHotkey("id");
            PolyGesture gesture = hotkey.Binding.Gesture as PolyGesture;
            gesture.Key = newKey;
            gesture.ModifierKeys = newModifiers;

            // Act
            viewModel.SaveSettings();

            // Assert
            Assert.Equal(expectedJsonString, appSettings.Object.HotkeyMappings);
            appSettings.Verify(x => x.Save(), Times.AtLeastOnce);
        }
        #endregion

        #region LoadSettings tests

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LoadSettings_IsCalled_ShouldLoadShowInfluences(bool expectedShowInfluences)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();
            _ = appSettings.Setup(x => x.ShowInfluences).Returns(() => expectedShowInfluences);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.CanvasShowInfluences = !expectedShowInfluences;

            // Act
            viewModel.LoadSettings();

            // Assert
            Assert.Equal(expectedShowInfluences, viewModel.CanvasShowInfluences);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LoadSettings_IsCalled_ShouldLoadShowGrid(bool expectedShowGrid)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();
            _ = appSettings.Setup(x => x.ShowGrid).Returns(() => expectedShowGrid);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.CanvasShowGrid = !expectedShowGrid;

            // Act
            viewModel.LoadSettings();

            // Assert
            Assert.Equal(expectedShowGrid, viewModel.CanvasShowGrid);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LoadSettings_IsCalled_ShouldLoadShowIcons(bool expectedShowIcons)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();
            _ = appSettings.Setup(x => x.ShowIcons).Returns(() => expectedShowIcons);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.CanvasShowIcons = !expectedShowIcons;

            // Act
            viewModel.LoadSettings();

            // Assert
            Assert.Equal(expectedShowIcons, viewModel.CanvasShowIcons);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LoadSettings_IsCalled_ShouldLoadShowLabels(bool expectedShowLabels)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();
            _ = appSettings.Setup(x => x.ShowLabels).Returns(() => expectedShowLabels);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.CanvasShowLabels = !expectedShowLabels;

            // Act
            viewModel.LoadSettings();

            // Assert
            Assert.Equal(expectedShowLabels, viewModel.CanvasShowLabels);
        }

        [Fact]
        public void LoadSettings_IsCalled_ShouldLoadMainWindowHeight()
        {
            // Arrange            
            double expectedMainWindowHeight = 42.4;

            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();
            _ = appSettings.Setup(x => x.MainWindowHeight).Returns(() => expectedMainWindowHeight);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);

            // Act
            viewModel.LoadSettings();

            // Assert
            Assert.Equal(expectedMainWindowHeight, viewModel.MainWindowHeight);
        }

        [Fact]
        public void LoadSettings_IsCalled_ShouldLoadMainWindowWidth()
        {
            // Arrange            
            double expectedMainWindowWidth = 42.4;

            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();
            _ = appSettings.Setup(x => x.MainWindowWidth).Returns(() => expectedMainWindowWidth);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);

            // Act
            viewModel.LoadSettings();

            // Assert
            Assert.Equal(expectedMainWindowWidth, viewModel.MainWindowWidth);
        }

        [Fact]
        public void LoadSettings_IsCalled_ShouldLoadMainWindowLeft()
        {
            // Arrange            
            double expectedMainWindowLeft = 42.4;

            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();
            _ = appSettings.Setup(x => x.MainWindowLeft).Returns(() => expectedMainWindowLeft);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);

            // Act
            viewModel.LoadSettings();

            // Assert
            Assert.Equal(expectedMainWindowLeft, viewModel.MainWindowLeft);
        }

        [Fact]
        public void LoadSettings_IsCalled_ShouldLoadMainWindowTop()
        {
            // Arrange            
            double expectedMainWindowTop = 42.4;

            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();
            _ = appSettings.Setup(x => x.MainWindowTop).Returns(() => expectedMainWindowTop);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);

            // Act
            viewModel.LoadSettings();

            // Assert
            Assert.Equal(expectedMainWindowTop, viewModel.MainWindowTop);
        }

        [Theory]
        [InlineData(System.Windows.WindowState.Maximized)]
        [InlineData(System.Windows.WindowState.Normal)]
        public void LoadSettings_IsCalled_ShouldLoadMainWindowWindowState(System.Windows.WindowState expectedMainMainWindowWindowState)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();
            _ = appSettings.Setup(x => x.MainWindowWindowState).Returns(() => expectedMainMainWindowWindowState);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);

            // Act
            viewModel.LoadSettings();

            // Assert
            Assert.Equal(expectedMainMainWindowWindowState, viewModel.MainWindowWindowState);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LoadSettings_IsCalled_ShouldLoadEnableAutomaticUpdateCheck(bool expectedEnableAutomaticUpdateCheck)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();
            _ = appSettings.Setup(x => x.EnableAutomaticUpdateCheck).Returns(() => expectedEnableAutomaticUpdateCheck);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.PreferencesUpdateViewModel.AutomaticUpdateCheck = !expectedEnableAutomaticUpdateCheck;

            // Act
            viewModel.LoadSettings();

            // Assert
            Assert.Equal(expectedEnableAutomaticUpdateCheck, viewModel.PreferencesUpdateViewModel.AutomaticUpdateCheck);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LoadSettings_IsCalled_ShouldLoadUseCurrentZoomOnExportedImageValue(bool expectedUseCurrentZoomOnExportedImageValue)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();
            _ = appSettings.Setup(x => x.UseCurrentZoomOnExportedImageValue).Returns(() => expectedUseCurrentZoomOnExportedImageValue);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.UseCurrentZoomOnExportedImageValue = !expectedUseCurrentZoomOnExportedImageValue;

            // Act
            viewModel.LoadSettings();

            // Assert
            Assert.Equal(expectedUseCurrentZoomOnExportedImageValue, viewModel.UseCurrentZoomOnExportedImageValue);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LoadSettings_IsCalled_ShouldLoadRenderSelectionHighlightsOnExportedImageValue(bool expectedRenderSelectionHighlightsOnExportedImageValue)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();
            _ = appSettings.Setup(x => x.RenderSelectionHighlightsOnExportedImageValue).Returns(() => expectedRenderSelectionHighlightsOnExportedImageValue);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.RenderSelectionHighlightsOnExportedImageValue = !expectedRenderSelectionHighlightsOnExportedImageValue;

            // Act
            viewModel.LoadSettings();

            // Assert
            Assert.Equal(expectedRenderSelectionHighlightsOnExportedImageValue, viewModel.RenderSelectionHighlightsOnExportedImageValue);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void LoadSettings_IsCalled_ShouldLoadRenderVersionOnExportedImageValue(bool expectedRenderVersionOnExportedImageValue)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();
            _ = appSettings.Setup(x => x.RenderVersionOnExportedImageValue).Returns(() => expectedRenderVersionOnExportedImageValue);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            viewModel.RenderVersionOnExportedImageValue = !expectedRenderVersionOnExportedImageValue;

            // Act
            viewModel.LoadSettings();

            // Assert
            Assert.Equal(expectedRenderVersionOnExportedImageValue, viewModel.RenderVersionOnExportedImageValue);
        }

        [Theory]
        [InlineData("id", Key.A, ModifierKeys.Alt, Key.S, ModifierKeys.Control | ModifierKeys.Shift, ExtendedMouseAction.None, GestureType.KeyGesture, @"{""id"":{""Key"":62,""MouseAction"":0,""Modifiers"":6,""Type"":1}}")]
        [InlineData("id", Key.A, ModifierKeys.Alt, Key.None, ModifierKeys.Shift, ExtendedMouseAction.LeftDoubleClick, GestureType.MouseGesture, @"{""id"":{""Key"":0,""MouseAction"":5,""Modifiers"":4,""Type"":0}}")]
        [InlineData("id", Key.A, ModifierKeys.Alt, Key.A, ModifierKeys.Alt, ExtendedMouseAction.None, GestureType.KeyGesture, "{}")]
        public void LoadSettings_IsCalled_ShouldLoadRemappedHotkeys(string id, Key key, ModifierKeys modifiers, Key expectedKey, ModifierKeys expectedModifiers, ExtendedMouseAction expectedMouseAction, GestureType expectedType, string settingsString)
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            ICommand command = Mock.Of<ICommand>(c => c.CanExecute(It.IsAny<object>()) == true);

            MainViewModel viewModel = GetViewModel(null, appSettings.Object);
            _ = appSettings.Setup(x => x.HotkeyMappings).Returns(settingsString);

            // Act
            viewModel.LoadSettings();

            viewModel.HotkeyCommandManager.AddHotkey(id, new InputBinding(command, new PolyGesture(key, modifiers)));
            PolyGesture gesture = viewModel.HotkeyCommandManager.GetHotkey(id).Binding.Gesture as PolyGesture;
            // Assert
            Assert.Equal(expectedKey, gesture.Key);
            Assert.Equal(expectedModifiers, gesture.ModifierKeys);
            Assert.Equal(expectedMouseAction, gesture.MouseAction);
            Assert.Equal(expectedType, gesture.Type);
        }

        #endregion

        #region LanguageSelectedCommand tests

        [Fact]
        public void LanguageSelectedCommand_IsExecutedWithNull_ShouldNotThrow()
        {
            // Arrange
            MainViewModel viewModel = GetViewModel();

            // Act
            Exception ex = Record.Exception(() => viewModel.LanguageSelectedCommand.Execute(null));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void LanguageSelectedCommand_IsExecutedWithUnknownDataType_ShouldNotThrow()
        {
            // Arrange
            MainViewModel viewModel = GetViewModel();

            // Act
            Exception ex = Record.Exception(() => viewModel.LanguageSelectedCommand.Execute(42));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void LanguageSelectedCommand_AlreadyIsLanguageChange_ShouldNotSetLanguage()
        {
            // Arrange
            string languageBeforeChange = "English";

            Mock<ICommons> commons = new();
            _ = commons.SetupAllProperties();
            _ = commons.SetupGet(x => x.LanguageCodeMap).Returns(() => []);
            commons.Object.CurrentLanguage = languageBeforeChange;

            MainViewModel viewModel = GetViewModel(commons.Object, null);
            viewModel.IsLanguageChange = true;

            SupportedLanguage languageToSet = new("Deutsch");

            // Act
            Exception ex = Record.Exception(() => viewModel.LanguageSelectedCommand.Execute(languageToSet));

            // Assert
            Assert.Null(ex);
            Assert.Equal(languageBeforeChange, commons.Object.CurrentLanguage);
        }

        [Fact]
        public void LanguageSelectedCommand_IsExecutedWithSupportedLanguage_ShouldSetLanguage()
        {
            // Arrange
            string languageBeforeChange = "English";

            Mock<ICommons> commons = new();
            _ = commons.SetupAllProperties();
            _ = commons.SetupGet(x => x.LanguageCodeMap).Returns(() => []);
            commons.Object.CurrentLanguage = languageBeforeChange;

            MainViewModel viewModel = GetViewModel(commons.Object, null);

            SupportedLanguage languageToSet = new("Deutsch");

            // Act
            viewModel.LanguageSelectedCommand.Execute(languageToSet);

            // Assert
            Assert.Equal(languageToSet.Name, commons.Object.CurrentLanguage);
        }

        [Fact]
        public void LanguageSelectedCommand_IsExecutedWithSupportedLanguage_ShouldSetIsLanguageChangeToFalse()
        {
            // Arrange
            string languageBeforeChange = "English";

            Mock<ICommons> commons = new();
            _ = commons.SetupAllProperties();
            _ = commons.SetupGet(x => x.LanguageCodeMap).Returns(() => []);
            commons.Object.CurrentLanguage = languageBeforeChange;

            MainViewModel viewModel = GetViewModel(commons.Object, null);

            SupportedLanguage languageToSet = new("Deutsch");

            // Act
            viewModel.LanguageSelectedCommand.Execute(languageToSet);

            // Assert
            Assert.Equal(languageToSet.Name, commons.Object.CurrentLanguage);
            Assert.False(viewModel.IsLanguageChange);
        }

        #endregion

        #region MergeRoads tests

        [Fact]
        public void MergeRoads_IsCalled_RoadColorAndBorderIsPreserved()
        {
            // Arrange
            List<LayoutObject> roads =
            [
                new LayoutObject(
                    new AnnoObject()
                    {
                        Position = new Point(1, 1),
                        Size = new Size(1, 1),
                        Road = true,
                        Color = new SerializableColor(255, 0, 0, 0),
                        Borderless = false
                    }, null, null, null),
                new LayoutObject(
                    new AnnoObject()
                    {
                        Position = new Point(2, 1),
                        Size = new Size(1, 1),
                        Road = true,
                        Color = new SerializableColor(255, 0, 0, 0),
                        Borderless = true
                    }, null, null, null),
                new LayoutObject(
                    new AnnoObject()
                    {
                        Position = new Point(3, 1),
                        Size = new Size(1, 1),
                        Road = true,
                        Color = new SerializableColor(255, 100, 100, 100),
                        Borderless = true
                    }, null, null, null)
            ];
            _mockedAnnoCanvas.PlacedObjects.AddRange(roads);
            Mock<IAdjacentCellGrouper> cellGrouper = new();

            MainViewModel viewModel = GetViewModel(adjacentCellGrouperToUse: cellGrouper.Object);

            // Act
            viewModel.MergeRoads(null);

            // Assert
            cellGrouper.Verify(g => g.GroupAdjacentCells(It.IsAny<LayoutObject[][]>(), It.IsAny<bool>()), Times.Never);
            Assert.Equal(3, _mockedAnnoCanvas.PlacedObjects.Count());
        }

        #endregion

        #region SaveFile tests

        [Fact]
        public void SaveFile_LayoutVersionChangedByUser_ShouldUseLayoutVersion()
        {
            // Arrange
            LayoutFile layoutFileUsedToSave = null;
            Version versionToSave = new(42, 42, 42, 42);

            Mock<ILayoutLoader> mockedLayoutLoader = new();
            _ = mockedLayoutLoader.Setup(x => x.SaveLayout(It.IsAny<LayoutFile>(), It.IsAny<string>()))
                .Callback<LayoutFile, string>((layout, filePath) => layoutFileUsedToSave = layout);

            MainViewModel viewModel = GetViewModel(layoutLoaderToUse: mockedLayoutLoader.Object);
            viewModel.LayoutSettingsViewModel.LayoutVersion = versionToSave;

            // Act
            viewModel.SaveFile("dummy");

            // Assert
            mockedLayoutLoader.Verify(x => x.SaveLayout(It.IsAny<LayoutFile>(), It.IsAny<string>()), Times.Once());
            Assert.Equal(versionToSave, layoutFileUsedToSave.LayoutVersion);
        }

        [Fact]
        public void SaveFile_ShouldNormalizeCanvas()
        {
            // Arrange
            int calledBorderWidth = -1;
            Mock<IAnnoCanvas> mockedCanvas = new();
            _ = mockedCanvas.Setup(x => x.Normalize(It.IsAny<int>()))
                .Callback<int>(x => calledBorderWidth = x);

            Mock<ILayoutLoader> mockedLayoutLoader = new();

            MainViewModel viewModel = GetViewModel(annoCanvasToUse: mockedCanvas.Object, layoutLoaderToUse: mockedLayoutLoader.Object);

            // Act
            viewModel.SaveFile("dummy");

            // Assert
            mockedCanvas.Verify(x => x.Normalize(It.IsAny<int>()), Times.Once());
            Assert.Equal(1, calledBorderWidth);
        }

        [Fact]
        public void SaveFile_ExceptionIsRaised_ShouldShowError()
        {
            // Arrange
            Exception expectedException = new("This should raise.");
            Mock<ILayoutLoader> mockedLayoutLoader = new();
            _ = mockedLayoutLoader.Setup(x => x.SaveLayout(It.IsAny<LayoutFile>(), It.IsAny<string>())).Throws(expectedException);

            Mock<IMessageBoxService> mockedMessageBoxService = new();

            MainViewModel viewModel = GetViewModel(layoutLoaderToUse: mockedLayoutLoader.Object,
                messageBoxServiceToUse: mockedMessageBoxService.Object);

            // Act
            viewModel.SaveFile("dummy");

            // Assert
            mockedMessageBoxService.Verify(x => x.ShowError(It.IsAny<object>(), expectedException.Message, It.IsAny<string>()), Times.Once());
        }

        #endregion

        #region 

        [Fact]
        public void LayoutSettingsViewModel_SettingLayoutVersion_ShouldBeConsideredUnsavedChange()
        {
            // Arrange
            Version versionToSave = new(42, 42, 42, 42);

            Mock<IAnnoCanvas> mockedAnnoCanvas = new();
            Mock<IUndoManager> mockedUndoManager = new();
            _ = mockedAnnoCanvas.SetupAllProperties();
            _ = mockedAnnoCanvas.SetupGet(x => x.UndoManager).Returns(mockedUndoManager.Object);
            MainViewModel viewModel = GetViewModel(annoCanvasToUse: mockedAnnoCanvas.Object);

            // Act
            viewModel.LayoutSettingsViewModel.LayoutVersion = versionToSave;

            // Assert
            mockedUndoManager.Verify(x => x.RegisterOperation(It.Is<ModifyLayoutVersionOperation>(y => y.NewValue == new Version(42, 42, 42, 42))));
        }

        #endregion
    }
}
