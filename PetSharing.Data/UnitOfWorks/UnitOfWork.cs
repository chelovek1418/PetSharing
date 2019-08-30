using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetSharing.Data.Contexts;
using PetSharing.Data.Entities;
using PetSharing.Data.Repositories;
using System;

namespace PetSharing.Data.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        UserManager<User> UserManager { get; }
        RoleManager<IdentityRole> RoleManager { get; }
        SignInManager<User> SignInManager { get; }
        IRepository<PetProfile> PetProfiles { get; }
        IRepository<Post> Posts { get; }
        IMessageRepository Messages { get; }
        ICommentRepository Comments { get;  }
        void Save();
    }
    public class EFUnitOfWork : IUnitOfWork
    {
        PetSharingDbContext db;
        PetProfileRepository petProfileRepository;
        PostRepository postRepository;
        CommentRepository commentRepository;
        MessageRepository messageRepository;

        public EFUnitOfWork(DbContextOptions<PetSharingDbContext> options, UserManager<User> user, RoleManager<IdentityRole> role, SignInManager<User> signIn)
        {
            db = new PetSharingDbContext(options);
            UserManager = user;
            RoleManager = role;
            SignInManager = signIn;
        }

        public UserManager<User> UserManager { get; }

        public RoleManager<IdentityRole> RoleManager { get; }

        public SignInManager<User> SignInManager { get; }

        public ICommentRepository Comments
        {
            get
            {
                if (commentRepository == null)
                    commentRepository = new CommentRepository(db);
                return commentRepository;
            }
        }

        public IMessageRepository Messages
        {
            get
            {
                if (messageRepository == null)
                    messageRepository = new MessageRepository(db);
                return messageRepository;
            }
        }

        public IRepository<Post> Posts
        {
            get
            {
                if (postRepository == null)
                    postRepository = new PostRepository(db);
                return postRepository;
            }
        }

        public IRepository<PetProfile> PetProfiles
        {
            get
            {
                if (petProfileRepository == null)
                    petProfileRepository = new PetProfileRepository(db);
                return petProfileRepository;
            }
        }

        public async void Save()
        {
            await db.SaveChangesAsync();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    UserManager.Dispose();
                    RoleManager.Dispose();
                    db.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}