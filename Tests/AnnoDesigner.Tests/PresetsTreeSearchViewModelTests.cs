using AnnoDesigner.Models;
using AnnoDesigner.ViewModels;
using System.Collections.ObjectModel;
using Xunit;

namespace AnnoDesigner.Tests
{
    public class PresetsTreeSearchViewModelTests
    {
        #region ctor tests

        [Fact]
        public void Ctor_ShouldSetDefaultValues()
        {
            // Arrange/Act
            PresetsTreeSearchViewModel viewModel = new();

            // Assert
            Assert.False(viewModel.HasFocus);
            Assert.Empty(viewModel.SearchText);
            Assert.NotNull(viewModel.ClearSearchTextCommand);
            Assert.NotNull(viewModel.GotFocusCommand);
            Assert.NotNull(viewModel.LostFocusCommand);
            Assert.NotNull(viewModel.GameVersionFilterChangedCommand);
        }

        #endregion

        #region GotFocusCommand tests

        [Fact]
        public void GotFocusCommand_ShouldCanExecute()
        {
            // Arrange
            PresetsTreeSearchViewModel viewModel = new();

            // Act
            bool result = viewModel.GotFocusCommand.CanExecute(null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GotFocusCommand_IsExecuted_ShouldSetHasFocusTrue()
        {
            // Arrange
            PresetsTreeSearchViewModel viewModel = new()
            {
                HasFocus = false
            };

            // Act
            viewModel.GotFocusCommand.Execute(null);

            // Assert
            Assert.True(viewModel.HasFocus);
        }

        #endregion

        #region LostFocusCommand tests

        [Fact]
        public void LostFocusCommand_ShouldCanExecute()
        {
            // Arrange
            PresetsTreeSearchViewModel viewModel = new();

            // Act
            bool result = viewModel.LostFocusCommand.CanExecute(null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void LostFocusCommand_IsExecuted_ShouldSetHasFocusFalse()
        {
            // Arrange
            PresetsTreeSearchViewModel viewModel = new()
            {
                HasFocus = true
            };

            // Act
            viewModel.LostFocusCommand.Execute(null);

            // Assert
            Assert.False(viewModel.HasFocus);
        }

        #endregion

        #region ClearSearchTextCommand tests

        [Fact]
        public void ClearSearchTextCommand_ShouldCanExecute()
        {
            // Arrange
            PresetsTreeSearchViewModel viewModel = new();

            // Act
            bool result = viewModel.ClearSearchTextCommand.CanExecute(null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ClearSearchTextCommand_IsExecuted_ShouldSetSearchTextEmpty()
        {
            // Arrange
            PresetsTreeSearchViewModel viewModel = new()
            {
                SearchText = "dummy"
            };

            // Act
            viewModel.ClearSearchTextCommand.Execute(null);

            // Assert
            Assert.Empty(viewModel.SearchText);
        }

        #endregion

        #region GameVersionFilterChangedCommand  tests

        [Fact]
        public void GameVersionFilterChangedCommand_IsExecutedWithGameVersionFilter_ShouldNegateIsSelected()
        {
            // Arrange
            PresetsTreeSearchViewModel viewModel = new();

            GameVersionFilter gameVersionFilter = new()
            {
                IsSelected = true
            };

            // Act
            viewModel.GameVersionFilterChangedCommand.Execute(gameVersionFilter);

            // Assert
            Assert.False(gameVersionFilter.IsSelected);
        }

        [Fact]
        public void GameVersionFilterChangedCommand_IsExecuted_ShouldRaisePropertyChanged()
        {
            // Arrange
            PresetsTreeSearchViewModel viewModel = new();

            // Act/Assert
            Assert.PropertyChanged(viewModel,
                nameof(PresetsTreeSearchViewModel.SelectedGameVersionFilters),
                () => viewModel.GameVersionFilterChangedCommand.Execute(null));
        }

        #endregion

        #region SelectedGameVersionFilters  tests

        [Fact]
        public void SelectedGameVersionFilters_IsCalled_ShouldReturnCorrectCollection()
        {
            // Arrange
            PresetsTreeSearchViewModel viewModel = new();
            viewModel.GameVersionFilters[0].IsSelected = true;

            // Act
            ObservableCollection<GameVersionFilter> result = viewModel.SelectedGameVersionFilters;

            // Assert
            _ = Assert.Single(result);
        }

        #endregion

        #region SelectedGameVersions  tests

        [Fact]
        public void SelectedGameVersions_IsCalled_ShouldSetIsSelectedForCorrectItems()
        {
            // Arrange
            PresetsTreeSearchViewModel viewModel = new();
            viewModel.GameVersionFilters[0].IsSelected = false;
            viewModel.GameVersionFilters[1].IsSelected = false;

            // Act
            viewModel.SelectedGameVersions = viewModel.GameVersionFilters[0].Type | viewModel.GameVersionFilters[1].Type;

            // Assert
            Assert.True(viewModel.GameVersionFilters[0].IsSelected);
            Assert.True(viewModel.GameVersionFilters[1].IsSelected);
        }

        #endregion
    }
}
