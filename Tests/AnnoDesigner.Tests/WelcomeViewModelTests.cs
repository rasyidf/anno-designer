using AnnoDesigner.Core.Models;
using AnnoDesigner.Models;
using AnnoDesigner.ViewModels;
using Moq;
using System;
using Xunit;

namespace AnnoDesigner.Tests
{
    public class WelcomeViewModelTests
    {
        private readonly ICommons _mockedCommons;
        private readonly IAppSettings _mockedAppSettings;

        public WelcomeViewModelTests()
        {
            _mockedCommons = new Mock<ICommons>().Object;
            _mockedAppSettings = new Mock<IAppSettings>().Object;
        }

        private WelcomeViewModel GetViewModel(ICommons commonsToUse = null, IAppSettings appSettingsToUse = null)
        {
            return new WelcomeViewModel(commonsToUse ?? _mockedCommons, appSettingsToUse ?? _mockedAppSettings);
        }

        #region ctor tests

        [Fact]
        public void Ctor_ShouldSetDefaultValues()
        {
            // Arrange/Act
            WelcomeViewModel viewModel = GetViewModel();

            // Assert
            Assert.Null(viewModel.SelectedItem);
            Assert.NotNull(viewModel.ContinueCommand);
            Assert.NotNull(viewModel.Languages);
        }

        [Fact]
        public void Ctor_ShouldSetCorrectNumberOfLanguages()
        {
            // Arrange/Act
            WelcomeViewModel viewModel = GetViewModel();

            // Assert
            Assert.Equal(6, viewModel.Languages.Count);
        }

        #endregion

        #region ContinueCommand tests

        [Fact]
        public void ContinueCommand_SelectedItemIsNull_ShouldNotCanExecute()
        {
            // Arrange
            WelcomeViewModel viewModel = GetViewModel();

            // Act
            bool result = viewModel.ContinueCommand.CanExecute(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ContinueCommand_SelectedItemIsNotNull_ShouldCanExecute()
        {
            // Arrange
            WelcomeViewModel viewModel = GetViewModel();
            viewModel.SelectedItem = viewModel.Languages[0];

            // Act
            bool result = viewModel.ContinueCommand.CanExecute(null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ContinueCommand_IsExecuted_ShouldSetSelectedLanguageInCommons()
        {
            // Arrange
            Mock<ICommons> commons = new();
            _ = commons.SetupAllProperties();

            WelcomeViewModel viewModel = GetViewModel(commons.Object);
            viewModel.SelectedItem = viewModel.Languages[1];

            string expectedLanguage = viewModel.SelectedItem.Name;

            // Act
            viewModel.ContinueCommand.Execute(null);

            // Assert
            Assert.Equal(expectedLanguage, commons.Object.CurrentLanguage);
        }

        [Fact]
        public void ContinueCommand_IsExecuted_ShouldSetSelectedLanguageInAppSettings()
        {
            // Arrange            
            Mock<ICommons> commons = new();
            _ = commons.SetupAllProperties();

            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            WelcomeViewModel viewModel = GetViewModel(commons.Object, appSettings.Object);
            viewModel.SelectedItem = viewModel.Languages[1];

            string expectedLanguage = viewModel.SelectedItem.Name;

            // Act
            viewModel.ContinueCommand.Execute(null);

            // Assert
            Assert.Equal(expectedLanguage, appSettings.Object.SelectedLanguage);
        }

        [Fact]
        public void ContinueCommand_IsExecuted_ShouldSaveAppSettings()
        {
            // Arrange            
            Mock<IAppSettings> appSettings = new();
            _ = appSettings.SetupAllProperties();

            WelcomeViewModel viewModel = GetViewModel(_mockedCommons, appSettings.Object);
            viewModel.SelectedItem = viewModel.Languages[1];

            string expectedLanguage = viewModel.SelectedItem.Name;

            // Act
            viewModel.ContinueCommand.Execute(null);

            // Assert
            appSettings.Verify(x => x.Save(), Times.Once);
        }

        [Fact]
        public void ContinueCommand_IsExecutedWithICloseable_ShouldCallClose()
        {
            // Arrange
            WelcomeViewModel viewModel = GetViewModel();
            viewModel.SelectedItem = viewModel.Languages[1];

            Mock<ICloseable> closeable = new();

            // Act
            viewModel.ContinueCommand.Execute(closeable.Object);

            // Assert
            closeable.Verify(x => x.Close(), Times.Once);
        }

        [Fact]
        public void ContinueCommand_IsExecutedWithoutICloseable_ShouldNotThrow()
        {
            // Arrange
            WelcomeViewModel viewModel = GetViewModel();
            viewModel.SelectedItem = viewModel.Languages[1];

            // Act
            Exception ex = Record.Exception(() => viewModel.ContinueCommand.Execute(null));

            // Assert
            Assert.Null(ex);
        }

        #endregion
    }
}
