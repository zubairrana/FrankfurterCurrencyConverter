using CurrencyConverter.BusinessLogic.DTOs.Common;

namespace CurrencyConverter.BusinessLogic.Extensions
{
    public static class PaginationExtensions
    {
        public static PagedResponse<T> ToPagedResponse<T>(
            this IEnumerable<T> source,
            int page,
            int pageSize)
        {
            // Validate
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize > 100 ? 100 : pageSize;

            var list = source.ToList();
            var totalCount = list.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var items = list
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResponse<T>(
                Items: items,
                Page: page,
                PageSize: pageSize,
                TotalCount: totalCount,
                TotalPages: totalPages,
                HasNextPage: page < totalPages,
                HasPreviousPage: page > 1
            );
        }
    }
}
