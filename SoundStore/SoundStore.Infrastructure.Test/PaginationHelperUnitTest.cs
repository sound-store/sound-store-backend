using SoundStore.Infrastructure.Helpers;

namespace SoundStore.Infrastructure.Test
{
    public class PaginationHelperUnitTest
    {
        [Fact]
        public void CreatePaginatedList_ReturnsCorrectPage()
        {
            // Arrange
            var data = Enumerable.Range(1, 100).AsQueryable(); // Items: 1 to 100
            int pageNumber = 2;
            int pageSize = 10;

            // Act
            var result = PaginationHelper.CreatePaginatedList(data, pageNumber, pageSize);

            // Assert
            Assert.Equal(10, result.Items.Count);               // 10 items per page
            Assert.Equal(100, result.TotalItems);               // total items
            Assert.Equal(2, result.PageNumber);                 // current page
            Assert.Equal(10, result.TotalPages);                // total pages
            Assert.True(result.HasPreviousPage);                // should have previous
            Assert.True(result.HasNextPage);                    // should have next
            Assert.Equal(11, result.Items.First());             // items 11–20 on page 2
            Assert.Equal(20, result.Items.Last());
        }

        [Fact]
        public void CreatePaginatedList_FirstPage_IsCorrect()
        {
            // Arrange
            var data = Enumerable.Range(1, 50).AsQueryable();
            int pageNumber = 1;
            int pageSize = 10;

            // Act
            var result = PaginationHelper.CreatePaginatedList(data, pageNumber, pageSize);

            // Assert
            Assert.Equal(10, result.Items.Count);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(50, result.TotalItems);
            Assert.Equal(5, result.TotalPages);
            Assert.False(result.HasPreviousPage);
            Assert.True(result.HasNextPage);
            Assert.Equal(1, result.Items.First());
            Assert.Equal(10, result.Items.Last());
        }

        [Fact]
        public void CreatePaginatedList_LastPage_IsCorrect()
        {
            // Arrange
            var data = Enumerable.Range(1, 95).AsQueryable();
            int pageNumber = 10;
            int pageSize = 10;

            // Act
            var result = PaginationHelper.CreatePaginatedList(data, pageNumber, pageSize);

            // Assert
            Assert.Equal(5, result.Items.Count); // 95 items => 5 on last page
            Assert.Equal(10, result.PageNumber);
            Assert.Equal(95, result.TotalItems);
            Assert.Equal(10, result.TotalPages);
            Assert.True(result.HasPreviousPage);
            Assert.False(result.HasNextPage);
            Assert.Equal(91, result.Items.First());
            Assert.Equal(95, result.Items.Last());
        }

        [Fact]
        public void CreatePaginatedList_PageOutOfRange_ReturnsEmpty()
        {
            // Arrange
            var data = Enumerable.Range(1, 30).AsQueryable();
            int pageNumber = 5; // Only 3 pages exist
            int pageSize = 10;

            // Act
            var result = PaginationHelper.CreatePaginatedList(data, pageNumber, pageSize);

            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(30, result.TotalItems);
            Assert.Equal(5, result.PageNumber);   // still shows the requested page number
            Assert.Equal(3, result.TotalPages);
            Assert.True(result.HasPreviousPage);
            Assert.False(result.HasNextPage);
        }
    }
}