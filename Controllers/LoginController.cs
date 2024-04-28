using _2reli_api;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _2reli_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {   
        private readonly IConfiguration _configuration;
        private readonly string _connectionString = "Server=108.181.197.189;Port=19793;Database=reli;Uid=root;Pwd=duyhung2004;";
        public class UserRes
        {
            public int id { get; set; }
            public string nickname { get; set; }
            public string Jwt { get; set; }

        }
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            var user = Authenticate(userLogin);
            if(user != null) {
                var token = Generate(user);
              
                user.Jwt = token;
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = false,
                    Expires = DateTimeOffset.Now.AddDays(7), // Thời gian sống của cookie
                    SameSite = SameSiteMode.None, // Cài đặt SameSite tùy thuộc vào yêu cầu của ứng dụng
                    Secure = false // Đảm bảo cookie chỉ được gửi qua HTTPS
                };
                Response.Cookies.Append("userData", user.Id.ToString(), cookieOptions);
                Response.Cookies.Append("Jwt", token, cookieOptions);
                var userRes = new UserRes
                {
                    id = user.Id,
                    nickname = user.Nickname,
                    Jwt = token
                };

                return Ok(userRes);

            }
            return NotFound("User not found.");
        }
        private string Generate(User user)
        {
            var sercuityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(sercuityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: new[] { new Claim(ClaimTypes.Name, user.Nickname) },
                expires: DateTime.Now.AddHours(2), // Thời gian hết hạn của token
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        private User Authenticate(UserLogin userLogin)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var query = "SELECT * FROM \"user\" WHERE Nickname = @Nickname AND Password = @Password";;
                var parameters = new { Nickname = userLogin.Nickname, Password = userLogin.Password };
                var result = connection.QueryFirstOrDefault<User>(query, parameters);
                if (result != null) return result;
                else return null;
            }
        }

    }
}
