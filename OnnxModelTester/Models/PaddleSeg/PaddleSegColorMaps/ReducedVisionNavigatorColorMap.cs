using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnnxModelTester.Models.PaddleSeg.PaddleSegColorMaps
{
    public class ReducedVisionNavigatorColorMap : IPaddleSegColorMap
    {
        private readonly Dictionary<byte, byte[]> _colorMap = new Dictionary<byte, byte[]>
            {
                { 0,    new byte[] { 0,         0,      0,      192 } },         // road, black     
                { 1,    new byte[] { 168,       16,     243,    192 } },         // sidewalk, purple
                { 2,    new byte[] { 250,       250,    55,     192 } },         // wall, yellow
                { 3,    new byte[] { 250,       50,     83,     192 } },         // obstacle, red
                { 4,    new byte[] { 0,         255,    0,      192 } },         // grass, light-green
                { 5,    new byte[] { 51,        221,    255,    192 } },         // sky, light-blue
                { 6,    new byte[] { 245,       147,    49,     192 } },         // person, orange
                { 7,    new byte[] { 65,        65,     232,    192 } },         // vehicle, dark-blue
                { 8,    new byte[] { 2,         100,    27,     192 } },         // vegetation, dark-green
                { 9,    new byte[] { 255,       253,    208,    192 } },         // zebra, beige
                { 10,   new byte[] { 204,       51,     102,    192 } },         // train, claret
            };
        public Dictionary<byte, byte[]> ColorMap => _colorMap;

    }
}
