using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Net.Http.Headers;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Runtime.InteropServices.JavaScript;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace Yelp
{

    public class HttpServerObservable : IObservable<Lokal>
    {
        const string API_KEY = "LnX1-p20RwoEELsFS1VV6OAdvZfg9WnWiQClefEUafsjhBJMKNAebw7BAjXOIFxVqRCuTdfJg9c57FlWqmt19aMH0Pk2d-Q8i8JQ0bTklNEc1qOlPnrWGVNjcVGbZHYx";
        const string BASE_URL = "https://api.yelp.com/v3";
        public Subject<Lokal> lokali;
        public HttpClient Client { get; set; }

        public HttpServerObservable()
        {
            Client = new HttpClient();
            lokali = new Subject<Lokal>();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_KEY);
        }

        public void Pretraga(string lokacija, double rating, string kategorija)
        {
            Client.GetAsync($"{BASE_URL}/businesses/search?location={lokacija}&categories={kategorija}&open_now=true").ContinueWith(async task =>
            {
                try
                {
                    var url = $"{BASE_URL}/businesses/search?location={lokacija}&open_now=true&categories={kategorija}";//bez &sort_by=price jer ne moze po price
                    var response = task.Result;
                    response.EnsureSuccessStatusCode();
                    var neparsovan = await response.Content.ReadAsStringAsync();
                    var parsovan = JObject.Parse(neparsovan);
                    var content = JsonConvert.DeserializeObject<List<Lokal>>(parsovan["businesses"].ToString());
                    if (content.Count == 0)
                        throw new Exception("Nema takvih lokala!");
                    var filteredContent = content.Where(lokal => lokal.Rating >= rating).ToList();
                    filteredContent.Sort();
                    foreach(var lokal in filteredContent)
                    {
                        lokali.OnNext(lokal);
                    }
                    lokali.OnCompleted();
                }
                catch (Exception ex)
                {
                    lokali.OnError(ex);
                }
            });
        }

        public IDisposable Subscribe(IObserver<Lokal> obs)
        {
            return lokali
                .SubscribeOn(ThreadPoolScheduler.Instance)
                .ObserveOn(ThreadPoolScheduler.Instance)
                .Subscribe(obs);
        }
    }
}
