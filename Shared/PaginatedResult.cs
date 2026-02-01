using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class PaginatedResult<TEntity>
    {
        private readonly int pageIndex;

        public PaginatedResult(int PageSize, int PageIndex, int TotalSize, IEnumerable<TEntity> Data)
        {
            this.PageSize = PageSize;
            this.PageIndex = PageIndex;
            this.TotalSize = TotalSize;
            this.Data = Data;
        }

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int TotalSize { get; set; }
        public IEnumerable<TEntity> Data { get; set; }
    }
}
