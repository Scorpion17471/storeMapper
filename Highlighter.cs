using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace mapperPizelScan
{
    public class Highlighter
    {
        public static void HighlightArea(Image source, List<string> aisleNames)
        {
            foreach (var aisle in aisleNames)
            {
                RectangleF area;
                if (ShelfAreas.TryGetValue(aisle, out area))
                {
                    source.Mutate(ctx => ctx.Fill(Color.FromRgba(255, 255, 0, 50), area));
                }
                else if (FridgeAreas.TryGetValue(aisle, out area))
                {
                    source.Mutate(ctx => ctx.Fill(Color.FromRgba(0, 255, 255, 50), area));
                }
                else if (FrozenAreas.TryGetValue(aisle, out area))
                {
                    source.Mutate(ctx => ctx.Fill(Color.FromRgba(0, 0, 255, 50), area));
                }
            }
        }

        public static Dictionary<string, RectangleF> ShelfAreas { get; } = new Dictionary<string, RectangleF>
        {
            {
                "Produce", new RectangleF(404, 222, 137, 185)
            },
            {
                "01A", new RectangleF(540, 220, 23, 100)
            },
            {
                "02A", new RectangleF(565, 220, 22, 100)
            },
            {
                "03A", new RectangleF(605, 250, 25, 125)
            },
            {
                "04A", new RectangleF(633, 250, 30, 125)
            },
            {
                "05A", new RectangleF(662, 270, 30, 105)
            },
            {
                "06A", new RectangleF(693, 270, 30, 105)
            },
            {
                "07A", new RectangleF(724, 240, 30, 135)
            },
            {
                "08A", new RectangleF(756, 240, 30, 135)
            },
            {
                "09A", new RectangleF(787, 260, 30, 95)
            },
            {
                "10A", new RectangleF(818, 260, 30, 95)
            },
            {
                "11A", new RectangleF(850, 240, 30, 135)
            },
            {
                "14A", new RectangleF(943, 240, 30, 135)
            },
            {
                "15A", new RectangleF(973, 240, 30, 135)
            },
            {
                "16A", new RectangleF(1003, 240, 30, 135)
            },
            {
                "17A", new RectangleF(1034, 240, 32, 135)
            },
            {
                "18A", new RectangleF(1067, 240, 30, 135)
            },
            {
                "19A", new RectangleF(1097, 240, 30, 135)
            },
            {
                "Wine 1", new RectangleF(1125, 102, 190, 35)
            },
            {
                "Wine 2", new RectangleF(1125, 137, 190, 35)
            },
            {
                "Wine 3", new RectangleF(1125, 171, 190, 35)
            },
            {
                "Wine 4", new RectangleF(1178, 350, 135, 25)
            },
            {
                "Wine 5", new RectangleF(1178, 372, 135, 25)
            },
            {
                "Wine 6", new RectangleF(1178, 396, 135, 25)
            },
            {
                "Wine 7", new RectangleF(1178, 420, 135, 25)
            },
            {
                "Wine 8", new RectangleF(1178, 443, 135, 25)
            },
            {
                "03C", new RectangleF(540, 392, 43, 22)
            },
            {
                "02B", new RectangleF(568, 413, 30, 120)
            },
            {
                "03B", new RectangleF(600, 405, 30, 58)
            },
            {
                "04B", new RectangleF(632, 405, 30, 58)
            },
            {
                "05B", new RectangleF(662, 405, 30, 110)
            },
            {
                "06B", new RectangleF(693, 405, 30, 110)
            },
            {
                "07B", new RectangleF(724, 405, 30, 110)
            },
            {
                "08B", new RectangleF(755, 405, 30, 60)
            },
            {
                "09B", new RectangleF(787, 405, 30, 60)
            },
            {
                "10B", new RectangleF(818, 405, 30, 60)
            },
            {
                "11B", new RectangleF(849, 405, 30, 110)
            },
            {
                "14B", new RectangleF(941, 405, 30, 110)
            },
            {
                "15B", new RectangleF(972, 405, 30, 110)
            },
            {
                "16B", new RectangleF(1003, 405, 30, 110)
            },
            {
                "17B", new RectangleF(1035, 405, 30, 110)
            },
            {
                "18B", new RectangleF(1066, 405, 30, 110)
            },
            {
                "19B", new RectangleF(1097, 405, 30, 110)
            },
            {
                "Floral", new RectangleF(512, 439, 28, 92)
            },
            {
                "Packaged Bread", new RectangleF(531, 561, 90, 50)
            },
            {
                "Fresh Bakery", new RectangleF(282, 104, 123, 316)
            },
            {
                "Prepared Foods", new RectangleF(68, 132, 215, 240)
            }
        };

        public static Dictionary<string, RectangleF> FridgeAreas { get; } = new Dictionary<string, RectangleF>
        {
            {
                "Dairy", new RectangleF(618, 528, 508, 45)
            },
            {
                "Cheese Shop", new RectangleF(404, 420, 105, 128)
            },
            {
                "Deli", new RectangleF(282, 419, 106, 122)
            },
            {
                "Seafood", new RectangleF(173, 371, 110, 155)
            },
            {
                "Meat Department", new RectangleF(173, 525, 325, 95)
            }
        };

        public static Dictionary<string, RectangleF> FrozenAreas { get; } = new Dictionary<string, RectangleF>
        {
            {
                "12A", new RectangleF(881, 240, 33, 135)
            },
            {
                "13A", new RectangleF(915, 240, 28, 135)
            },
            {
                "01B", new RectangleF(538, 413, 30, 120)
            },
            {
                "12B", new RectangleF(881, 405, 30, 110)
            },
            {
                "13B", new RectangleF(912, 405, 30, 110)
            }
        };
    }
}
