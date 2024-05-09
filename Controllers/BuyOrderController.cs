using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace _2reli_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuyOrderController : ControllerBase
    {
        private readonly string _connectionString = "Server=srv515925;Port=3306;Database=2reli_database;Uid=root;Pwd=ubuntu123;";

        [HttpPost] // Đánh dấu phương thức là HTTP POST
        public async Task<IActionResult> AddToBuyOderList(BuyOrderItem buyOrderItem) // Chỉnh sửa kiểu trả về và tham số
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"INSERT INTO buy_order (sell_name, product_name, product_quantity, product_price, product_status, buyer_id,product_id) 
                                VALUES (@Seller_name, @Product_name, @Product_quantity, @Product_price, @Product_status,@Buyer_id,@Product_id)";
                    var parameters = new
                    {
                        buyOrderItem.Seller_name,
                        buyOrderItem.Product_name,
                        buyOrderItem.Product_quantity,
                        buyOrderItem.Product_price,
                        buyOrderItem.Product_status,
                        buyOrderItem.Buyer_id,
                        buyOrderItem.Product_id,
                    };

                    await connection.ExecuteAsync(sql, parameters);

                    return Ok("Product added to buy order successfully");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding product to buy order: {ex.Message}");
            }
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBuyOrdersByUserId(int userId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT * FROM buy_order WHERE buyer_id = @Buyer_id";
                    var parameters = new { Buyer_id = userId };

                    var buyOrders = await connection.QueryAsync(sql, parameters);
                    return Ok(buyOrders);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving buy orders: {ex.Message}");
            }
        }
        [HttpGet("seller/{seller_name}")]
        public async Task<IActionResult> GetBuyOrdersBySellerName(string seller_name)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT * FROM buy_order WHERE sell_name = @Seller_name";
                    var parameters = new { Seller_name = seller_name };

                    var buyOrders = await connection.QueryAsync(sql, parameters);
                    return Ok(buyOrders);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving buy orders: {ex.Message}");
            }
        }
        [HttpPut("{id}/update-status")] 
        public async Task<IActionResult> UpdateBuyOrderStatus(int id) 
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Truy vấn để cập nhật product_status của đơn hàng mua
                    var sql = @"UPDATE buy_order 
                                SET product_status = product_status + 1 
                                WHERE id = @Id";
                    var parameters = new { Id = id };

                    await connection.ExecuteAsync(sql, parameters);

                    return Ok("Buy order status updated successfully");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating buy order status: {ex.Message}");
            }
        }
        }
}
