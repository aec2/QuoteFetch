using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuoteFetch.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace QuoteFetch
{
    class Program
    {

        static void Main(string[] args)
        {
           
            string UserName = "";
            if (args.Length == 0)
            {
                UserName = "Muhibbane";
            }
            else
            {
            UserName = args[0];
            }


            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var quoteResponseList = GetLinks(UserName);

            WriteToCsv(quoteResponseList);
            Console.WriteLine($"İşlem tamamlandı...");

        }

        private static List<Quote> GetLinks(string UserName)
        {
            Console.WriteLine($"Şu an {UserName} isimli kullanıcın alıntıları csv uzantılı olarak indiriliyor....");
            List<Quote> quoteList = null;
            int counter = 1;
            while (true)
            {
                Console.WriteLine($"#####Sayfa numarası => {counter} ");
                var tupleResult = ReadUrLGetQuotes($"http://api.1000kitap.com/okurCekV2?id={UserName}&bolum=&s=&reklam_beta=0&sayfa={counter}&kume=1230105787&z=21&us=27&fr=1");
                if (quoteList == null)
                {
                    quoteList = tupleResult.Item1;
                }
                else
                {
                    quoteList.AddRange(tupleResult.Item1);
                }
                if (counter == tupleResult.Item2) break;
                counter++;
            }
            //return list
            return quoteList;
        }

        private static Tuple<List<Quote>, int>  ReadUrLGetQuotes(string URL)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.UserAgent = "TestTest";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            String res;
            List<Quote> quoteList = new List<Quote>();
            var totalPages = 0;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                res = reader.ReadToEnd();
                dynamic json = JsonConvert.DeserializeObject(res);
                var test = json["gonderiler"];
                totalPages = Convert.ToInt32(json["toplamSayfa"]);
                foreach (var gonderi in test)
                {
                    if (gonderi["turu"] == "duvar")
                    {
                        //Console.WriteLine(gonderi["id"] + " : " +gonderi["alt"]["duvar"]["durum"]);
                        quoteList.Add(new Quote
                        {
                            Id = gonderi["id"],
                            QuoteText = gonderi["alt"]["duvar"]["durum"],
                            PostType = PostTypeEnum.wallText
                        });
                    }
                    else if(gonderi["turu"] == "sozler")
                    {
                        //Console.WriteLine(gonderi["id"] + " : " +gonderi["alt"]["sozler"]["soz"]);
                        quoteList.Add(new Quote
                        {
                            Id = gonderi["id"],
                            QuoteText = gonderi["alt"]["sozler"]["soz"],
                            Author = gonderi["alt"]["yazarlar"]["adi"],
                            BookTitle = gonderi["alt"]["kitaplar"]["adi"],
                            PostType = PostTypeEnum.quote
                        });
                    }
                    else
                    {
                        
                    }

                }
            }

            return Tuple.Create(quoteList, totalPages);
        }

        private static void WriteToCsv(List<Quote> quotes)
        {
            //before your loop
            var csv = new StringBuilder();

            //in your loop
            foreach (var quote in quotes)
            {
                var qouteText = quote.QuoteText.Replace('\n', ' ');
                var bookTitle = quote.BookTitle;
                var author = quote.Author;
                var id = quote.Id;
                //Suggestion made by KyleMit
                var newLine = $"{id};{author};{bookTitle};{qouteText}";
                csv.AppendLine(newLine); 
            }
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //after your loop
            File.WriteAllText(@$"{path}\BinKitapAlintilar_{DateTime.Now.Year}.csv", csv.ToString(), Encoding.GetEncoding(1254));

        }
    }

} 