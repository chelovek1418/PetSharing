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
        IUnitOfWork _unitOfWork { get; set; }

        public MessageService(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task<IEnumerable<MessageDto>> GetMessages(string myId, string companionId)
        {
            return (await _unitOfWork.Messages.GetMessages(myId, companionId)).Select(x => x.ToDto());
        }

        public async Task DeleteChat(string myId, string chatId)
        {
            await _unitOfWork.Messages.DeleteChat(myId, chatId);
        }

        public async Task Create(MessageDto message)
        {
            await _unitOfWork.Messages.Create(message.ToEntity());
        }

        public async Task Delete(int id)
        {
            await _unitOfWork.Messages.Delete(id);
        }
    }
}
