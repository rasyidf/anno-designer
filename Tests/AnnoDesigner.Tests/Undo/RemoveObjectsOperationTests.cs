using AnnoDesigner.Core.DataStructures;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Models;
using AnnoDesigner.Undo.Operations;
using Moq;
using System.Windows;
using Xunit;

namespace AnnoDesigner.Tests.Undo
{
    public class RemoveObjectsOperationTests
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
        public void Undo_SingleObject_ShouldAddObjectToCollection()
        {
            // Arrange
            QuadTree<LayoutObject> collection = Collection;
            Assert.Empty(collection);
            LayoutObject obj = CreateLayoutObject(5, 5, 2, 2);
            RemoveObjectsOperation<LayoutObject> operation = new()
            {
                Collection = collection,
                Objects =
                [
                    obj
                ]
            };

            // Act
            operation.Undo();

            // Assert
            _ = Assert.Single(collection);
        }

        [Fact]
        public void Undo_MultipleObjects_ShouldAddObjectsToCollection()
        {
            // Arrange
            QuadTree<LayoutObject> collection = Collection;
            LayoutObject obj1 = CreateLayoutObject(5, 5, 2, 2);
            LayoutObject obj2 = CreateLayoutObject(0, 0, 2, 2);
            RemoveObjectsOperation<LayoutObject> operation = new()
            {
                Collection = collection,
                Objects =
                [
                    obj1,
                    obj2
                ]
            };

            // Act
            operation.Undo();

            // Assert
            Assert.Equal(2, collection.Count);
        }

        #endregion

        #region Undo tests

        [Fact]
        public void Redo_SingleObject_ShouldRemoveObjectFromCollection()
        {
            // Arrange
            QuadTree<LayoutObject> collection = Collection;
            LayoutObject obj = CreateLayoutObject(5, 5, 2, 2);
            collection.Add(obj);
            RemoveObjectsOperation<LayoutObject> operation = new()
            {
                Collection = collection,
                Objects =
                [
                    obj
                ]
            };

            // Act
            operation.Redo();

            // Assert
            Assert.Empty(collection);
        }

        [Fact]
        public void Redo_MultipleObjects_ShouldRemoveObjectsFromCollection()
        {
            // Arrange
            QuadTree<LayoutObject> collection = Collection;
            LayoutObject obj1 = CreateLayoutObject(5, 5, 2, 2);
            LayoutObject obj2 = CreateLayoutObject(0, 0, 2, 2);
            collection.Add(obj1);
            collection.Add(obj2);
            RemoveObjectsOperation<LayoutObject> operation = new()
            {
                Collection = collection,
                Objects =
                [
                    obj1,
                    obj2
                ]
            };

            // Act
            operation.Redo();

            // Assert
            Assert.Empty(collection);
        }

        #endregion
    }
}
