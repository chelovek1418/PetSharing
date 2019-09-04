using Microsoft.EntityFrameworkCore;
using PetSharing.Data.Contexts;
using PetSharing.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetSharing.Data.Repositories
{
    public interface IMessageRepository
    {
        Task Create(Message message);
        Task<IEnumerable<Message>> GetMessages(string myId, string comapanionId);
        Task Delete(int id);
        Task DeleteChat(string myId, string chatId);
    }

    public class MessageRepository : IMessageRepository
    {
        readonly PetSharingDbContext db;

        public MessageRepository(PetSharingDbContext context)
        {
            db = context;
        }

        public async Task<IEnumerable<Message>> GetMessages(string myId, string comapanionId)
        {
            return (await db.Users.Include(y=>y.Messages.Where(m=>m.ReceiverId==comapanionId)).FirstOrDefaultAsync(x => x.Id == myId)).Messages.OrderByDescending(t=>t.Date);
        }

        public async Task Create(Message message)
        {
            await db.Messages.AddAsync(message);
            await db.SaveChangesAsync();
        }
        public async Task Delete(int id)
        {
            db.Messages.Remove(await db.Messages.FirstOrDefaultAsync(x => x.Id == id));
            await db.SaveChangesAsync();
        }

        public async Task DeleteChat(string myId, string chatId)
        {
            var mes = (await db.Users.Include(x => x.Messages.Where(y => y.ReceiverId == chatId)).FirstOrDefaultAsync(u => u.Id == myId)).Messages;
            mes.ForEach(x => db.Messages.Remove(x));
            await db.SaveChangesAsync();
        }
    }
}
