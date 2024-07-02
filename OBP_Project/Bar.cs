using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBP_Project
{
    public class Bar
    {
        public string Color { get; set; }
        public string XData { get; set; }
        public string YData { get; set; }
        public string Title { get; set; }
        public string BarWidth { get; set; }
        public string BarHatch { get; set; }
        public string Xlabel { get; set; }
        public string Ylabel { get; set; }

        public Bar(string color, string xData, string yData, string title, string barWidth, string barHatch,
            string xlabel, string ylabel)
        {
            Color = color;
            XData = xData;
            YData = yData;
            Title = title;
            BarWidth = barWidth;
            BarHatch = barHatch;
            Xlabel = xlabel;
            Ylabel = ylabel;

        }
    }
}
