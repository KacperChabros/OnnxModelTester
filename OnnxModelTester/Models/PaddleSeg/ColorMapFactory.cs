using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnnxModelTester.Models.PaddleSeg
{
    public static class ColorMapFactory
    {
        public static IPaddleSegColorMap CreateColorMap(string userInput)
        {
            return userInput.ToLower() switch
            {
                "full" => new FullVisionNavigatorColorMap(),
                "reduced" => new ReducedVisionNavigatorColorMap(),
                _ => throw new ArgumentException("Invalid input, no matching color map found.")
            };
        }
    }
}
