using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yelp
{
    public class Lokal
    {
        public string Location { get; set; }    
        public string Name { get; set; }
        public bool Open {  get; set; }
        public double Rating { get; set; }
        public double Price { get; set; }

        public string Categories { get; set; }

        public Lokal (bool open, double rating, double price, string kategorija, string name, string lokacija)
        {
            Name = name;
            Location = lokacija;
            Open = open;
            Rating = rating;
            Price = price;
            Categories = kategorija;
        }


    }
}
