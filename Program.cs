using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

namespace mapperPizelScan
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Console.WriteLine("Provide location: ");
            //string location = Console.ReadLine();

            Image map = Image.Load("FinalMapPoc.png");

            List<string> aislesToHighlight = [];

            foreach (var item in Highlighter.ShelfAreas.Keys)
            {
                aislesToHighlight.Add(item);
            }
            foreach (var item in Highlighter.FridgeAreas.Keys)
            {
                aislesToHighlight.Add(item);
            }
            foreach (var item in Highlighter.FrozenAreas.Keys)
            {
                aislesToHighlight.Add(item);
            }

            Highlighter.HighlightArea(map, ["16A", "02B", "17A", "19B", "13A", "11B", "Dairy", "Wine 6"]);

            map.Save("output.png");
        }
    }
}