using AnnoDesigner.Core.Converters;
using System.Globalization;
using System.Windows;
using Xunit;

namespace AnnoDesigner.Core.Tests
{
    public class BoolToVisibilityConverterTests
    {
        #region ctor tests

        [Fact]
        public void ctor_Defaults_Set()
        {
            // Arrange/Act
            BoolToVisibilityConverter converter = new();

            // Assert
            Assert.Equal(Visibility.Visible, converter.TrueValue);
            Assert.Equal(Visibility.Collapsed, converter.FalseValue);
        }

        #endregion

        #region Convert tests

        [Fact]
        public void Convert_PassedTrue_ShouldReturnVisible()
        {
            // Arrange/Act
            BoolToVisibilityConverter converter = new();

            // Act
            object result = converter.Convert(true, typeof(Visibility), null, CultureInfo.CurrentCulture);

            // Assert
            Assert.Equal(Visibility.Visible, result);
        }

        [Fact]
        public void Convert_PassedFalse_ShouldReturnCollapsed()
        {
            // Arrange/Act
            BoolToVisibilityConverter converter = new();

            // Act
            object result = converter.Convert(false, typeof(Visibility), null, CultureInfo.CurrentCulture);

            // Assert
            Assert.Equal(Visibility.Collapsed, result);
        }

        [Fact]
        public void Convert_ChangedFalsValue_ShouldReturnSetValue()
        {
            // Arrange/Act
            BoolToVisibilityConverter converter = new()
            {
                FalseValue = Visibility.Hidden
            };

            // Act
            object result = converter.Convert(false, typeof(Visibility), null, CultureInfo.CurrentCulture);

            // Assert
            Assert.Equal(Visibility.Hidden, result);
        }

        [Fact]
        public void Convert_PassedUnknownValue_ShouldReturnNull()
        {
            // Arrange/Act
            BoolToVisibilityConverter converter = new();

            // Act
            object result = converter.Convert(string.Empty, typeof(Visibility), null, CultureInfo.CurrentCulture);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region ConvertBack tests

        [Fact]
        public void ConvertBack_PassedTrueValue_ShouldReturnTrue()
        {
            // Arrange/Act
            BoolToVisibilityConverter converter = new();

            // Act
            bool result = (bool)converter.ConvertBack(Visibility.Visible, typeof(bool), null, CultureInfo.CurrentCulture);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ConvertBack_PassedFalseValue_ShouldReturnFalse()
        {
            // Arrange/Act
            BoolToVisibilityConverter converter = new();

            // Act
            bool result = (bool)converter.ConvertBack(Visibility.Collapsed, typeof(bool), null, CultureInfo.CurrentCulture);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ConvertBack_PassedUnknownValue_ShouldReturnNull()
        {
            // Arrange/Act
            BoolToVisibilityConverter converter = new();

            // Act
            object result = converter.ConvertBack(Visibility.Hidden, typeof(bool), null, CultureInfo.CurrentCulture);

            // Assert
            Assert.Null(result);
        }

        #endregion
    }
}
