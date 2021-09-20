using chat.sssu.Hubs;
using chat.sssu.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Saving;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;

namespace chat.sssu.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> chat;
        private readonly DataBaseContext db;
        private readonly IWebHostEnvironment _appEnvironment;

        public ChatController(IHubContext<ChatHub> _chat, DataBaseContext context, IWebHostEnvironment appEnvironment)
        {
            chat = _chat;
            db = context;
            _appEnvironment = appEnvironment;
        }

        [HttpGet]
        [Route("{id}/messages")]
        public async Task<Array> GetMessages(string id)
        {
            Array messages = await db.Messages.Where(x => x.ChatId == id).ToArrayAsync();
            List<PrepearedMessage> prepearedMessages = new();

            foreach (Message message in messages)
            {
                PrepearedMessage prepearedMessage = new PrepearedMessage()
                {
                    ChatId = message.ChatId,
                    SenderId = message.SenderId,
                    SenderName = message.SenderName,
                    Text = message.Text,
                    Timestamp = message.Timestamp
                };

                Models.File file = await db.Files.Where(x => x.MessageId == message.Id.ToString()).FirstOrDefaultAsync();

                if (file != null)
                {
                    prepearedMessage.File = file;
                }

                prepearedMessages.Add(prepearedMessage);
            }

            return prepearedMessages.ToArray();
        }

        [HttpPost]
        [Route("join")]
        public async Task<ActionResult> JoinChat(ChatConnection data)
        {
            await chat.Groups.AddToGroupAsync(data.ConnectionId, data.ChatId);
            return Ok();
        }

        [HttpPost]
        [Route("leave")]
        public async Task<ActionResult> LeaveChat(ChatConnection data)
        {
            await chat.Groups.RemoveFromGroupAsync(data.ConnectionId, data.ChatId);
            return Ok();
        }

        [HttpPost]
        [Route("send")]
        public async Task<ActionResult> SendMessage([FromForm]UploadedMessage uploadedMessage)
        {

            Message message = new Message()
            {
                ChatId = uploadedMessage.ChatId,
                SenderId = uploadedMessage.SenderId,
                SenderName = uploadedMessage.SenderName,
                Text = uploadedMessage.Text,
                Date = DateTime.Now,
                Timestamp = uploadedMessage.Timestamp
            };
            await db.Messages.AddAsync(message);
            await db.SaveChangesAsync();

            PrepearedMessage prepearedMessage = new PrepearedMessage()
            {
                ChatId = message.ChatId,
                SenderId = message.SenderId,
                SenderName = message.SenderName,
                Text = message.Text,
                Timestamp = message.Timestamp,
            };


            IFormFile file = uploadedMessage.File;

            if (file != null)
            {               
                string dir = _appEnvironment.WebRootPath + "\\Files\\";

                string date = "[" + DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss") + "]";

                string docDBPath = "Docs\\" + date + file.FileName;
                string previewDBPath = "Previews\\" + date + Path.GetFileNameWithoutExtension(file.FileName) + ".png";


                string docFullPath = dir + docDBPath;
                string previewFullPath = dir + previewDBPath;

                using (var fileStream = new FileStream(docFullPath, FileMode.Create))
                {
                    await uploadedMessage.File.CopyToAsync(fileStream);
                }

                Document doc = new Document(docFullPath);

                ImageSaveOptions options = new ImageSaveOptions(SaveFormat.Png);
                options.Resolution = 160;

                doc.Save(previewFullPath, options);

                Models.File newFile = new()
                {
                    MessageId = message.Id.ToString(),
                    Name = file.FileName,
                    Path = docDBPath,
                    Preview = previewDBPath
                };

                await db.Files.AddAsync(newFile);
                await db.SaveChangesAsync();

                prepearedMessage.File = newFile;
            }

            await chat.Clients.Group(prepearedMessage.ChatId).SendAsync("ReceiveMessage", prepearedMessage);

            return Ok();
        }
    }
}
