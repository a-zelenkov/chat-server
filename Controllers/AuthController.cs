using chat.sssu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace chat.sssu.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly DataBaseContext db;
        public AuthController(DataBaseContext context) =>
            db = context;


        [HttpGet]
        [Route("user")]
        public async Task<ActionResult<User>> GetUser()
        {
            if (HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                string token = HttpContext.Request.Headers["Authorization"];

                User user = await db.Users.FirstOrDefaultAsync(x => x.Token == token);
                
                if (user != null)
                    return Ok(user);
            }

            return Unauthorized("\"Unauthorized\"");
        }

        [HttpPost]
        [Route("registration")]
        public async Task<ActionResult<UserToken>> Registration(User user)
        {
            if (await db.Users.FirstOrDefaultAsync(x => x.Login == user.Login) != null)
                ModelState.AddModelError("login", "Этот логин уже занят");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            user.Password = GetHash(user.Password);
            user.Token = GetHash($"{user.Login}{DateTime.Now}");

            db.Users.Add(user);
            db.SaveChanges();

            UserToken response = new()
            {
                Token = user.Token
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<UserToken>> Login(UserLogin data)
        {
            User user = await db.Users.FirstOrDefaultAsync(x => x.Login == data.Login && x.Password == GetHash(data.Password));

            if (user == null)
            {
                ModelState.AddModelError("login", "Не удалось войти");
                ModelState.AddModelError("password", "Неверно указан логин или пароль");
                return BadRequest(ModelState);
            }

            string token = GetHash($"{user.Login}{DateTime.Now}");


            UserToken response = new()
            {
                Token = token
            };

            user.Token = token;

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return Ok(response);
        }

        [HttpGet]
        [Route("logout")]
        public async Task<ActionResult> LogOut()
        {
            if (HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                string token = HttpContext.Request.Headers["Authorization"];

                User user = await db.Users.FirstOrDefaultAsync(x => x.Token == token);

                if (user != null)
                {
                    string newToken = GetHash($"{user.Login}{DateTime.Now}");

                    UserToken response = new()
                    {
                        Token = newToken
                    };

                    user.Token = token;

                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();

                    return Ok(response);
                }
            }

            return Unauthorized("\"Unauthorized\"");
        }

        private static string GetHash(string input)
        {
            input += "sdfjwefoo452gk=";
            SHA512 sha = SHA512.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hash);
        }
    }
}