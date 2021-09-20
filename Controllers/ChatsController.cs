using chat.sssu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace chat.sssu.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatsController : Controller
    {
        private readonly DataBaseContext db;

        public ChatsController(DataBaseContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<Array> GetAll()
        {
            return await db.Chats.ToArrayAsync();
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> CreateChat(Chat chat)
        {
            await db.Chats.AddAsync(chat);
            await db.SaveChangesAsync();
            return Ok();
        }

    }
}
