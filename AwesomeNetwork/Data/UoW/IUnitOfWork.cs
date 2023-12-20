using AwesomeNetwork.Data.Repository;
using AwesomeNetwork.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeNetwork.Data.UoW
{
    public interface IUnitOfWork: IDisposable
    {
        
        int SaveChanges(bool ensureAutoHistory = false);

        IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class;
        List<Post> GetPosts(string userId);
    }
}
