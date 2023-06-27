using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yelp
{
    public class LokalObserver : IObserver<Lokal>
    {
        public double AvgRating { get; set; }
        private double SumRating { get; set; }
        private double NumberOfLocals { get; set; }
        public LokalObserver() 
        {
            AvgRating = 0;
            SumRating = 0;
            NumberOfLocals = 0;
        }

        public void OnCompleted()
        {
            AvgRating = SumRating / NumberOfLocals;
            Console.WriteLine($"Prosecna ocena za sve vracene lokale je {AvgRating}");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine(error.ToString());
        }

        public void OnNext(Lokal value)
        {
            Console.WriteLine(value.Name + " " + value.Rating + " " + Thread.CurrentThread.ManagedThreadId);

            SumRating += value.Rating;
            NumberOfLocals++;
        }
    }
}
