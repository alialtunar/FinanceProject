using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceProject.DataAccesLayer.Abstract
{
    public interface IGenericDal<T> where T : class
    {
        Task InsertAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(int id);

        Task<T> GetByIdAsync(int id);

        Task<List<T>> GetAllAsync();
    }
}
