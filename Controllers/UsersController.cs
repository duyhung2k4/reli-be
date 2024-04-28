using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace _2reli_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<User>> GetUsers()
        {
        var connectionString = "Server=108.181.197.189;Port=19793;Database=reli;Uid=root;Pwd=duyhung2004;";
            var connecttion = new NpgsqlConnection(connectionString);
            var sql = "SELECT * FROM User";
            var result = await connecttion.QueryAsync<User>(sql);
            return result;
        }
        private readonly string _connectionString = "Server=108.181.197.189;Port=19793;Database=reli;Uid=root;Pwd=duyhung2004;";

        [HttpPost]
        public async Task<IActionResult> AddUser(User newUser)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"INSERT INTO User (name, password, phone_number, nickname ) 
                                VALUES (@Name, @Password, @Phone_Number, @Nickname)";
                    var parameters = new
                    {
                        newUser.Name,
                        newUser.Password,
                        newUser.Phone_Number,
                        newUser.Nickname,
                    };

                    await connection.ExecuteAsync(sql, parameters);

                    await connection.ExecuteAsync(sql, newUser);

                    return Ok("User added successfully");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error adding user: " + ex.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT * FROM User WHERE id = @Id";

                    var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });

                    if (user == null)
                    {
                        return NotFound();
                    }

                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching user: " + ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdate updatedUser)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"UPDATE user 
                        SET name = @Name,
                            phone_number = @Phone_Number,
                            nickname = @Nickname,
                            email = @Email,
                            province = @Province,
                            district = @District,
                            ward = @Ward,
                            address = @Address,
                            dob = @Dob,
                            gender = @Gender
                        WHERE id = @Id";

                    var parameters = new
                    {
                        updatedUser.Name,
                        updatedUser.Phone_Number,
                        updatedUser.Nickname,
                        updatedUser.Email,
                        updatedUser.Province,
                        updatedUser.District,
                        updatedUser.Ward,
                        updatedUser.Address,
                        updatedUser.Dob,
                        updatedUser.Gender,
                        Id = id
                    };

                    await connection.ExecuteAsync(sql, parameters);

                    return Ok("User updated successfully");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating user: {ex.Message}");
            }
        }
        
        [HttpPost("upload-avatar")]
        public async Task<IActionResult> UploadAvatar(UserAva userava)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Check if the user already has an avatar
                    var checkAvatarSql = "SELECT COUNT(*) FROM user_ava WHERE user_id = @User_id";
                    var avatarCount = await connection.ExecuteScalarAsync<int>(checkAvatarSql, new { userava.User_id });

                    if (avatarCount > 0)
                    {
                        // Update the existing avatar
                        var updateAvatarSql = "UPDATE user_ava SET user_ava = @User_ava WHERE user_id = @User_id";
                        await connection.ExecuteAsync(updateAvatarSql, new { userava.User_ava, userava.User_id });
                    }
                    else
                    {
                        // Insert a new avatar
                        var insertAvatarSql = "INSERT INTO user_ava (user_ava, user_id) VALUES (@User_ava, @User_id)";
                        await connection.ExecuteAsync(insertAvatarSql, new { userava.User_ava, userava.User_id });
                    }

                    return Ok("Avatar uploaded successfully");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading avatar: " + ex.Message);
            }
        }
        /// <summary>
        /// Lấy ra ava người dùng tương ứng với id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}/avatar")]
        public async Task<IActionResult> GetAvatar(int userId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT user_ava FROM user_ava WHERE user_id = @User_id";
                    var userAvatar = await connection.QueryFirstOrDefaultAsync<string>(sql, new { User_id = userId });

                    if (userAvatar == null)
                    {
                        return Ok(null);
                    }

                    return Ok(userAvatar);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching avatar: " + ex.Message);
            }
        }
    }
}
