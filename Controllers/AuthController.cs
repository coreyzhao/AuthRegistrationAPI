using System.Data;
using AuthRegistrationAPI.Data;
using AuthRegistrationAPI.Dtos;
using AuthRegistrationAPI.Helpers;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AuthRegistrationAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
        {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;

        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password != userForRegistration.PasswordConfirm)
            {
                return BadRequest("Passwords do not match");
            }

            string sqlCheckUserExists = $"SELECT Email FROM AppSchema.Auth WHERE Email = '{userForRegistration.Email}'";
            var existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

            if (existingUsers.Any())
            {
                return Conflict("User with this email already exists");
            }

            var passwordSalt = GenerateSalt();
            var passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

            string sqlAddAuth = $@"
                INSERT INTO AppSchema.Auth (Email, PasswordHash, PasswordSalt) 
                VALUES ('{userForRegistration.Email}', @PasswordHash, @PasswordSalt)";

            List<SqlParameter> sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@PasswordSalt", SqlDbType.VarBinary) { Value = passwordSalt },
                new SqlParameter("@PasswordHash", SqlDbType.VarBinary) { Value = passwordHash }
            };

            if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
            {
                string sqlAddUser = $@"
                    INSERT INTO AppSchema.Users (FirstName, LastName, Email, Gender, Active) 
                    VALUES ('{userForRegistration.FirstName}', '{userForRegistration.LastName}', '{userForRegistration.Email}', '{userForRegistration.Gender}', 1)";

                if (_dapper.ExecuteSql(sqlAddUser))
                {
                    return Ok();
                }

                return StatusCode(500, "Error: Unable to add the user.");
            }

            return StatusCode(500, "Error: Unable to authenticate the user.");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = $@"
                SELECT PasswordHash, PasswordSalt 
                FROM AppSchema.Auth 
                WHERE Email = '{userForLogin.Email}'";

            var userForConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

            if (userForConfirmation == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

            if (!passwordHash.SequenceEqual(userForConfirmation.PasswordHash))
            {
                return Unauthorized("Incorrect password");
            }

            string userIdSql = $"SELECT UserId FROM AppSchema.Users WHERE Email = '{userForLogin.Email}'";
            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string> {
                { "token", _authHelper.CreateToken(userId) }
            });
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userIdSql = $"SELECT UserId FROM AppSchema.Users WHERE UserId = '{User.FindFirst("userId")?.Value}'";
            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            if (userId == 0)
            {
                return Unauthorized("Invalid token.");
            }

            var token = _authHelper.CreateToken(userId);
            return Ok(new { token });
        }

        private byte[] GenerateSalt()
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }    
}