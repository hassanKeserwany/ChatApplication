using Microsoft.EntityFrameworkCore;

namespace API.Helper
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);// number of pages

            PageSize = pageSize; // Number of items per page
            TotalCount = count; // Total count of items across all pages
            AddRange(items);// Add items to the internal list

            /*
            
            When AddRange(items) is called within the constructor of PagedList<T>,
            it iterates over each element in items
            and adds them to the internal list of PagedList<T>
            These elements represent the current page's data within the paged list structure,
            allowing subsequent operations and access to the paginated data set..

            */
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }


        //   IQueryable<T> is the result of a database query .

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source,int pageNumber,
            int pageSize)
        {
            var count = await source.CountAsync();
            var items=await source.Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
