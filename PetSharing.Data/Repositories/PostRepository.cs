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
    public class PostRepository : IRepository<Post>
    {
        private PetSharingDbContext db;

        public PostRepository(PetSharingDbContext context)
        {
            db = context;
        }

        public async Task<IEnumerable<Post>> GetBySub(int skip, string id)
        {
            return await db.Users.SelectMany(x => x.Subscriptions
            .Where(y => y.UserId == id))
            .SelectMany(z=>z.PetProfile.Posts)
            .OrderByDescending(i=>i.Date)
            .Skip(skip)
            .Take(20)
            .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetAllAsync(int skip)
        {
            return await db.Posts.Skip(skip).Take(20).ToListAsync();
        }

        public async Task<Post> GetAsync(int id)
        {
            return await db.Posts.Include(p => p.Comments).ThenInclude(y=>y.Select(i=> i.User)).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> CreateAsync(Post post)
        {
            await db.Posts.AddAsync(post);
            await db.SaveChangesAsync();
            return post.Id;
        }

        public async Task UpdateAsync(Post post)
        {
            db.Entry(post).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var post = await db.Posts.Include(x => x.Comments).FirstOrDefaultAsync(d => d.Id == id);
            post.Comments.ForEach(x => db.Comments.Remove(x));
            db.Posts.Remove(post);
            await db.SaveChangesAsync();
        }
    }
}
