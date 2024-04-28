using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace _2reli_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {

        private readonly string _connectionString = "Server=mysql-170726-0.cloudclusters.net;Port=15658;Database=2reli_database;Uid=admin;Pwd=hN8U2cQv;";

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCartByUserId(int userId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT * FROM cart WHERE user_id = @UserId";
                    var parameters = new { UserId = userId };

                    var cartItems = await connection.QueryAsync(@"SELECT * FROM cart WHERE user_id = @UserId", parameters);
                    return Ok(cartItems);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving cart items: {ex.Message}");
            }
        }
        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng
        /// </summary>
        /// <param name="cart"></param>
        /// <returns></returns>
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(Cart cart)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"INSERT INTO cart (user_id, product_id, quantity, product_price, product_status,product_type,product_name, product_seller) 
                                VALUES (@User_id, @Product_id, @Quantity, @Product_price, @Product_status, @Product_type,@Product_name,@product_seller)";
                    var parameters = new
                    {
                        cart.User_id,
                        cart.Product_id,
                        cart.Quantity,
                        cart.Product_price,
                        cart.Product_status,
                        cart.Product_type,
                        cart.Product_name,
                        cart.Product_seller,
                    };

                    await connection.ExecuteAsync(sql, parameters);

                    return Ok("Product added to cart successfully");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding product to cart: {ex.Message}");
            }
        }
        [HttpDelete("{cartId}")]
        public async Task<IActionResult> RemoveCartItem(int cartId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "DELETE FROM cart WHERE id = @CartId";
                    var parameters = new { CartId = cartId };

                    await connection.ExecuteAsync(sql, parameters);

                    return Ok("Cart item removed successfully");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error removing cart item: {ex.Message}");
            }
        }
    }
}