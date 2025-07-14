using EasyPay.Common;
using System.Linq.Expressions;

namespace EasyPay.Common
{
    public interface IRepositoryBase<TEntity, TKey> where TEntity : EntityBase<TKey>
    {
        #region Basic CRUD Operations
        Task<TEntity> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task UpdateAsync(TEntity entity);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities);
        Task DeleteAsync(TKey id, bool isHardDelete = false);
        Task DeleteAsync(TEntity entity, bool isHardDelete = false);
        Task DeleteRangeAsync(IEnumerable<TKey> ids, bool isHardDelete = false);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool isHardDelete = false);
        #endregion

        #region Advanced Query Operations
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> predicate,
                                                Expression<Func<TEntity, TResult>> selector);
        #endregion

        #region Specification Pattern
        Task<IEnumerable<TEntity>> GetBySpecificationAsync(ISpecification<TEntity> spec);
        Task<int> CountBySpecificationAsync(ISpecification<TEntity> spec);
        Task<TEntity> FirstOrDefaultBySpecificationAsync(ISpecification<TEntity> spec);
        #endregion

        #region Pagination
        Task<IPagedList<TEntity>> GetPagedListAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Expression<Func<TEntity, object>> orderBy = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true);

        Task<IPagedList<TResult>> GetPagedListAsync<TResult>(
            Expression<Func<TEntity, bool>> predicate = null,
            Expression<Func<TEntity, object>> orderBy = null,
            Expression<Func<TEntity, TResult>> selector = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true) where TResult : class;
        #endregion


        #region Transaction Support
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task<int> SaveChangesAsync();
        #endregion

        #region Advanced Features
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate = null,
            Expression<Func<TEntity, object>> orderBy = null,
            Expression<Func<TEntity, object>> orderByDescending = null,
            Expression<Func<TEntity, object>> groupBy = null,
            List<Expression<Func<TEntity, object>>> Includes = null,
                                bool disableTracking = true);

        Task ExecuteSqlRawAsync(string sql, params object[] parameters);
        Task<IEnumerable<TEntity>> FromSqlRawAsync(string sql, params object[] parameters);
        #endregion
    }
}