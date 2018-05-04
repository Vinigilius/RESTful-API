using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulAPI.Helpers
{
    public class AuthorsResourceParamenters
    {
        const int MaxPageSize = 20;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 3;

        public int PageSize {
            get {
                return _pageSize;
            }
            set {
                _pageSize = (value > MaxPageSize ? MaxPageSize : value);
                _pageSize = (value < 0 ? 1 : value);
            }
        }
    }
}
