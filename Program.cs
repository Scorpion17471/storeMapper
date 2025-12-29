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

            var rect = Highlighter.Areas.GetValueOrDefault("Produce");

            Highlighter.HighlightArea(rect);
        }
    }
}