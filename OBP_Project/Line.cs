using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBP_Project
{
    public class Line
    {
        public string Color { get; set; }
        public string XData { get; set; }
        public string YData { get; set; }
        public string Title { get; set; }
        public string LineWidth { get; set; }
        public string LineStyle { get; set; }
        public string Xlabel { get; set; }
        public string Ylabel { get; set; }



        public Line(string color, string xData, string yData, string title, string lineWidth, string lineStyle,
            string xlabel, string ylabel)
        {
            Color = color;
            XData = xData;
            YData = yData;
            Title = title;
            LineWidth = lineWidth;
            LineStyle = lineStyle;
            Xlabel = xlabel;
            Ylabel = ylabel;
            

        }
    }
}
