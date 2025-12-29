using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace mapperPizelScan
{
    public class Highlighter
    {
        public static void HighlightArea(RectangleF areaToHighlight)
        {
            using (Image image = Image.Load("FinalMapPoc.png"))
            {
                image.Mutate(ctx => ctx.Fill(Color.FromRgba(255, 255, 0, 50), areaToHighlight));
                image.Save("HighlightedMap.png");
            }
        }

        public static Dictionary<string, RectangleF> Areas { get; } = new Dictionary<string, RectangleF>
        {
            {
                "Produce", new RectangleF(404, 222, 137, 185)
            }
        };
    }
}
