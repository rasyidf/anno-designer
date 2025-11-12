using AnnoDesigner.Core.Models;
using AnnoDesigner.Helper;
using AnnoDesigner.Models;
using System.Windows;
using Xunit;

namespace AnnoDesigner.Tests
{
    public class LayoutObjectTests
    {
        private static readonly ICoordinateHelper coordinateHelper;

        static LayoutObjectTests()
        {
            coordinateHelper = new CoordinateHelper();
        }

        #region GridInfluenceRangeRect tests

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-0.001)]
        public void GridInfluenceRangeRect_InfluenceRangeIsZeroOrNegative_ShouldReturnEmptyRect(double influenceRangeToSet)
        {
            // Arrange
            AnnoObject annoObject = new()
            {
                InfluenceRange = influenceRangeToSet,
                Size = new Size(10, 10),
                Position = new Point(42, 42)
            };
            LayoutObject layoutObject = new(annoObject, null, null, null);

            // Act
            Rect influenceRangeRect = layoutObject.GridInfluenceRangeRect;

            // Assert
            Assert.Equal(annoObject.Position, influenceRangeRect.Location);
            Assert.Equal(default, influenceRangeRect.Size);
        }

        #endregion

        #region GetScreenRadius tests

        [Theory]
        [InlineData(5, 5, 100)]
        [InlineData(3, 3, 100)]
        [InlineData(5.5, 5.5, 100)]
        public void GetScreenRadius_SizeHeightAndWidthAreOdd_ShouldAdjustRadius(double widthToSet, double heightToSet, double expectedRadius)
        {
            // Arrange            
            AnnoObject annoObject = new()
            {
                Size = new Size(widthToSet, heightToSet),
                Radius = 10
            };
            LayoutObject layoutObject = new(annoObject, coordinateHelper, null, null);

            // Act
            double screenRadius = layoutObject.GetScreenRadius(10);

            // Assert
            Assert.Equal(expectedRadius, screenRadius);
        }

        [Fact]
        public void GetScreenRadius_SizeHeightIsOdd_ShouldNotAdjustRadius()
        {
            // Arrange            
            AnnoObject annoObject = new()
            {
                Size = new Size(8, 5),
                Radius = 10
            };
            LayoutObject layoutObject = new(annoObject, coordinateHelper, null, null);

            // Act
            double screenRadius = layoutObject.GetScreenRadius(10);

            // Assert
            Assert.Equal(100, screenRadius);
        }

        [Fact]
        public void GetScreenRadius_SizeWidthIsOdd_ShouldNotAdjustRadius()
        {
            // Arrange            
            AnnoObject annoObject = new()
            {
                Size = new Size(5, 8),
                Radius = 10
            };
            LayoutObject layoutObject = new(annoObject, coordinateHelper, null, null);

            // Act
            double screenRadius = layoutObject.GetScreenRadius(10);

            // Assert
            Assert.Equal(100, screenRadius);
        }

        [Fact]
        public void GetScreenRadius_NeitherSizeWidthNorHeightIsOdd_ShouldNotAdjustRadius()
        {
            // Arrange            
            AnnoObject annoObject = new()
            {
                Size = new Size(8, 8),
                Radius = 10
            };
            LayoutObject layoutObject = new(annoObject, coordinateHelper, null, null);

            // Act
            double screenRadius = layoutObject.GetScreenRadius(10);

            // Assert
            Assert.Equal(100, screenRadius);
        }

        #endregion
    }
}
