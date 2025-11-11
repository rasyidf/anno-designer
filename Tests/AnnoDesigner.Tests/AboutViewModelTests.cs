using AnnoDesigner.Core.Models;
using AnnoDesigner.ViewModels;
using Moq;
using System;
using Xunit;

namespace AnnoDesigner.Tests
{
    public class AboutViewModelTests
    {
        private AboutViewModel GetViewModel()
        {
            return new AboutViewModel();
        }

        #region CloseWindowCommand tests

        [Fact(Skip = "needs abstraction for localization")]
        public void CloseWindowCommand_IsExecutedWithICloseable_ShouldCallClose()
        {
            // Arrange
            AboutViewModel viewModel = GetViewModel();

            Mock<ICloseable> closeable = new();

            // Act
            viewModel.CloseWindowCommand.Execute(closeable.Object);

            // Assert
            closeable.Verify(x => x.Close(), Times.Once);
        }

        [Fact(Skip = "needs abstraction for localization")]
        public void CloseWindowCommand_IsExecutedWithoutICloseable_ShouldNotThrow()
        {
            // Arrange
            AboutViewModel viewModel = GetViewModel();

            // Act
            Exception ex = Record.Exception(() => viewModel.CloseWindowCommand.Execute(null));

            // Assert
            Assert.Null(ex);
        }

        #endregion
    }
}
