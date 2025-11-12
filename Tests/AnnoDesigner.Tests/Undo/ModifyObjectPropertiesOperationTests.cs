using AnnoDesigner.Core.Models;
using AnnoDesigner.Models;
using AnnoDesigner.Undo.Operations;
using Moq;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using Xunit;

namespace AnnoDesigner.Tests.Undo
{
    public class ModifyObjectPropertiesOperationTests
    {
        private LayoutObject CreateLayoutObject(Color color)
        {
            return new LayoutObject(new AnnoObject()
            {
                Color = color
            }, Mock.Of<ICoordinateHelper>(), Mock.Of<IBrushCache>(), Mock.Of<IPenCache>());
        }

        #region Undo tests

        [Fact]
        public void Undo_SingleObject_ShouldSetColor()
        {
            // Arrange
            Color expectedColor = Colors.Black;
            LayoutObject obj = CreateLayoutObject(Colors.White);
            ModifyObjectPropertiesOperation<LayoutObject, SerializableColor> operation = new()
            {
                PropertyName = nameof(LayoutObject.Color),
                ObjectPropertyValues = new List<(LayoutObject, SerializableColor, SerializableColor)>()
                {
                    (obj, expectedColor, obj.Color)
                }
            };

            // Act
            operation.Undo();

            // Assert
            Assert.Equal<SerializableColor>(expectedColor, obj.Color);
        }

        [Fact]
        public void Undo_ShouldInvokeRedrawAction()
        {
            // Arrange
            Mock<Action> actionMock = new();
            ModifyObjectPropertiesOperation<LayoutObject, SerializableColor> operation = new()
            {
                PropertyName = nameof(LayoutObject.Color),
                ObjectPropertyValues = new List<(LayoutObject, SerializableColor, SerializableColor)>(),
                AfterAction = actionMock.Object
            };

            // Act
            operation.Undo();

            // Assert
            actionMock.Verify(action => action(), Times.Once());
        }

        [Fact]
        public void Undo_MultipleObjects_ShouldSetColors()
        {
            // Arrange
            Color expectedColor1 = Colors.Black;
            Color expectedColor2 = Colors.Blue;
            LayoutObject obj1 = CreateLayoutObject(Colors.White);
            LayoutObject obj2 = CreateLayoutObject(Colors.Red);
            ModifyObjectPropertiesOperation<LayoutObject, SerializableColor> operation = new()
            {
                PropertyName = nameof(LayoutObject.Color),
                ObjectPropertyValues = new List<(LayoutObject, SerializableColor, SerializableColor)>()
                {
                    (obj1, expectedColor1, obj1.Color),
                    (obj2, expectedColor2, obj2.Color),
                }
            };

            // Act
            operation.Undo();

            // Assert
            Assert.Equal<SerializableColor>(expectedColor1, obj1.Color);
            Assert.Equal<SerializableColor>(expectedColor2, obj2.Color);
        }

        #endregion

        #region Redo tests

        [Fact]
        public void Redo_SingleObject_ShouldSetColor()
        {
            // Arrange
            Color expectedColor = Colors.Black;
            LayoutObject obj = CreateLayoutObject(Colors.White);
            ModifyObjectPropertiesOperation<LayoutObject, SerializableColor> operation = new()
            {
                PropertyName = nameof(LayoutObject.Color),
                ObjectPropertyValues = new List<(LayoutObject, SerializableColor, SerializableColor)>()
                {
                    (obj, obj.Color, expectedColor)
                }
            };

            // Act
            operation.Redo();

            // Assert
            Assert.Equal<SerializableColor>(expectedColor, obj.Color);
        }

        [Fact]
        public void Redo_MultipleObjects_ShouldSetColors()
        {
            // Arrange
            Color expectedColor1 = Colors.Black;
            Color expectedColor2 = Colors.Blue;
            LayoutObject obj1 = CreateLayoutObject(Colors.White);
            LayoutObject obj2 = CreateLayoutObject(Colors.Red);
            ModifyObjectPropertiesOperation<LayoutObject, SerializableColor> operation = new()
            {
                PropertyName = nameof(LayoutObject.Color),
                ObjectPropertyValues = new List<(LayoutObject, SerializableColor, SerializableColor)>()
                {
                    (obj1, obj1.Color, expectedColor1),
                    (obj2, obj2.Color, expectedColor2),
                }
            };

            // Act
            operation.Redo();

            // Assert
            Assert.Equal<SerializableColor>(expectedColor1, obj1.Color);
            Assert.Equal<SerializableColor>(expectedColor2, obj2.Color);
        }

        [Fact]
        public void Redo_ShouldInvokeRedrawAction()
        {
            // Arrange
            Mock<Action> actionMock = new();
            ModifyObjectPropertiesOperation<LayoutObject, SerializableColor> operation = new()
            {
                PropertyName = nameof(LayoutObject.Color),
                ObjectPropertyValues = new List<(LayoutObject, SerializableColor, SerializableColor)>(),
                AfterAction = actionMock.Object
            };

            // Act
            operation.Redo();

            // Assert
            actionMock.Verify(action => action(), Times.Once());
        }

        #endregion
    }
}
