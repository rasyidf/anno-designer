using AnnoDesigner.Core.Converters;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Xunit;

namespace AnnoDesigner.Core.Tests
{
    public class ModifierKeysToVisibilityConverterTests
    {
        #region Convert tests

        [Theory]
        [InlineData(ModifierKeys.Control, Visibility.Visible)]
        [InlineData(ModifierKeys.Alt, Visibility.Visible)]
        [InlineData(ModifierKeys.Shift, Visibility.Visible)]
        [InlineData(ModifierKeys.Windows, Visibility.Visible)]
        [InlineData(ModifierKeys.None, Visibility.Collapsed)]
        [InlineData(ModifierKeys.Control | ModifierKeys.Alt, Visibility.Visible)]
        public void Convert_PassedKnownValue_ShouldReturnCorrectValue(ModifierKeys input, Visibility expected)
        {
            // Arrange
            ModifierKeysToVisibilityConverter converter = new();

            // Act
            object result = converter.Convert(input, typeof(Visibility), null, CultureInfo.CurrentCulture);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(ModifierKeys.Control, Visibility.Visible)]
        [InlineData(ModifierKeys.Alt, Visibility.Visible)]
        [InlineData(ModifierKeys.Shift, Visibility.Visible)]
        [InlineData(ModifierKeys.Windows, Visibility.Visible)]
        [InlineData(ModifierKeys.None, Visibility.Collapsed)]
        [InlineData(ModifierKeys.Control | ModifierKeys.Alt, Visibility.Visible)]
        public void Convert_PassedParameterValue_ShouldReturnCorrectValue(ModifierKeys input, Visibility expected)
        {
            // Arrange
            ModifierKeysToVisibilityConverter converter = new();

            // Act
            object result = converter.Convert(null, typeof(Visibility), input, CultureInfo.CurrentCulture);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Convert_PassedUnknownValueAndUnknownParameter_ShouldReturnNull()
        {
            // Arrange
            ModifierKeysToVisibilityConverter converter = new();

            // Act
            object result = converter.Convert(null, typeof(Visibility), null, CultureInfo.CurrentCulture);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region ConvertBack tests

        [Fact]
        public void ConvertBack_PassedAnyValue_ShouldThrow()
        {
            // Arrange
            ModifierKeysToVisibilityConverter converter = new();

            // Act/Assert
            _ = Assert.Throws<NotImplementedException>(() => converter.ConvertBack(ModifierKeys.Control, typeof(Visibility), null, CultureInfo.CurrentCulture));
        }

        #endregion
    }
}
