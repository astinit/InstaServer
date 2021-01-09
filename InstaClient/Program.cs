using Grpc.Net.Client;
using InstaServer;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace InstaClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://instaserver.astinco.net");
            //using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Insta.InstaClient(channel);
            var reply1 = await client.GetPageDataAsync(new PageLink { Link = "https://www.instagram.com/accounts/login/?next=/p/CI0VJzwpgwM/" });
            Console.WriteLine(JsonConvert.SerializeObject(reply1, Formatting.Indented));
            var reply2 = await client.GetPageDataAsync(new PageLink { Link = "https://www.instagram.com/p/CI0VJzwpgwM/" });
            Console.WriteLine(JsonConvert.SerializeObject(reply2, Formatting.Indented));
            //var reply4 = await client.GetPageDataAsync(new PageLink { Link = "https://www.instagram.com/p/CIlNrEfhzXU/" });
            //Console.WriteLine(JsonConvert.SerializeObject(reply4, Formatting.Indented));
            //var reply5 = await client.GetPageDataAsync(new PageLink { Link = "https://www.instagram.com/p/CI0VJzwpgwM/" });
            //Console.WriteLine(JsonConvert.SerializeObject(reply5, Formatting.Indented));
            Console.ReadKey();
        }
    }
}
