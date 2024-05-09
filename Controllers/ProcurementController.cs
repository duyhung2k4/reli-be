using _2reli_api.Entity;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace _2reli_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcurementController : ControllerBase
    {
        public readonly string _connectionString = "Server=srv515925;Port=3306;Database=2reli_database;Uid=root;Pwd=ubuntu123;";
        [HttpPost]
        public IActionResult AddProcurement([FromBody] Procurement procurement)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    var sql = @"INSERT INTO procurement_product (product_name, product_descr, product_quantity, product_status, product_price, turn_way, user_id, product_handle) 
                                VALUES (@Product_name, @Product_descr, @Product_quantity, @Product_status, @Product_price, @Turn_way,@User_id,@Product_handle)";
                    var parameters = new
                    {
                        procurement.Product_name,
                        procurement.Product_descr,
                        procurement.Product_quantity,
                        procurement.Product_status,
                        procurement.Product_price,
                        procurement.Turn_way,
                        procurement.User_id,
                        procurement.Product_handle,
                    };

                    connection.Execute(sql, parameters);

                    return Ok("Procurement added successfully");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding procurement: {ex.Message}");
            }
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProductById(int userId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var sql = "SELECT * FROM procurement_product WHERE user_id = @User_id";
                    var product = await connection.QueryAsync<Procurement>(sql, new { User_id = userId });

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
        [HttpPost("images/")]
        public async Task<IActionResult> AddProcurementProductImage([FromBody] ProductImg productImage)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var selectSql = "SELECT id FROM procurement_product ORDER BY id DESC LIMIT 1;";
                    var productId = await connection.QueryFirstOrDefaultAsync<int>(selectSql);
                    var sql = @"INSERT INTO procurement_product_images 
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
        [HttpGet("images/{productId}")]
        public async Task<IActionResult> GetProcurementProductImagesById(int productId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT product_image FROM procurement_product_images WHERE product_id = @ProductId";
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
