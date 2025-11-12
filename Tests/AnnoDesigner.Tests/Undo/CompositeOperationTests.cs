using AnnoDesigner.Undo.Operations;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AnnoDesigner.Tests.Undo
{
    public class CompositeOperationTests
    {
        #region Undo tests

        [Fact]
        public void Undo_ShouldUndoOperationsInCorrectOrder()
        {
            // Arrange
            List<IOperation> order = [];

            Mock<IOperation> op1 = new();
            _ = op1.Setup(op => op.Undo()).Callback(() => order.Add(op1.Object));
            Mock<IOperation> op2 = new();
            _ = op2.Setup(op => op.Undo()).Callback(() => order.Add(op2.Object));

            CompositeOperation operation = new()
            {
                Operations =
                [
                    op1.Object,
                    op2.Object
                ]
            };

            // Act
            operation.Undo();

            // Assert
            Assert.Equal(new[] { op2.Object, op1.Object }, order);
        }

        #endregion

        #region Redo tests

        [Fact]
        public void Redo_ShouldRedoOperationsInCorrectOrder()
        {
            // Arrange
            List<IOperation> order = [];

            Mock<IOperation> op1 = new();
            _ = op1.Setup(op => op.Redo()).Callback(() => order.Add(op1.Object));
            Mock<IOperation> op2 = new();
            _ = op2.Setup(op => op.Redo()).Callback(() => order.Add(op2.Object));

            CompositeOperation operation = new()
            {
                Operations =
                [
                    op1.Object,
                    op2.Object
                ]
            };

            // Act
            operation.Redo();

            // Assert
            Assert.Equal(new[] { op1.Object, op2.Object }, order);
        }

        #endregion
    }
}
