using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBP_Project
{
    public class Scatter
    {
        public string Color { get; set; }
        public string XData { get; set; }
        public string YData { get; set; }
        public string Title { get; set; }
        public string PointSize { get; set; }
        public string ScatterMarker { get; set; }
        public string Xlabel { get; set; }
        public string Ylabel { get; set; }

        public Scatter(string color, string xData, string yData, string title, string pointSize, string scatterMarker,
            string xlabel, string ylabel)
        {
            Color = color;
            XData = xData;
            YData = yData;
            Title = title;
            PointSize = pointSize;
            ScatterMarker = scatterMarker;
            Xlabel = xlabel;
            Ylabel = ylabel;

        }
    }
}
