using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PetSharing.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetSharing.API.SignalR
{
    public class ChatHub : Hub { }

    public class CustomUserIdProvider : IUserIdProvider
    {
        readonly IUserService _userService;

        public CustomUserIdProvider(IUserService userService)
        {
            _userService = userService;
        }

        public virtual string GetUserId(HubConnectionContext connection)
        {
            return _userService.GetCurrentUserAsync(connection.User).Result.Id;
        }
    }
}
