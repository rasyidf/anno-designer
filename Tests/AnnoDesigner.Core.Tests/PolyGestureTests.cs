using AnnoDesigner.Core.Models;
using System;
using System.Windows;
using System.Windows.Input;
using Xunit;

namespace AnnoDesigner.Core.Tests
{
    public class PolyGestureTests
    {
        public class TestDevice : InputDevice
        {
            public override IInputElement Target { get; }
            public override PresentationSource ActiveSource { get; }
        }

        [Fact]
        public void Set_Type_InvalidGestureType_ShouldThrowArgumentException()
        {
            //Arrange
            PolyGesture p = new(Key.A, ModifierKeys.Control);
            //Act and Assert
            _ = Assert.Throws<ArgumentException>(() => p.Type = (GestureType)int.MaxValue);
        }

        [Theory]
        [InlineData(GestureType.KeyGesture, true)]
        [InlineData(GestureType.MouseGesture, true)]
        [InlineData((GestureType)int.MaxValue, false)]
        [InlineData((GestureType)2, false)]
        [InlineData((GestureType)3, false)]
        [InlineData((GestureType)(-1), false)]
        public void IsDefinedGestureType_ReturnsCorrectValue(GestureType type, bool expected)
        {
            Assert.Equal(expected, PolyGesture.IsDefinedGestureType(type));
        }

        [Fact]
        public void Matches_NullArgument_ShouldReturnFalse()
        {
            //Arrange
            PolyGesture p = new();

            //Act
            bool actual = p.Matches(null, null);

            //Assert
            Assert.False(actual);
        }

        [Fact]
        public void Matches_InvalidInputEventArgsType_ShouldReturnFalse()
        {
            //Arrange
            PolyGesture p = new()
            {
                Type = GestureType.KeyGesture
            };

            //Act
            bool actual = p.Matches(null, new InputEventArgs(new TestDevice(), 0));

            //Assert
            Assert.False(actual);
        }
    }
}
