using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Monetrack.Shared.Models;

namespace Monetrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IDbConnection _db;

        public AuthController(IDbConnection db)
        {
            _db = db;
        }

        [HttpPost("register")]
        public IActionResult Register(string username, string email, string password)
        {
            try
            {
                string newId = Guid.NewGuid().ToString();
                string sql = @"INSERT INTO Users (Id, Username, Email, PasswordHash)
                               VALUES (@Id,@Username,@Email,@PasswordHash)";

                _db.Execute(sql, new
                {
                    Id = newId,
                    Username = username,
                    Email = email,
                    PasswordHash = password
                });
                return Ok(new { message = "Пользователь создан в БД!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new {error =  ex.Message});
            }
        }
        [HttpPost("login")]
        public IActionResult Login(string login, string password)
        {
            try
            {
                string sql = "SELECT * FROM Users WHERE (Username = @Login OR Email = @Login) AND PasswordHash = @Password";

                var user = _db.QueryFirstOrDefault<User>(sql, new { Login = login, Password = password });

                if (user != null)
                {
                    return Ok(new { message = "Успешный вход!", userId = user.Id });
                }
                else
                {
                    return Unauthorized(new { error = "Неверный логин или пароль!" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
