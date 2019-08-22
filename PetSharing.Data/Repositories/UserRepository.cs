using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetSharing.Data.Contexts;
using PetSharing.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetSharing.Data.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private PetSharingDbContext db;

        public UserRepository(PetSharingDbContext context)
        {
            db = context;
        }

        public IEnumerable<User> GetAll()
        {
            return db.Users;
        }

        public User Get(int id)
        {
            return db.Users.Find(id);
        }

        public int Create(User user)
        {
            db.Users.Add(user);
            int.TryParse(user.Id, out int id);
            return id;
        }

        public void Update(User user)
        {
            db.Entry(user).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            User user = db.Users.Find(id);
            if (user != null)
                db.Users.Remove(user);
        }
    }
}
