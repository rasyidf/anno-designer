using AnnoDesigner.Core.Extensions;
using AnnoDesigner.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AnnoDesigner.Core.Tests
{
    public class IEnumerableExtensionsTests
    {
        [Fact]
        public void WithoutIgnoredObjects_ListIsNull_ShouldReturnNull()
        {
            // Arrange/Act
            IEnumerable<AnnoObject> result = IEnumerableExtensions.WithoutIgnoredObjects(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void WithoutIgnoredObjects_ListIsEmpty_ShouldReturnEmptyList()
        {
            // Arrange/Act
            IEnumerable<AnnoObject> result = IEnumerableExtensions.WithoutIgnoredObjects([]);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void WithoutIgnoredObjects_ListHasNoIgnorableObjects_ShouldReturnSameList()
        {
            // Arrange
            List<AnnoObject> list =
            [
                new() {
                    Template = "Dummy"
                },
                new() {
                    Template = "AnotherDummy"
                }
            ];

            // Act
            IEnumerable<AnnoObject> result = IEnumerableExtensions.WithoutIgnoredObjects(list);

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
            List<AnnoObject> list =
            [
                new() {
                    Template = "Blocker"
                },
                new() {
                    Template = "Dummy"
                },
                new() {
                    Template = "AnotherDummy"
                }
            ];

            // Act
            IEnumerable<AnnoObject> result = IEnumerableExtensions.WithoutIgnoredObjects(list);

            // Assert
            Assert.NotEqual(list.Count(), result.Count());
            Assert.All(result, x =>
            {
                Assert.NotEqual("Blocker", x.Template, StringComparer.OrdinalIgnoreCase);
            });
        }
    }
}
