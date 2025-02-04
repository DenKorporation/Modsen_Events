using Microsoft.EntityFrameworkCore;

namespace Application.Common.Models;

public class PagedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }

    public bool HasNextPage => Page * PageSize < TotalCount;
    public bool HasPreviousPage => Page > 1;
    
    private PagedList(IReadOnlyCollection<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> query, int page, int pageSize, CancellationToken cancellationToken = default)
    { 
        if (page < 0)
        {
            throw new ArgumentException("Cannot be negative.", nameof(page));
        }
        if (pageSize < 0)
        {
            throw new ArgumentException("Cannot be negative.", nameof(pageSize));
        }
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedList<T>(items, page, pageSize, totalCount);
    }
}