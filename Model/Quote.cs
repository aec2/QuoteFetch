using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteFetch.Model
{
    class Quote
    {
        public int Id { get; set; }
        public string QuoteText { get; set; }
        public string Author { get; set; }
        public string BookTitle { get; set; }
        public PostTypeEnum PostType { get; set; } 

    }

    enum PostTypeEnum
    {
        quote,
        wallText
    }
}
