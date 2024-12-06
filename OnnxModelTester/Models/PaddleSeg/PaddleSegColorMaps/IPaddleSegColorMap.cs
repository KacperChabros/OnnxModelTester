using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnnxModelTester.Models.PaddleSeg.PaddleSegColorMaps
{
    public interface IPaddleSegColorMap
    {
        Dictionary<byte, byte[]> ColorMap { get; }
    }
}
