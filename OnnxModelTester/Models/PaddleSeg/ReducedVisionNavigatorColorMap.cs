using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnnxModelTester.Models.PaddleSeg
{
    public class ReducedVisionNavigatorColorMap : IPaddleSegColorMap
    {
        private readonly Dictionary<byte, byte[]> _colorMap = new Dictionary<byte, byte[]>
            {
                { 0,    new byte[] { 0,         0,      0,      255 } },         // road, black     
                { 1,    new byte[] { 168,       16,     243,    255 } },         // sidewalk, purple
                { 2,    new byte[] { 250,       250,    55,     255 } },         // wall, yellow
                { 3,    new byte[] { 250,       50,     83,     255 } },         // obstacle, red
                { 4,    new byte[] { 0,         255,    0,      255 } },         // grass, light-green
                { 5,    new byte[] { 51,        221,    255,    255 } },         // sky, light-blue
                { 6,    new byte[] { 245,       147,    49,     255 } },         // person, orange
                { 7,    new byte[] { 65,        65,     232,    255 } },         // vehicle, dark-blue
                { 8,    new byte[] { 2,         100,    27,     255 } },         // vegetation, dark-green
                //{ 9,    new byte[] { 37,        219,    188,    255 } },         // bike_path, cyan?
                { 9,    new byte[] { 255,       253,    208,    255 } },         // zebra, beige
                //{ 11,   new byte[] { 143,       143,    144,    255 } },         // animal, grey
                { 10,   new byte[] { 204,       51,     102,    255 } },         // train, claret
            };
        public Dictionary<byte, byte[]> ColorMap => _colorMap;

    }
}
