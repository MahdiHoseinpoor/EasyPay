using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EasyPay.Common
{
    public interface ISpecification<TEntity>
    {
        Expression<Func<TEntity, bool>> Predicate { get; }
        List<Expression<Func<TEntity, object>>> Includes { get; }
        Expression<Func<TEntity, object>> OrderBy { get; }
        Expression<Func<TEntity, object>> OrderByDescending { get; }
        Expression<Func<TEntity, object>> GroupBy { get; }

    }
}
