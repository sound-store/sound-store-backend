using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SoundStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class SeedDataForProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "AccessoriesIncluded", "BatteryLife", "Connectivity", "CreatedAt", "Description", "FrequencyResponse", "Name", "Price", "Sensitivity", "SpecialFeatures", "Status", "StockQuantity", "SubCategoryId", "Type", "UpdatedAt", "Warranty" },
                values: new object[,]
                {
                    { 1L, "Cáp kết nối, túi đựng", null, "Có dây", new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thiết kế mạnh mẽ cùng pin dài mang lại trải nghiệm loa Marshall Emberton 3 vô cùng khác biệt, pin lên tới 32 tiếng, chống nước IP 67, có Micro trong, sạc cực nhanh.", "20Hz - 20kHz", "Tai nghe Marshall Major 5", 4290000m, "105 dB", "Âm thanh chi tiết, khả năng chống ồn hiệu quả.", 1, 50, 6, "Tai nghe", null, "Bảo hành 12 tháng" },
                    { 2L, "Cáp sạc, ốp tai", null, "Có dây", new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thiết kế đơn giản của tai nghe Marshall Minor IV cho phép bạn tập trung vào điều quan trọng nhất – âm nhạc. Những chiếc tai nghe này bao gồm tất cả các tính năng để bạn có thể đắm chìm trong âm thanh đặc trưng của Marshall mọi lúc mọi nơi. Mang theo hơn 30 giờ thời gian chơi không dây cùng với hộp đựng, nghĩa là bạn có hơn một ngày để đắm chìm trong âm nhạc hoặc podcast yêu thích của mình. Thêm thiết kế tai nghe chống nước , bạn sẽ tận hưởng từng nhịp điệu một cách hoàn toàn thoải mái.", "18Hz - 22kHz", "Tai nghe Marshall Minor 4", 3690000m, "100 dB", "Chất lượng âm thanh tuyệt vời, thiết kế tiện lợi.", 1, 70, 7, "Tai nghe", null, "Bảo hành 12 tháng" },
                    { 3L, "Cáp kết nối, đệm tai mềm mại", null, "Có dây", new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tai nghe Marshall Motif II A.N.C mang đến âm thanh lớn trong môt thiết kế bỏ túi nhỏ gọn. Hộp sạc với kiểu dáng thời thượng cung cấp cho tai nghe khả năng chơi nhạc không dây lên tới 30 giờ. Mẫu tai nghe in-ear này giờ đây được trang bị công nghệ Bluetooth LE (Low Energy) cho khả năng kết nối không dây tốt hơn bao giờ hết. Với tính năng khử tiếng ồn chủ động và việc cải thiện hiệu suất lọc gió, Motif II A.N.C có thể loại bỏ tiếng ồn từ đám đông và phương tiện xung quanh, giúp bạn thả hồn vào âm nhạc mà không bị phân tâm.", "20Hz - 20kHz", "Tai nghe Marshall Motif 2 A.N.C", 5490000m, "102 dB", "Âm thanh vòm, micro rõ ràng.", 1, 30, 7, "Tai nghe", null, "Bảo hành 12 tháng" },
                    { 4L, "Cáp sạc, hướng dẫn sử dụng", "10 giờ", "Bluetooth", new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đến với tai nghe Marshall Minor III và trải nghiệm âm thanh đặc trưng của Marshall với khả năng âm thanh chi tiết, rõ nét. Tận hưởng thời lượng pin với tổng 25 giờ sử dụng và tự do nghe nhạc không dây với chất lượng âm thanh cực tốt. Tai nghe Marshall Minor 3 được tối ưu tất cả bước sử dụng phức tạp, tất cả những gì cần làm là ghép nối với thiết bị của mình và trải nghiệm âm nhạc.", "50Hz - 20kHz", "Tai nghe Marshall Minor 3", 3390000m, "95 dB", "Kết nối nhanh, thời lượng pin lên đến 10 giờ.", 1, 40, 7, "Tai nghe", null, "Bảo hành 12 tháng" },
                    { 5L, "Cáp sạc, túi đựng", "8 giờ", "Jack 3.5mm", new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tai nghe Marshall Mode cho ra chất lượng âm thanh mạnh mẽ chỉ trong một chiếc thiết kế nhỏ gọn. Driver cải tiến cho đầu ra nhạc chất lượng cao mà không bị biến dạng âm thanh. Thiết kế độc đáo có 4 chiếc cao su bọc ngoài đi kèm với các kích cỡ khác nhau để bạn có thể thay đổi tùy theo kích cỡ tai của mỗi người.", "55Hz - 18kHz", "Tai nghe Marshall Mode", 1290000m, "90 dB", "Âm bass mạnh mẽ, thiết kế di động.", 1, 60, 7, "Tai nghe", null, "Bảo hành 12 tháng" },
                    { 6L, "Cáp nguồn, hướng dẫn sử dụng", null, "Bluetooth", new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tai nghe Marshall Major 4 mang thiết kế đặc trưng của Marshall cho khả năng chơi nhạc không dây lên đến 80 giờ liên tục, bổ sung thêm tính năng sạc không dây và thiết kế được cải tiến đem đến sự thoải mái vượt trội và tiện dụng hơn. Các dải âm được bổ sung tạo ra âm trầm mạnh mẽ, âm trung mượt mà và âm bổng tuyệt vời khiến cho khoảnh khắc thưởng nhạc trở nên trọn vẹn hơn bao giờ hết.", "45Hz - 19kHz", "Tai nghe Marshall Major 4", 4290000m, "92 dB", "Âm thanh sống động, thiết kế hiện đại.", 1, 25, 6, "Tai nghe", null, "Bảo hành 24 tháng" },
                    { 7L, "Hộp đựng, cáp sạc", "20 giờ", "Wireless", new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tai nghe không dây với công nghệ tiên tiến, thời lượng pin lên đến 20 giờ.", "20Hz - 20kHz", "Loa Marshall Willen 2 (Chính Hãng ASH)", 2990000m, "104 dB", "Kết nối ổn định, thời gian sử dụng lâu dài.", 1, 45, 1, "Loa", null, "Bảo hành 12 tháng" },
                    { 8L, "Cáp, bộ chuyển đổi", "17 giờ cho mỗi lần sạc", "Wireless", new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Loa Marshall Middleton là chiếc loa di động nhỏ gọn với âm thanh mạnh mẽ và sống động mà chỉ Marshall có thể đem lại. Marshall Middleton sử dụng công nghệ True Stereophonic, một dạng âm thanh đa hướng độc đáo từ Marshall giúp bạn trải nghiệm âm thanh 360° t tuyệt vời dù ở bất kì vị trí nào trong hơn 20 giờ chơi nhạc. Đặc biệt loa Marshall Middleton tích hợp công nghệ cho phép ghép nhiều loa với nhau đem đến trải nghiệm âm thanh đa chiều. Marshall Middleton có thể được sử dụng thoải mái cả ở nhà hay ở bất kì nơi nào với các không gian khác nhau.", "19Hz - 21kHz", "Loa Marshall Middleton", 8090000m, "103 dB", "Âm thanh ổn định, thiết kế tiện lợi.", 1, 5, 1, "Loa", null, "Bảo hành 12 tháng" },
                    { 9L, "Cáp, adapter, ốp bảo vệ", null, null, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bộ phụ kiện đa năng cho tai nghe, gồm cáp, adapter và ốp bảo vệ.", null, "Marshall Bộ Phụ Kiện Tai nghe", 29.99m, null, "Tăng cường tuổi thọ cho tai nghe, dễ sử dụng.", 1, 100, 3, "Phụ kiện", null, "Không bảo hành" },
                    { 10L, "Túi đựng", null, null, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Túi đựng cao cấp giúp bảo vệ tai nghe khỏi va đập và trầy xước.", null, "Marshall Túi Đựng Tai nghe", 39.99m, null, "Chất liệu bền, thiết kế thời trang.", 1, 80, 3, "Phụ kiện", null, "Bảo hành 6 tháng" },
                    { 11L, "Bộ sạc", null, null, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bộ sạc chuyên dụng cho loa Marshall, thiết kế để sạc nhanh và an toàn.", null, "Marshall Bộ Sạc Loa", 49.99m, null, "Sạc nhanh, thiết kế nhỏ gọn.", 1, 35, 3, "Phụ kiện", null, "Bảo hành 6 tháng" },
                    { 12L, "Điều khiển từ xa", null, "Wireless", new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bộ điều khiển từ xa cho loa, giúp điều chỉnh âm lượng và chuyển đổi nguồn nhạc.", null, "Marshall Bộ Điều Khiển từ Xa", 59.99m, null, "Tiện lợi, dễ sử dụng, tương thích với nhiều thiết bị.", 1, 40, 3, "Phụ kiện", null, "Bảo hành 12 tháng" },
                    { 13L, "Cáp kết nối", null, null, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cáp kết nối chất lượng cao, đảm bảo truyền tải âm thanh mượt mà và ổn định.", null, "Marshall Cáp Kết Nối", 19.99m, null, "Bền bỉ, khả năng truyền dẫn cao.", 1, 150, 3, "Phụ kiện", null, "Không bảo hành" },
                    { 14L, "Đế sạc", null, null, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Đế sạc dành cho tai nghe không dây, cho phép sạc nhanh và an toàn.", null, "Marshall Đế Sạc Tai nghe", 34.99m, null, "Thiết kế độc đáo, sạc nhanh.", 1, 45, 3, "Phụ kiện", null, "Bảo hành 6 tháng" },
                    { 15L, "Cáp, hướng dẫn sử dụng", null, "Có dây", new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hệ thống loa home theater cho trải nghiệm giải trí tuyệt vời tại gia với âm thanh vòm sống động.", "40Hz - 20kHz", "Marshall Hệ Thống Loa Home Theater", 499.99m, "98 dB", "Âm thanh sống động, dễ dàng lắp đặt.", 1, 20, 2, "Loa", null, "Bảo hành 24 tháng" },
                    { 16L, "Cáp kết nối, đệm tai", null, "Có dây", new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tai nghe headset gaming với micro tích hợp, mang lại âm thanh rõ ràng và trải nghiệm chơi game sống động.", "20Hz - 20kHz", "Marshall Tai nghe Headset Gaming", 159.99m, "103 dB", "Thiết kế chắc chắn, micro rõ ràng.", 1, 30, 1, "Tai nghe", null, "Bảo hành 12 tháng" },
                    { 17L, "Cáp sạc, giá đỡ", "8 giờ", "Bluetooth", new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Loa karaoke với hiệu ứng ánh sáng độc đáo, mang lại trải nghiệm âm nhạc sôi động cho các buổi tiệc.", "45Hz - 19kHz", "Marshall Loa Karaoke", 279.99m, "96 dB", "Hiệu ứng ánh sáng, âm thanh mạnh mẽ.", 1, 25, 2, "Loa", null, "Bảo hành 12 tháng" },
                    { 18L, "Cáp, giá đỡ, túi đựng", null, null, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bộ phụ kiện loa gồm cáp kết nối, giá đỡ và túi đựng, hỗ trợ tối đa cho trải nghiệm âm nhạc.", null, "Marshall Bộ Phụ Kiện Loa", 39.99m, null, "Thiết kế chắc chắn, dễ sử dụng.", 1, 90, 3, "Phụ kiện", null, "Bảo hành 6 tháng" },
                    { 19L, "Cáp kết nối, đệm tai", null, "Có dây", new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tai nghe studio chuyên nghiệp, cung cấp âm thanh trung thực cho việc thu âm và xử lý nhạc.", "20Hz - 20kHz", "Marshall Tai nghe Studio", 189.99m, "106 dB", "Âm thanh chuẩn studio, thiết kế bền bỉ.", 1, 35, 1, "Tai nghe", null, "Bảo hành 12 tháng" },
                    { 20L, "Cáp sạc, hướng dẫn sử dụng", "10 giờ", "Bluetooth", new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Loa Dolby sử dụng công nghệ tiên tiến, mang lại trải nghiệm âm thanh sống động và mượt mà.", "40Hz - 20kHz", "Marshall Loa Dolby", 399.99m, "97 dB", "Công nghệ Dolby, âm thanh sống động, thiết kế hiện đại.", 1, 15, 2, "Loa", null, "Bảo hành 24 tháng" }
                });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "Id", "CreatedAt", "ProductId", "UpdatedAt", "Url" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, "https://soundway.vn/wp-content/uploads/2024/10/2.png" },
                    { 2L, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, "https://soundway.vn/wp-content/uploads/2024/10/major-v-two-asset-hybrid-09.png" },
                    { 3L, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, "https://soundway.vn/wp-content/uploads/2024/10/major-v-two-asset-hybrid-03-lon.png" },
                    { 4L, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, "https://soundway.vn/wp-content/uploads/2024/10/5-major-v-brown-lifestyle-desktop-trung-binh.png" },
                    { 5L, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, null, "https://soundway.vn/wp-content/uploads/2021/09/Tang-ngay-01-goi-Marshall-11.png" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2L);
        }
    }
}
