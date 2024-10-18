namespace OnnxModelTester.Models.PaddleSeg
{
    public static class PaddleSegClassColorMap
    {
        private static readonly Dictionary<byte, byte[]> _colorMap = new Dictionary<byte, byte[]>
            {
                { 0,    new byte[] { 40,       40,     40,     128 } },         // road, black     
                { 1,    new byte[] { 168,      16,     243,    128 } },         // sidewalk, purple
                { 2,    new byte[] { 250,      250,    55,     128 } },         // wall, yellow
                { 3,    new byte[] { 250,      50,     83,     128 } },         // obstacle, red
                { 4,    new byte[] { 0,        255,    0,      128 } },         // grass, light-green
                { 5,    new byte[] { 51,       221,    255,    128 } },         // sky, light-blue
                { 6,    new byte[] { 245,      147,    49,     128 } },         // person, orange
                { 7,    new byte[] { 65,       65,     232,    128 } },         // vehicle, dark-blue
                { 8,    new byte[] { 2,        100,    27,     128 } },         // vegetation, dark-green
                //{ 9,    new byte[] { 37,       219,    188,    128 } },         // bike_path, cyan?
                //{ 10,   new byte[] { 255,      253,    208,    128 } },         // zebra, beige
                //{ 11,   new byte[] { 143,      143,    144,    128 } },         // animal, grey
                { 255, new byte[] { 0,0,0,255} }
            };
        public static Dictionary<byte, byte[]> ColorMap => _colorMap;

    }
}
