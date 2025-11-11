using AnnoDesigner.Helper;
using AnnoDesigner.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Xunit;

namespace AnnoDesigner.Tests
{
    public class AdjacentCellGrouperTests
    {
        private class Item
        {
            public int Value { get; set; }
        }

        private readonly AdjacentCellGrouper grouper = new();

        /// <summary>
        /// Returns grid representation of string lines.
        /// Character "X" is used to represent grid cell being occupied. Any other character means empty cell.
        /// Start of coordinates is in top left corner, width is horizontal and height is vertical.
        /// </summary>
        /// <example>
        /// "XX  ",
        /// "XX  ",
        /// "XXXX",
        /// "XXXX"
        /// </example>
        public static bool[][] ParseGrid(params string[] gridLines)
        {
            bool[][] preTranspose = gridLines.Select(line => line.Select(c => c == 'X').ToArray()).ToArray();
            bool[][] postTranspose = Enumerable.Range(0, gridLines.Max(i => i.Length)).Select(i => new bool[gridLines.Length]).ToArray();

            for (int i = 0; i < gridLines.Length; i++)
            {
                for (int j = 0; j < gridLines[i].Length; j++)
                {
                    postTranspose[j][i] = preTranspose[i][j];
                }
            }

            return postTranspose;
        }

        [Fact]
        public void MergeItems_WholeArray_OneBigObject()
        {
            // Arrange
            bool[][] cells = ParseGrid(
                "XXXXX",
                "XXXXX",
                "XXXXX",
                "XXXXX",
                "XXXXX");

            // Act
            List<CellGroup<bool>> groups = grouper.GroupAdjacentCells(cells, false).ToList();

            // Assert
            _ = Assert.Single(groups);
            Assert.Equal(new Rect(0, 0, 5, 5), groups[0].Bounds);
        }

        [Fact]
        public void MergeItems_ChessboardWithoutSingleCellMerge_NothingFound()
        {
            // Arrange
            bool[][] cells = ParseGrid(
                "X X X ",
                " X X X",
                "X X X ",
                " X X X");

            // Act
            List<CellGroup<bool>> groups = grouper.GroupAdjacentCells(cells, false).ToList();

            // Assert
            Assert.Empty(groups);
        }

        [Fact]
        public void MergeItems_ChessboardWithSingleCells_LotOfSmallObjects()
        {
            // Arrange
            bool[][] cells = ParseGrid(
                "X X X ",
                " X X X",
                "X X X ",
                " X X X");

            // Act
            List<CellGroup<bool>> groups = grouper.GroupAdjacentCells(cells, true).ToList();

            // Assert
            Assert.Equal(12, groups.Count);
            Assert.True(groups.All(g => g.Bounds.Width * g.Bounds.Height == 1));
        }

        [Fact]
        public void MergeItems_Pyramid()
        {
            // Arrange
            bool[][] cells = ParseGrid(
                "   X   ",
                "  XXX  ",
                " XXXXX ",
                "XXXXXXX");

            // Act
            List<CellGroup<bool>> groups = grouper.GroupAdjacentCells(cells, false).ToList();

            // Assert
            Assert.Equal(new[]
            {
                new Rect(1, 2, 5, 2),
                new Rect(2, 1, 3, 1)
            }, groups.Select(g => g.Bounds));
        }

        [Fact]
        public void MergeItems_Corners_FirstMatchReturnedFirst()
        {
            // Arrange
            bool[][] cells = ParseGrid(
                "XXX XXX",
                "X     X",
                "X     X",
                "       ",
                "X     X",
                "X     X",
                "XXX XXX");

            // Act
            List<CellGroup<bool>> groups = grouper.GroupAdjacentCells(cells, false).ToList();

            // Assert
            Assert.Equal(new[]
            {
                new Rect(0, 0, 1, 3),
                new Rect(0, 4, 1, 3),
                new Rect(4, 0, 3, 1),
                new Rect(4, 6, 3, 1),
                new Rect(1, 0, 2, 1),
                new Rect(1, 6, 2, 1),
                new Rect(6, 1, 1, 2),
                new Rect(6, 4, 1, 2)
            }, groups.Select(g => g.Bounds));
        }

        [Fact]
        public void MergeItems_RandomWithSpacesBetween_ReturnsMatchesFromLargestAreaFirst()
        {
            // Arrange
            bool[][] cells = ParseGrid(
                "XX XXX X",
                "XX XXX X",
                "   XXX X",
                "       X",
                "XXXXXX X");

            // Act
            List<CellGroup<bool>> groups = grouper.GroupAdjacentCells(cells, false).ToList();

            // Assert
            Assert.Equal(new[]
            {
                new Rect(3, 0, 3, 3), // area 9
                new Rect(0, 4, 6, 1), // area 6
                new Rect(7, 0, 1, 5), // area 5
                new Rect(0, 0, 2, 2)  // area 4
            }, groups.Select(g => g.Bounds));
        }

        [Fact]
        public void MergeItems_RandomWithoutSpacesBetween_ItemsAreNotUsedMultipleTimes()
        {
            // Arrange
            bool[][] cells = ParseGrid(
                "XXXX X",
                "XXXX X",
                " XXXXX",
                " XXXXX",
                "     X",
                "XXXXXX");

            // Act
            List<CellGroup<bool>> groups = grouper.GroupAdjacentCells(cells, false).ToList();

            // Assert
            Assert.Equal(new[]
            {
                new Rect(1, 0, 3, 4), // area 12
                new Rect(0, 5, 6, 1), // area 6
                new Rect(5, 0, 1, 5), // area 5
                new Rect(0, 0, 1, 2), // area 2
                new Rect(4, 2, 1, 2)  // area 2
            }, groups.Select(g => g.Bounds));
        }

        [Fact]
        public void MergeItems_MultipleSameReferences_ReferenceIsUsedOnlyOnce()
        {
            // Arrange
            Item item1 = new() { Value = 1 }; // X
            Item item2 = new() { Value = 1 }; // Y
            Item item3 = new() { Value = 1 }; // Z
            // XYZZ
            // XY
            // XY
            // X
            Item[][] cells = new Item[][]
            {
                new Item[]
                {
                    item1, item1, item1, item1
                },
                new Item[]
                {
                    item2, item2, item2, null
                },
                new Item[]
                {
                    item3, null, null, null
                },
                new Item[]
                {
                    item3, null, null, null
                }
            };

            // Act
            List<CellGroup<Item>> groups = grouper.GroupAdjacentCells(cells, false).ToList();

            // Assert
            Assert.Equal(new[]
            {
                new Rect(0, 0, 2, 3), // area 6
                new Rect(2, 0, 2, 1), // area 2
            }, groups.Select(g => g.Bounds));

            Assert.Equal(new[]
            {
                item1,
                item2
            }, groups[0].Items);
            Assert.Equal(new[]
            {
                item3
            }, groups[1].Items);
        }
    }
}
