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
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetAllAsync(int postId, int skip);
        Task<Comment> GetAsync(int id);
        Task DeleteAsync(int id);
        Task CreateAsync(Comment comment);
    }

    public class CommentRepository : ICommentRepository
    {
        private PetSharingDbContext db;

        public CommentRepository(PetSharingDbContext context)
        {
            db = context;
        }



        public async Task<IEnumerable<Comment>> GetAllAsync(int postId, int skip)
        {
            return (await db.Posts.FirstOrDefaultAsync(x => x.Id == postId)).Comments.OrderByDescending(y => y.Date).Skip(skip).Take(30);
        }

        public async Task<Comment> GetAsync(int id) => await db.Comments.FirstOrDefaultAsync(x => x.Id == id);

        public async Task DeleteAsync (int id)
        {
            db.Comments.Remove(await db.Comments.FirstOrDefaultAsync(x => x.Id == id));
            await db.SaveChangesAsync();
        }

        public async Task CreateAsync(Comment comment)
        {
            await db.Comments.AddAsync(comment);
            await db.SaveChangesAsync();
        }
    }
}
