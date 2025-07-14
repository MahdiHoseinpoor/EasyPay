using EasyPay.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Common
{
    public class RepositoryBase<TEntity, TKey> : IRepositoryBase<TEntity, TKey> where TEntity : EntityBase<TKey>
    {
        DbContext _context;
        DbSet<TEntity> _dbset;
        IDbContextTransaction _transaction;
        public RepositoryBase(DbContext context)
        {
            _context = context;
            _dbset = _context.Set<TEntity>();
        }
        #region Basic CRUD Operations
        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await _dbset.FindAsync(id);
        }
        public async Task<IEnumerable<TEntity>> GetAllAsync(){
            return await _dbset.ToListAsync();
        }
        public async Task AddAsync(TEntity entity){
            await _dbset.AddAsync(entity);
        }
        public async Task AddRangeAsync(IEnumerable<TEntity> entities){
            await _dbset.AddRangeAsync(entities);
        }
        public async Task UpdateAsync(TEntity entity){
            entity.ModifiedAt = DateTime.Now;
            _context.Update(entity);
        }
        public Task UpdateRangeAsync(IEnumerable<TEntity> entities){
            //TODO: Use Better way for set ModifiedAt
            foreach (var entity in entities)
            {
                entity.ModifiedAt = DateTime.Now;
            }
            _context.UpdateRange(entities);
            return Task.CompletedTask;
        }
        public async Task DeleteAsync(TKey id, bool isHardDelete = false){
            //TODO: replace exeption with result object error
            var entity =await GetByIdAsync(id);
            if (entity == null) throw new Exception("can't find entity with id="+id);
            await DeleteAsync(entity, isHardDelete);
        }
        public Task DeleteAsync(TEntity entity, bool isHardDelete = false){
            //TODO: replace exeption with result object error
            if (isHardDelete)
            {
                _dbset.Remove(entity);
            }
            else
            {
                if (entity is ISoftDeletable softDeleteableEntity)
                {
                    softDeleteableEntity.IsDeleted = true;
                    softDeleteableEntity.DeletedAt = DateTime.Now;
                }
                else
                {
                    throw new Exception("entity is not SoftDeleteable");
                }
            }
            return Task.CompletedTask;
        }
        public async Task DeleteRangeAsync(IEnumerable<TKey> ids, bool isHardDelete = false){

            var entities = await _dbset.Where(p=>ids.Contains(p.Id)).ToListAsync();
            await DeleteRangeAsync(entities, isHardDelete);
        }
        public Task DeleteRangeAsync(IEnumerable<TEntity> entities, bool isHardDelete = false){
            if (isHardDelete)
                _dbset.RemoveRange(entities);
            else
            {
                foreach (var entity in entities)
                    if (entity is ISoftDeletable soft)
                        soft.IsDeleted = true;
            }
            return Task.CompletedTask;
        }
        #endregion

        #region Advanced Query Operations
        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate){
            return await _dbset.AnyAsync(predicate);
        }
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null){
            return await _dbset.CountAsync(predicate);
        }
        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate){
            return await _dbset.FirstOrDefaultAsync(predicate);
        }
        public async Task<TResult> FirstOrDefaultAsync<TResult>(Expression<Func<TEntity, bool>> predicate,
                                                Expression<Func<TEntity, TResult>> selector){
            return await _dbset.Where(predicate).Select(selector).FirstOrDefaultAsync();
        }
        #endregion

        #region Specification Pattern
        private Task<IQueryable<TEntity>> ApplySpecificationAsync(ISpecification<TEntity> spec, bool disableTracking = true)
        {
            IQueryable<TEntity> query = Query(spec.Predicate, spec.OrderBy, spec.OrderByDescending, spec.GroupBy, spec.Includes, disableTracking);
            return Task.FromResult(query);
        }
        public async Task<IEnumerable<TEntity>> GetBySpecificationAsync(ISpecification<TEntity> spec){
            var query = await ApplySpecificationAsync(spec);
            return await query.ToListAsync();
        }
        public async Task<int> CountBySpecificationAsync(ISpecification<TEntity> spec){
            var query = await ApplySpecificationAsync(spec);
            return await query.CountAsync();
        }
        public async Task<TEntity> FirstOrDefaultBySpecificationAsync(ISpecification<TEntity> spec){
            var query = await ApplySpecificationAsync(spec);
            return await query.FirstOrDefaultAsync();
        }
        #endregion

        #region Pagination
        public async Task<IPagedList<TEntity>> GetPagedListAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Expression<Func<TEntity, object>> orderBy = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true){

            IQueryable<TEntity> query = Query(predicate, orderBy, disableTracking: disableTracking);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<TEntity>(items, totalCount, pageIndex, pageSize);

        }

        public async Task<IPagedList<TResult>> GetPagedListAsync<TResult>(
            Expression<Func<TEntity, bool>> predicate = null,
            Expression<Func<TEntity, object>> orderBy = null,
            Expression<Func<TEntity, TResult>> selector = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true) where TResult : class{
            IQueryable<TEntity> query = Query(predicate, orderBy, disableTracking: disableTracking);

            var totalCount = await query.CountAsync();

            var items = await query
                .Select(selector)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedList<TResult>(items, totalCount, pageIndex, pageSize);
        }
        #endregion

        #region Transaction Support
        public async Task BeginTransactionAsync(){
            if (_transaction == null) {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }
        public async Task CommitTransactionAsync(){
            try
            {
                if (_transaction != null)
                {
                    SaveChangesAsync();
                    _transaction?.Commit();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
            }
            finally
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                }
            }
        }
        public async Task RollbackTransactionAsync(){
            try
            {
                await _transaction?.RollbackAsync();
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }
        public async Task<int> SaveChangesAsync(){
            return await _context.SaveChangesAsync();
        }
        #endregion

        #region Advanced Features
        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate = null,
            Expression<Func<TEntity, object>> orderBy = null,
            Expression<Func<TEntity, object>> orderByDescending = null,
            Expression<Func<TEntity, object>> groupBy = null,
            List<Expression<Func<TEntity, object>>> Includes = null,
                                bool disableTracking = true)
        {
            IQueryable<TEntity> query = _dbset;
            if (disableTracking)
                query = query.AsNoTracking();
            if (predicate != null)
                query = query.Where(predicate);    
            if (orderBy != null)
                query = query.OrderBy(orderBy);
            else if (orderByDescending != null)
                query = query.OrderByDescending(orderByDescending);
            if (groupBy != null)
                query = query.GroupBy(groupBy).SelectMany(x => x);

            query = Includes
                .Aggregate(query, (current, include) => current.Include(include));
            return query;
        }

        public async Task ExecuteSqlRawAsync(string sql, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("SQL query must not be null or empty.", nameof(sql));

            await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public async Task<IEnumerable<TEntity>> FromSqlRawAsync(string sql, params object[] parameters)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException("SQL query must not be null or empty.", nameof(sql));

            return await _dbset
                .FromSqlRaw(sql, parameters)
                .ToListAsync();
        }

        #endregion
    }
}
