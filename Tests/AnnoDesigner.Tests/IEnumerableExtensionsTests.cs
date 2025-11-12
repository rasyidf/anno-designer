using AnnoDesigner.Core.Models;
using AnnoDesigner.Extensions;
using AnnoDesigner.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AnnoDesigner.Tests
{
    public class IEnumerableExtensionsTests
    {
        private readonly ICoordinateHelper mockedCoordinateHelper;
        private readonly IBrushCache mockedBrushCache;
        private readonly IPenCache mockedPenCache;

        public IEnumerableExtensionsTests()
        {
            mockedCoordinateHelper = new Mock<ICoordinateHelper>().Object;
            mockedBrushCache = new Mock<IBrushCache>().Object;
            mockedPenCache = new Mock<IPenCache>().Object;
        }

        [Fact]
        public void WithoutIgnoredObjects_ListIsNull_ShouldReturnNull()
        {
            // Arrange/Act
            ICollection<LayoutObject> result = IEnumerableExtensions.WithoutIgnoredObjects(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void WithoutIgnoredObjects_ListIsEmpty_ShouldReturnEmptyList()
        {
            // Arrange/Act
            ICollection<LayoutObject> result = IEnumerableExtensions.WithoutIgnoredObjects([]);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void WithoutIgnoredObjects_ListHasNoIgnorableObjects_ShouldReturnSameList()
        {
            // Arrange
            List<LayoutObject> list =
            [
                new(
                    new AnnoObject
                    {
                        Template = "Dummy"
                    } ,mockedCoordinateHelper, mockedBrushCache, mockedPenCache),
                new(
                    new AnnoObject
                    {
                        Template = "AnotherDummy"
                    } ,mockedCoordinateHelper, mockedBrushCache, mockedPenCache),
            ];

            // Act
            ICollection<LayoutObject> result = IEnumerableExtensions.WithoutIgnoredObjects(list);

            // Assert
            Assert.Equal(list.Count(), result.Count());
            Assert.All(result, x =>
            {
                Assert.Contains(x, list);
            });
        }

        [Fact]
        public void WithoutIgnoredObjects_ListHasIgnorableObjects_ShouldReturnFilteredList()
        {
            // Arrange
            List<LayoutObject> list =
            [
                new(
                    new AnnoObject
                    {
                        Template = "Blocker"
                    } ,mockedCoordinateHelper, mockedBrushCache, mockedPenCache),
                new(
                    new AnnoObject
                    {
                        Template = "Dummy"
                    } ,mockedCoordinateHelper, mockedBrushCache, mockedPenCache),
                new(
                    new AnnoObject
                    {
                        Template = "AnotherDummy"
                    } ,mockedCoordinateHelper, mockedBrushCache, mockedPenCache),
            ];

            // Act
            ICollection<LayoutObject> result = IEnumerableExtensions.WithoutIgnoredObjects(list);

            // Assert
            Assert.NotEqual(list.Count(), result.Count());
            Assert.All(result, x =>
            {
                Assert.NotEqual("Blocker", x.WrappedAnnoObject.Template, StringComparer.OrdinalIgnoreCase);
            });
        }
    }
}
