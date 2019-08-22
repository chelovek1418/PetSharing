using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetSharing.Data.Repositories
{
    public interface IRepository<T> where T : class 
    {
        Task<IEnumerable<T>> GetBySub(int skip, string id);
        Task<IEnumerable<T>> GetAllAsync(int skip);
        Task<T> GetAsync(int id);
        Task<int> CreateAsync(T item);
        Task UpdateAsync(T item);
        Task DeleteAsync(int id);
    }
}
