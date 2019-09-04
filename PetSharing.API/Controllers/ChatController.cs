using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PetSharing.API.Extensions;
using PetSharing.API.SignalR;
using PetSharing.Contracts;
using PetSharing.Domain.Dtos;
using PetSharing.Domain.Services;

namespace PetSharing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        readonly IUserService _userService;
        readonly IMessageService _messageService;
        readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IUserService userService, IMessageService messageService, IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
            _messageService = messageService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            return Ok((await _userService.GetChats(User.Claims.FirstOrDefault(x => x.Type == "UserID").Value)).Select(x => x.ToContract()).ToList());
        }

        [Route("get")]
        public async Task<IActionResult> GetChat(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();
            var user = _userService.FindById(id);
            var messages = (await _messageService.GetMessages(User.Claims.FirstOrDefault(x => x.Type == "UserID").Value, id)).Select(x => x.ToContract()).ToList();
            return Ok(new ChatContract { Date = messages.FirstOrDefault().Date, Name = user.Result.UserName, PicUrl = user.Result.PicUrl, LastMessage = messages.FirstOrDefault().Text }.Messages = messages);
        }

        [Route("delete")]
        [HttpDelete]
        public async Task<IActionResult> DeleteChat(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();
            await _messageService.DeleteChat(User.Claims.FirstOrDefault(x => x.Type == "UserID").Value, id);
            return Ok();
        }

        [Route("send")]
        [HttpPost]
        public async Task<IActionResult> SendMessage(MessageContract message)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == "UserID").Value;
            if (userId != message.ReceiverId)
                await _hubContext.Clients.User(userId).SendAsync("Receive", message);
            await _hubContext.Clients.User(message.ReceiverId).SendAsync("Receive", message);
            await _messageService.Create(new MessageDto { Date = DateTime.Now, Text = message.Text, ReceiverId = message.ReceiverId, UserId = userId });
            return Ok();
        }

        [Route("deletemsg")]
        [HttpDelete]
        public async Task<IActionResult> DeleteMessage(int? id)
        {
            if (id == null)
                return BadRequest();
            await _messageService.Delete((int)id);
            return Ok();
        }
    }
}