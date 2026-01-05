using SixLabors.ImageSharp;
using System.Net.Sockets;
using System.Text;

namespace mapperPizelScan
{
    public class Program
    {
        public static void Main()
        {
            // Connect using a socket to scraper
            Console.WriteLine("Connecting to scraper...");
            using TcpClient client = new("host.docker.internal", 5858);
            Console.WriteLine("Scraper connected!");

            using NetworkStream stream = client.GetStream();

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
            stream.Write(Encoding.UTF8.GetBytes(itemList + "\r\n"));
            Console.WriteLine("Message sent, waiting for aisle list...");
            
            var buffer = new byte[1024];
            var data = stream.Read(buffer);
            string? response = Encoding.UTF8.GetString(buffer, 0, data);
            
            while(response.EndsWith("\r\n") == false)
            {
                data = stream.Read(buffer);
                response += Encoding.UTF8.GetString(buffer, 0, data);
            }

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