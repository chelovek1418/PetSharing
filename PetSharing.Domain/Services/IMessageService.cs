using Microsoft.AspNetCore.SignalR;
using PetSharing.Data.Entities;
using PetSharing.Data.UnitOfWorks;
using PetSharing.Domain.Dtos;
using PetSharing.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetSharing.Domain.Services
{
    public interface IMessageService
    {
        Task <IEnumerable<MessageDto>> GetMessages(string myId, string companionId);
        Task DeleteChat(string myId, string chatId);
        Task Create(MessageDto messege);
        Task Delete(int id);
    }

    public class MessageService : IMessageService
    {
        IUnitOfWork UnitOfWork { get; set; }

        public MessageService(IUnitOfWork uow)
        {
            UnitOfWork = uow;
        }

        public async Task<IEnumerable<MessageDto>> GetMessages(string myId, string companionId)
        {
            return (await UnitOfWork.Messages.GetMessages(myId, companionId)).Select(x => x.ToDto());
        }

        public async Task DeleteChat(string myId, string chatId)
        {
            await UnitOfWork.Messages.DeleteChat(myId, chatId);
        }

        public async Task Create(MessageDto message)
        {
            await UnitOfWork.Messages.Create(message.ToEntity());
        }

        public async Task Delete(int id)
        {
            await UnitOfWork.Messages.Delete(id);
        }
    }
}
