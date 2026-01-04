using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace mapperPizelScan
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Connect using a named pipe to scraper
            using var pipeClient = new NamedPipeClientStream(
                ".",
                "mapperPipe",
                PipeDirection.InOut,
                PipeOptions.Asynchronous
            );

            Console.WriteLine("Connecting to scraper...");
            pipeClient.Connect();
            Console.WriteLine("Scraper connected!");

            using var reader = new StreamReader(pipeClient);
            using var writer = new StreamWriter(pipeClient) { AutoFlush = true };

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

            itemListBuilder.Length--; // Remove last comma
            string itemList = itemListBuilder.ToString();

            // Send item list to scraper and wait for aisle list
            Console.WriteLine($"Sending message: {itemList}");
            writer.Write(itemList);
            writer.Flush();
            Console.WriteLine("Message sent, waiting for aisle list...");
            
            string? response = reader.ReadLine();

            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("No aisles to highlight. Exiting.");
                return;
            }

            Console.WriteLine($"Received aisle list: {response}");

            // Highlight aisles on map
            string[] aislesToHighlight = response.Split(',');

            Image map = Image.Load("FinalMapPoc.png");
            Highlighter.HighlightArea(map, aislesToHighlight);

            map.Save("output.png");
        }
    }
}