using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yelp
{
    public class Kategorija
    {
        public string Alias { get; set; }
        public string Title { get; set; }
        public Kategorija(string alias, string title)
        {
            Alias = alias;
            Title = title;
        }
    }
}
