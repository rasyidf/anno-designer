using AnnoDesigner.Core.DataStructures;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Models;
using AnnoDesigner.Undo.Operations;
using Moq;
using System.Collections.Generic;
using System.Windows;
using Xunit;

namespace AnnoDesigner.Tests.Undo
{
    public class MoveObjectsOperationTests
    {
        private QuadTree<LayoutObject> Collection => new(new Rect(-16, -16, 32, 32));

        private LayoutObject CreateLayoutObject(double x, double y, double width, double height)
        {
            return new LayoutObject(new AnnoObject()
            {
                Position = new Point(x, y),
                Size = new Size(width, height)
            }, Mock.Of<ICoordinateHelper>(), Mock.Of<IBrushCache>(), Mock.Of<IPenCache>());
        }

        #region Undo tests

        [Fact]
        public void Undo_SingleObject_ShouldMoveObject()
        {
            // Arrange
            Rect expectedRect = new(-1, -2, 4, 3);

            QuadTree<LayoutObject> collection = Collection;
            LayoutObject obj = CreateLayoutObject(1, 2, 3, 4);
            collection.Add(obj);

            MoveObjectsOperation<LayoutObject> operation = new()
            {
                QuadTree = collection,
                ObjectPropertyValues = new List<(LayoutObject, Rect, Rect)>()
                {
                    (obj, expectedRect, obj.Bounds)
                }
            };

            // Act
            operation.Undo();

            // Assert
            Assert.Equal(expectedRect, obj.Bounds);
        }

        [Fact]
        public void Undo_MultipleObjects_ShouldMoveObjects()
        {
            // Arrange
            Rect expectedRect1 = new(-1, -2, 4, 3);
            Rect expectedRect2 = new(-3, -4, 8, 7);

            QuadTree<LayoutObject> collection = Collection;
            LayoutObject obj1 = CreateLayoutObject(1, 2, 3, 4);
            LayoutObject obj2 = CreateLayoutObject(5, 6, 7, 8);
            collection.Add(obj1);
            collection.Add(obj2);

            MoveObjectsOperation<LayoutObject> operation = new()
            {
                QuadTree = collection,
                ObjectPropertyValues = new List<(LayoutObject, Rect, Rect)>()
                {
                    (obj1, expectedRect1, obj1.Bounds),
                    (obj2, expectedRect2, obj2.Bounds)
                }
            };

            // Act
            operation.Undo();

            // Assert
            Assert.Equal(expectedRect1, obj1.Bounds);
            Assert.Equal(expectedRect2, obj2.Bounds);
        }

        [Fact]
        public void Undo_QuadTreeReindex_ShouldBeCalled()
        {
            // Arrange
            Mock<IQuadTree<LayoutObject>> collection = new();
            LayoutObject obj = CreateLayoutObject(1, 2, 3, 4);
            MoveObjectsOperation<LayoutObject> operation = new()
            {
                QuadTree = collection.Object,
                ObjectPropertyValues = new List<(LayoutObject, Rect, Rect)>()
                {
                    (obj, new Rect(-1, -2, 4, 3), obj.Bounds)
                }
            };

            // Act
            operation.Undo();

            // Assert
            collection.Verify(c => c.ReIndex(It.IsAny<LayoutObject>(), It.IsAny<Rect>()), Times.Once());
        }

        #endregion

        #region Redo tests

        [Fact]
        public void Redo_SingleObject_ShouldMoveObject()
        {
            // Arrange
            Rect expectedRect = new(-1, -2, 4, 3);

            QuadTree<LayoutObject> collection = Collection;
            LayoutObject obj = CreateLayoutObject(1, 2, 3, 4);
            collection.Add(obj);

            MoveObjectsOperation<LayoutObject> operation = new()
            {
                QuadTree = collection,
                ObjectPropertyValues = new List<(LayoutObject, Rect, Rect)>()
                {
                    (obj, obj.Bounds, expectedRect)
                }
            };

            // Act
            operation.Redo();

            // Assert
            Assert.Equal(expectedRect, obj.Bounds);
        }

        [Fact]
        public void Redo_MultipleObjects_ShouldMoveObjects()
        {
            // Arrange
            Rect expectedRect1 = new(-1, -2, 4, 3);
            Rect expectedRect2 = new(-3, -4, 8, 7);

            QuadTree<LayoutObject> collection = Collection;
            LayoutObject obj1 = CreateLayoutObject(1, 2, 3, 4);
            LayoutObject obj2 = CreateLayoutObject(5, 6, 7, 8);
            collection.Add(obj1);
            collection.Add(obj2);

            MoveObjectsOperation<LayoutObject> operation = new()
            {
                QuadTree = collection,
                ObjectPropertyValues = new List<(LayoutObject, Rect, Rect)>()
                {
                    (obj1, obj1.Bounds, expectedRect1),
                    (obj2, obj2.Bounds, expectedRect2)
                }
            };

            // Act
            operation.Redo();

            // Assert
            Assert.Equal(expectedRect1, obj1.Bounds);
            Assert.Equal(expectedRect2, obj2.Bounds);
        }

        [Fact]
        public void Redo_QuadTreeReindex_ShouldBeCalled()
        {
            // Arrange
            Mock<IQuadTree<LayoutObject>> collection = new();
            LayoutObject obj = CreateLayoutObject(1, 2, 3, 4);
            MoveObjectsOperation<LayoutObject> operation = new()
            {
                QuadTree = collection.Object,
                ObjectPropertyValues = new List<(LayoutObject, Rect, Rect)>()
                {
                    (obj, obj.Bounds, new Rect(-1, -2, 4, 3))
                }
            };

            // Act
            operation.Redo();

            // Assert
            collection.Verify(c => c.ReIndex(It.IsAny<LayoutObject>(), It.IsAny<Rect>()), Times.Once());
        }

        #endregion
    }
}
