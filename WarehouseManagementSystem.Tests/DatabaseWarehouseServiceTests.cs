using Microsoft.EntityFrameworkCore;
using WarehouseManagementSystem.Data;
using WarehouseManagementSystem.Services;
using Xunit;

namespace WarehouseManagementSystem.Tests
{
    public class DatabaseWarehouseServiceTests
    {
        private WarehouseDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<WarehouseDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            return new WarehouseDbContext(options);
        }

        [Fact]
        public async Task AddProduct_ShouldAddProductSuccessfully()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new DatabaseWarehouseService(context);
            var product = new Models.Product
            {
                Name = "测试产品",
                Barcode = "1234567890123",
                Price = 100m,
                Quantity = 50
            };

            // Act
            var result = await service.AddProductAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("测试产品", result.Name);
        }

        [Fact]
        public async Task GetProductByBarcode_ShouldReturnCorrectProduct()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new DatabaseWarehouseService(context);

            var product = new Models.Product
            {
                Name = "测试产品",
                Barcode = "1234567890123",
                Price = 100m,
                Quantity = 50
            };
            await service.AddProductAsync(product);

            // Act
            var result = await service.GetProductByBarcodeAsync("1234567890123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1234567890123", result.Barcode);
        }
    }
}