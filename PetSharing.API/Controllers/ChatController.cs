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
        private IUserService _userService;
        private IMessageService _messageService;
        IHubContext<ChatHub> _hubContext;

        public ChatController(IUserService userService, IMessageService messageService, IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
            _messageService = messageService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            return Ok((await _userService.GetChats(User)).Select(x => x.ToContract()).ToList());
        }

        public async Task<IActionResult> GetChat(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            var user = await _userService.FindById(id);
            var messages = (await _messageService.GetMessages((await _userService.GetCurrentUserAsync(User)).Id, id)).Select(x => x.ToContract()).ToList();
            var chat = new ChatContract { Date = messages.FirstOrDefault().Date, Name = user.UserName, PicUrl = user.PicUrl, LastMessage = messages.FirstOrDefault().Text }.Messages = messages;
            return Ok(chat);
        }

        public async Task<IActionResult> DeleteChat(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();
            await _messageService.DeleteChat((await _userService.GetCurrentUserAsync(User)).Id, id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SendMessage(MessageContract message)
        {
            var userId = (await _userService.GetCurrentUserAsync(User)).Id;
            if (userId != message.ReceiverId)
                await _hubContext.Clients.User(userId).SendAsync("Receive", message);
            await _hubContext.Clients.User(message.ReceiverId).SendAsync("Receive", message);
            await _messageService.Create(new MessageDto { Date = DateTime.Now, Text = message.Text, ReceiverId = message.ReceiverId });
            return RedirectToAction();
        }

        public async Task<IActionResult> DeleteMessage(int? id)
        {
            if (id == null)
                return BadRequest();
            await _messageService.Delete((int)id);
            return RedirectToAction();
        }
    }
}