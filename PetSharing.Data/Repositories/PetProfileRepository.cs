﻿using Microsoft.EntityFrameworkCore;
using PetSharing.Data.Contexts;
using System.Collections.Generic;
using PetSharing.Data.Entities;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace PetSharing.Data.Repositories
{

    public class PetProfileRepository : IRepository<PetProfile>
    {
        private PetSharingDbContext db;

        public PetProfileRepository(PetSharingDbContext context)
        {
            db = context;
        }

        public async Task<IEnumerable<PetProfile>> GetBySub(int skip, string id)
        {
            return await db.Users.SelectMany(x => x.Subscriptions
            .Where(y => y.UserId == id))
            .Select(z => z.PetProfile)
            .Skip(skip)
            .Take(20)
            .ToListAsync();
        }

        public async Task<IEnumerable<PetProfile>> GetAllAsync(int skip)
        {
            return await db.PetProfiles.Skip(skip).Take(20).ToListAsync();
        }

        public async Task<PetProfile> GetAsync(int id)
        {
            return await db.PetProfiles.Include(p=>p.Posts).Include(f=>f.Folowers).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> CreateAsync(PetProfile pet)
        {
            await db.PetProfiles.AddAsync(pet);
            await db.SaveChangesAsync();
            return pet.Id;
        }

        public async Task UpdateAsync(PetProfile pet)
        {
            db.PetProfiles.Update(pet);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var profile = await db.PetProfiles.Include(x => x.Posts).ThenInclude(p => p.Comments).Include(y=>y.Folowers).FirstOrDefaultAsync(d => d.Id == id);
            profile.Posts.ForEach(x => { x.Comments.ForEach(c => db.Comments.Remove(c)); db.Posts.Remove(x); });
            profile.Folowers.ForEach(x => profile.Folowers.Remove(x));
            db.PetProfiles.Remove(profile);
            await db.SaveChangesAsync();
        }
    }
}
