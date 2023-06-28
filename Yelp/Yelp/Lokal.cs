using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yelp
{
    public class Lokal : IComparable<Lokal>
    {
  
        public string Name { get; set; }
        public bool Open {  get; set; }
        public double Rating { get; set; }
        public string Price { get; set; }

        public Kategorija[] Categories { get; set; }

        public Lokal (bool open, double rating, string price, Kategorija[] kategorija, string name)
        {
            Name = name;            
            Open = open;
            Rating = rating;
            Price = price;
            Categories = kategorija;
        }

        public int CompareTo(Lokal? other)
        {
            if (other == null) return 1;
            if (other.Price == null) return -1;
            if (Price == null) return 1;
            if (Price.Length > other.Price.Length) return -1;
            else return 1;
        }
    }
}
