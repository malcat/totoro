using Totoro.Models;

namespace Totoro.Extensions;

public static class LinqExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, IPageable page)
    {
        return queryable.Paginate(page.PageNumber, page.PageSize);
    }

    public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, int page, int size = 50)
    {
        var skip = page > 1 ? page * size - size : 0;

        return skip == 0
            ? queryable.Take(size)
            : queryable.Skip(skip).Take(size);
    }
}
