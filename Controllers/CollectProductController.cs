using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace _2reli_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectProductController : ControllerBase
    {
        private readonly string _connectionString = "Server=srv515925;Port=3306;Database=2reli_database;Uid=root;Pwd=ubuntu123;";
        [HttpPost]
        public async Task<IActionResult> AddCollectProduct(CollectProduct collectProduct)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"INSERT INTO collect_product (collect_way, user_id, price, product_name) 
                                VALUES (@Collect_way, @User_id, @Price, @Product_name)";
                    var parameters = new
                    {
                        collectProduct.Collect_way,
                        collectProduct.User_id,
                        collectProduct.Price,
                        collectProduct.Product_name
                    };

                    await connection.ExecuteAsync(sql, parameters);

                    return Ok("Collect product added successfully");
                }
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Thêm ảnh vào khi post sản phẩm
        /// </summary>
        /// <param name="productImage"></param>
        /// <returns></returns>
        [HttpPost("images/")]
        public async Task<IActionResult> AddCollectProductImage([FromBody] ProductImg productImage)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var selectSql = "SELECT id FROM collect_product ORDER BY id DESC LIMIT 1;";
                    var productId = await connection.QueryFirstOrDefaultAsync<int>(selectSql);
                    var sql = @"INSERT INTO collect_product_images 
                                (product_id, product_image) 
                                VALUES 
                                (@ProductId, @ImageData)";
                    await connection.ExecuteAsync(sql, new { ProductId = productId, productImage.ImageData });
                }

                return Ok("Product image added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding product image: {ex.Message}");
            }
        }
        [HttpGet("collectProduct/{userId}")]
        public async Task<IActionResult> GetCollectProductsByUserId(int userId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT * FROM collect_product WHERE user_id = @UserId";
                    var parameters = new { UserId = userId };

                    var collectProducts = await connection.QueryAsync(sql, parameters);
                    return Ok(collectProducts);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving collect products: {ex.Message}");
            }
        }
        [HttpGet("images/{productId}")]
        public async Task<IActionResult> GetCollectProductImagesById(int productId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT product_image FROM collect_product_images WHERE product_id = @ProductId";
                    var parameters = new { ProductId = productId };

                    var productImages = await connection.QueryAsync<string>(sql, parameters);
                    return Ok(productImages);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving collect product images: {ex.Message}");
            }
        }
    }
}
