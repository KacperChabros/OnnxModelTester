using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnnxModelTester.Models.RTFormer
{
    public static class RTFormerClassColorMap
    {
        private static readonly Dictionary<byte, byte[]> _colorMap = new Dictionary<byte, byte[]>
            {
                { 0, new byte[] { 0, 0, 0, 128 } },         
                { 1, new byte[] { 128, 128, 128, 128 } }, 
                { 2, new byte[] { 255, 255, 255, 128 } }, 
                { 3, new byte[] { 64, 64, 64, 128 } },     
                { 4, new byte[] { 192, 192, 192, 128 } },  
                { 5, new byte[] { 12, 54, 251, 128 } }  
            };
        public static Dictionary<byte, byte[]> ColorMap => _colorMap;

    }
}
