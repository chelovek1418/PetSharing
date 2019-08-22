using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PetSharing.Domain.Services
{
    public interface IService<T>
    {
        Task<IEnumerable<T>> GetBySub(int skip, string id);
        Task<IEnumerable<T>> GetAll(int skip);
        Task<T> GetById(int id);
        Task<int> Create(T obj);
        Task Update(T obj);
        Task Delete(int id);
    }
}
