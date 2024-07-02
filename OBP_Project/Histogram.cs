using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBP_Project
{
    public class Histogram
    {
        public string Color { get; set; }
        public string XData { get; set; }
        public string Title { get; set; }
        public string BinSize { get; set; }
        public bool HistDensity { get; set; }
        public bool HistCum {  get; set; }
        public string Xlabel { get; set; }
        public string Ylabel { get; set; }

        public Histogram(string color, string xData, string title, string binSize, bool histDensity, bool histCum,
            string xlabel, string ylabel)
        {
            Color = color;
            XData = xData;
            Title = title;
            BinSize = binSize;
            HistDensity = histDensity;
            HistCum = histCum;
            Xlabel = xlabel;
            Ylabel = ylabel;

        }
    }
}
