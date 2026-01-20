using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Identity.Client;
using SixLabors.ImageSharp;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace mapperPizelScan
{
    public class Program
    {
        public static async Task Main()
        {
            // Instance servicebus client + sender/receiver
            var client = new ServiceBusClient(
                "mapperEvents.servicebus.windows.net",
                new DefaultAzureCredential(),
                new ServiceBusClientOptions
                {
                    TransportType = ServiceBusTransportType.AmqpWebSockets
                }
            );

            var sender = client.CreateSender("items");
            var receiver = client.CreateReceiver("aisles");

            // Get item list from user
            StringBuilder itemListBuilder = new();

            Console.WriteLine("Enter an item to search for or type DONE to finish list: ");
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input))
            {
                input = "DONE";
            }
            while (input != "DONE")
            {
                itemListBuilder.Append(input + ",");
                Console.WriteLine("Enter an item to search for or type DONE to finish list: ");
                input = Console.ReadLine();
            }

            if (itemListBuilder.Length == 0)
            {
                Console.WriteLine("No items entered. Exiting.");
                return;
            }
            itemListBuilder.Length--; // Remove last comma
            string itemList = itemListBuilder.ToString();

            // Send item list to scraper and wait for aisle list
            Console.WriteLine($"Sending message: {itemList}");
            await sender.SendMessageAsync(new ServiceBusMessage(itemList));
            Console.WriteLine("Message sent, waiting for aisle list...");
            
            var buffer = new byte[1024];
            var data = await receiver.ReceiveMessageAsync();
            string response = data.ToString();

            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("No aisles to highlight. Exiting.");
                return;
            }

            Console.WriteLine($"Received aisle list: {response}");

            // Highlight aisles on map
            string[] aislesToHighlight = response.Trim().Split(',');

            Console.WriteLine("Starting highlighting");
            Image map = Image.Load("./imgData/FinalMapPoc.png");
            Highlighter.HighlightArea(map, aislesToHighlight);
            Console.WriteLine("Highlighting done, saving image to imgData/output.png");
            map.Save("./imgData/output.png");
            Console.WriteLine("Image saved to imgData/output.png");
        }
    }
}