using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Common.Extensions;

public static class EventFilterExtensions
{
    public static IQueryable<Event> FilterByNameIfSet(this IQueryable<Event> query, string? name)
    {
        if (name is not null)
        {
            query = query.Where(e => e.Name.ToLower().Contains(name.ToLower()));
        }

        return query;
    }

    public static IQueryable<Event> FilterByAddressIfSet(this IQueryable<Event> query, string? address)
    {
        if (address is not null)
        {
            query = query.Where(e => e.Address.ToLower().Contains(address.ToLower()));
        }

        return query;
    }

    public static IQueryable<Event> FilterByCategoryIfSet(this IQueryable<Event> query, string? category)
    {
        if (category is not null)
        {
            query = query.Where(e => e.Category.ToLower().Contains(category.ToLower()));
        }

        return query;
    }

    public static IQueryable<Event> FilterByDateIfSet(this IQueryable<Event> query, DateTime? startDate, DateTime? endDate)
    {
        if (startDate is not null)
        {
            query = query.Where(e => e.Date >= startDate);
        }

        if (endDate is not null)
        {
            query = query.Where(e => e.Date <= endDate);
        }

        return query;
    }
}