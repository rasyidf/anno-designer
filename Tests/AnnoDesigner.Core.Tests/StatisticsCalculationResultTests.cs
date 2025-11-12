using AnnoDesigner.Core.Layout.Models;
using System;
using Xunit;

namespace AnnoDesigner.Core.Tests
{
    public class StatisticsCalculationResultTests
    {
        #region IEquatable tests

        [Fact]
        public void Implements_Interface_IEquatable()
        {
            Assert.True(typeof(IEquatable<StatisticsCalculationResult>).IsAssignableFrom(typeof(StatisticsCalculationResult)));
        }

        [Fact]
        public void Equals_IsEqual()
        {
            StatisticsCalculationResult result1 = new(42, 42, 45, 45, 3, 3, 9, 9, 100);
            StatisticsCalculationResult result2 = new(42, 42, 45, 45, 3, 3, 9, 9, 100);

            Assert.True(result1.Equals(result2));
            Assert.True(result1.Equals((object)result2));
            Assert.True(result1.Equals(result1));
        }

        [Fact]
        public void Equals_IsNotEqual()
        {
            StatisticsCalculationResult result1 = new(42, 42, 45, 45, 3, 3, 9, 9, 100);
            StatisticsCalculationResult result2 = new(21, 21, 45, 45, 3, 3, 9, 9, 100);

            Assert.False(result1.Equals(result2));
            Assert.False(result1.Equals((object)result2));
            Assert.False(result1.Equals(null));
        }

        [Fact]
        public void GetHashCode_IsEqual()
        {
            StatisticsCalculationResult result1 = new(42, 42, 45, 45, 3, 3, 9, 9, 100);
            StatisticsCalculationResult result2 = new(42, 42, 45, 45, 3, 3, 9, 9, 100);

            Assert.Equal(result1.GetHashCode(), result2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_IsNotEqual()
        {
            StatisticsCalculationResult result1 = new(42, 42, 45, 45, 3, 3, 9, 9, 100);
            StatisticsCalculationResult result2 = new(21, 21, 45, 45, 3, 3, 9, 9, 100);

            Assert.NotEqual(result1.GetHashCode(), result2.GetHashCode());
        }

        #endregion
    }
}
