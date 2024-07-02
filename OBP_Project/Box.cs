using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBP_Project
{
    public class Box
    {
        public string Color { get; set; }
        public string XData { get; set; }
        public string Title { get; set; }
        public string SymbolStyle { get; set; }
        public bool Vertical { get; set; }
        public string BoxWidth { get; set; }
        public string Xlabel { get; set; }
        public string Ylabel { get; set; }

        public Box(string color, string xData, string title, string symbolStyle, bool vertical, string boxWidth,
            string xlabel, string ylabel)
        {
            Color = color;
            XData = xData;
            Title = title;
            SymbolStyle = symbolStyle;
            Vertical = vertical;
            BoxWidth = boxWidth;
            Xlabel = xlabel;
            Ylabel = ylabel;

        }
    }
}
