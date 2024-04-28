using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace _2reli_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellProductController : ControllerBase
    {   
        [HttpGet]
        public async Task<IEnumerable<SellProduct>> GetUsers()
        {
            var connectionString = "Server=mysql-170726-0.cloudclusters.net;Port=15658;Database=2reli_database;Uid=admin;Pwd=hN8U2cQv;";
            var connecttion = new MySqlConnection(connectionString);
            var sql = "SELECT * FROM sell_product";
            var result = await connecttion.QueryAsync<SellProduct>(sql);
            return result;
        }
        private readonly string _connectionString = "Server=mysql-170726-0.cloudclusters.net;Port=15658;Database=2reli_database;Uid=admin;Pwd=hN8U2cQv;";
        /// <summary>
        /// Lấy ảnh của sản phẩm theo id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet("images/{productId}")]
        public async Task<IActionResult> GetProductImages(int productId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = "SELECT image_data FROM product_images WHERE product_id = @ProductId";
                var images = await connection.QueryAsync<string>(sql, new { ProductId = productId });

                if (images == null || images.AsList().Count == 0)
                {
                    return NotFound();
                }

                return Ok(images);
            }
        }
        /// <summary>
        /// Lấy thông tin sản phẩm theo id sản phẩm
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var sql = "SELECT * FROM sell_product WHERE id = @ProductId";
                    var product = await connection.QueryFirstOrDefaultAsync<SellProduct>(sql, new { ProductId = productId });

                    if (product == null)
                    {
                        return NotFound();
                    }

                    return Ok(product);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error fetching product: {ex.Message}");
            }
        }

        /// <summary>
        /// Đăng bán sản phẩm
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] SellProduct product)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var sql = @"INSERT INTO sell_product 
                                (product_name, product_descr, product_status, product_type, product_size, product_quantity, product_price,user_id,sell_status) 
                                VALUES 
                                (@Product_name, @Product_descr, @Product_status, @Product_type, @Product_size, @Product_quantity, @Product_price, @User_id,@Sell_status)";
                    await connection.ExecuteAsync(sql, product);
                }

                return Ok("Product added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding product: {ex.Message}");
            }
        }
        /// <summary>
        /// Lấy danh sách id theo thứ tự bản ghi và số bản ghi
        /// </summary>
        /// <param thứ tự bản ghi="startRecord"></param>
        /// <param số bản ghi="count"></param>
        /// <returns></returns>
        [HttpGet("{startRecord}/{count}")]
        public async Task<IActionResult> GetProducts(int startRecord, int count)
        {   
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var sql = @"SELECT * FROM sell_product LIMIT @StartRecord, @Count";
                    var result = await connection.QueryAsync<SellProduct>(sql, new { StartRecord = startRecord, Count = count });

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error fetching products: {ex.Message}");
            }
        }
        /// <summary>
        /// Thêm ảnh vào khi post sản phẩm
        /// </summary>
        /// <param name="productImage"></param>
        /// <returns></returns>
        [HttpPost("images/")]
        public async Task<IActionResult> AddProductImage([FromBody] ProductImg productImage)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var selectSql = "SELECT id FROM sell_product ORDER BY id DESC LIMIT 1;";
                    var productId = await connection.QueryFirstOrDefaultAsync<int>(selectSql);
                    var sql = @"INSERT INTO product_images 
                                (product_id, image_data) 
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
        
    }
}
