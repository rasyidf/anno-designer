using System.Windows;
using AnnoDesigner.Core.DataStructures;
using AnnoDesigner.Core.Models;
using Xunit;

namespace AnnoDesigner.Core.Tests
{
    public class QuadTreeTests
    {
        private class Bounded : IBounded
        {
            private Rect bounds;

            public Point Position { get => bounds.Location; set => bounds.Location = value; }
            public Size Size { get => bounds.Size; set => bounds.Size = value; }
            public Rect Bounds { get => bounds; set => bounds = value; }
        }

        #region EnsureBounds

        private static readonly Vector Top = new(0, -1);
        private static readonly Vector Right = new(1, 0);
        private static readonly Vector Bottom = new(0, 1);
        private static readonly Vector Left = new(-1, 0);

        private static readonly Rect DefaultRect = new(0, 0, 1, 1);
        private static readonly Point Center = new(0.5, 0.5);
        private static readonly Size SmallRectSize = new(0.1, 0.1);

        [Fact]
        public void EnsureBounds_Top_ShouldBeInflatedToTopRight()
        {
            // Arrange
            var collection = new QuadTree<Bounded>(DefaultRect);

            // Act
            collection.EnsureBounds(new Rect(Center + Top, SmallRectSize));

            // Assert
            Assert.Equal(collection.Extent, new Rect(0, -1, 2, 2));
        }

        [Fact]
        public void EnsureBounds_TopRight_ShouldBeInflatedToTopRight()
        {
            // Arrange
            var collection = new QuadTree<Bounded>(DefaultRect);

            // Act
            collection.EnsureBounds(new Rect(Center + Top + Right, SmallRectSize));

            // Assert
            Assert.Equal(collection.Extent, new Rect(0, -1, 2, 2));
        }

        [Fact]
        public void EnsureBounds_Right_ShouldBeInflatedToBottomRight()
        {
            // Arrange
            var collection = new QuadTree<Bounded>(DefaultRect);

            // Act
            collection.EnsureBounds(new Rect(Center + Right, SmallRectSize));

            // Assert
            Assert.Equal(collection.Extent, new Rect(0, 0, 2, 2));
        }

        [Fact]
        public void EnsureBounds_BottomRight_ShouldBeInflatedToBottomRight()
        {
            // Arrange
            var collection = new QuadTree<Bounded>(DefaultRect);

            // Act
            collection.EnsureBounds(new Rect(Center + Bottom + Right, SmallRectSize));

            // Assert
            Assert.Equal(collection.Extent, new Rect(0, 0, 2, 2));
        }

        [Fact]
        public void EnsureBounds_Bottom_ShouldBeInflatedToBottomLeft()
        {
            // Arrange
            var collection = new QuadTree<Bounded>(DefaultRect);

            // Act
            collection.EnsureBounds(new Rect(Center + Bottom, SmallRectSize));

            // Assert
            Assert.Equal(collection.Extent, new Rect(-1, 0, 2, 2));
        }

        [Fact]
        public void EnsureBounds_BottomLeft_ShouldBeInflatedToBottomLeft()
        {
            // Arrange
            var collection = new QuadTree<Bounded>(DefaultRect);

            // Act
            collection.EnsureBounds(new Rect(Center + Bottom + Left, SmallRectSize));

            // Assert
            Assert.Equal(collection.Extent, new Rect(-1, 0, 2, 2));
        }

        [Fact]
        public void EnsureBounds_Left_ShouldBeInflatedToTopLeft()
        {
            // Arrange
            var collection = new QuadTree<Bounded>(DefaultRect);

            // Act
            collection.EnsureBounds(new Rect(Center + Left, SmallRectSize));

            // Assert
            Assert.Equal(collection.Extent, new Rect(-1, -1, 2, 2));
        }

        [Fact]
        public void EnsureBounds_TopLeft_ShouldBeInflatedToTopLeft()
        {
            // Arrange
            var collection = new QuadTree<Bounded>(DefaultRect);

            // Act
            collection.EnsureBounds(new Rect(Center + Top + Left, SmallRectSize));

            // Assert
            Assert.Equal(collection.Extent, new Rect(-1, -1, 2, 2));
        }

        #endregion
    }
}
