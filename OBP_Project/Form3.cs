using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace OBP_Project
{
    public partial class Form3 : Form
    {
        private NetworkStream stream;
        private DataTable dataTable;
        private string selectedColorLineString;

        public Form3(DataTable dataTable)
        {
            InitializeComponent();
            this.dataTable = dataTable;
            stream = ConnectionManager.Instance.Stream;
            LoadColumnNames();
        }

        private void LoadColumnNames()
        {
            dataGridView1.Columns.Clear();
            if (dataTable != null)
            {
                foreach (DataColumn column in dataTable.Columns)
                {
                    dataGridView1.Columns.Add(column.ColumnName, column.ColumnName);
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    dataGridView1.Rows.Add(row.ItemArray);
                }
            }
            else
            {
                MessageBox.Show("No data loaded from CSV file.");
            }
        }

        private void lineSendButton_Click(object sender, EventArgs e)
        {
            if (dataTable != null && stream != null)
            {
                // Data X ve Data Y sütunlarını al
                string dataXColumnName = txtDataX.Text;
                string dataYColumnName = txtDataY.Text;
                string title = LineTitle.Text;
                string LineWidth = lineWidth.Text;
                string LineStyle = lineStyle.Text;
                

                // Sütunların indexlerini bul
                int dataXColumnIndex = dataTable.Columns.IndexOf(dataXColumnName);
                int dataYColumnIndex = dataTable.Columns.IndexOf(dataYColumnName);

                if (dataXColumnIndex == -1 || dataYColumnIndex == -1)
                {
                    MessageBox.Show("Data X or Data Y column not found.");
                    return;
                }

                List<Line> lines = new List<Line>();
                foreach (DataRow row in dataTable.Rows)
                {
                    // Data X ve Data Y değerlerini almak için satırı belirli sütunlardan çıkart
                    string dataXString = row[dataXColumnIndex].ToString();
                    string dataYString = row[dataYColumnIndex].ToString();

                    // Veriyi dönüştürmeye çalışmadan önce boşlukları kaldır ve gereksiz karakterleri temizle
                    dataXString = dataXString.Trim();
                    dataYString = dataYString.Trim();

                    // Eğer veri boş veya null ise, işleme devam etme
                    if (string.IsNullOrWhiteSpace(dataXString) || string.IsNullOrWhiteSpace(dataYString))
                    {
                        MessageBox.Show("Invalid data format.");
                        return;
                    }

                    // Line nesnesini oluştur ve listeye ekle
                    Line line = new Line(selectedColorLineString, dataXString, dataYString, title, LineWidth,
                        LineStyle, dataXColumnName, dataYColumnName);
                    lines.Add(line);
                }
                // Line listesini gönder
                SendLinesToServer(lines);

                MessageBox.Show("Data sent to server successfully.");
            }
            else
            {
                MessageBox.Show("No data to send or not connected to server.");
            }
        }

        private void SendLinesToServer(List<Line> lines)
        {
            StringBuilder dataToSend = new StringBuilder();
            bool firstLine = true;

            foreach (var line in lines)
            {
                string rowData;

                if (firstLine)
                {
                    rowData = $"Line,{line.Color},{line.XData},{line.YData},{line.Title},{line.LineWidth},{line.LineStyle},{line.Xlabel},{line.Ylabel}\n";
                    firstLine = false; // İlk satırdan sonra bu bayrağı kapat
                }
                else
                {
                    rowData = $"{line.XData},{line.YData}\n";
                }

                dataToSend.Append(rowData);
            }

            byte[] data = Encoding.ASCII.GetBytes(dataToSend.ToString());
            stream.Write(data, 0, data.Length);
        }


        private void btnLineSelectColor_Click(object sender, EventArgs e)
        {
            // Renk seçim iletişim kutusunu oluştur
            ColorDialog colorDialog = new ColorDialog();

            // Kullanıcı bir renk seçerse
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // Seçilen rengi al ve arka plan rengini ayarla
                Color selectedColor = colorDialog.Color;
                selectedColorLineString = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}"; // Seçilen rengi dizeye dönüştür
                panelColor.BackColor = selectedColor;
            }
        }
        private void scatterSendButton_Click(object sender, EventArgs e)
        {
            if (dataTable != null && stream != null)
            {
                // Data X ve Data Y sütunlarını al
                string dataXColumnName = scattertxtDataX.Text;
                string dataYColumnName = scattertxtDataY.Text;
                string title = scatterTitle.Text;
                string pointSize = scatterSize.Text;
                string scatterMarker = scattermarker.Text;


                // Sütunların indexlerini bul
                int dataXColumnIndex = dataTable.Columns.IndexOf(dataXColumnName);
                int dataYColumnIndex = dataTable.Columns.IndexOf(dataYColumnName);

                if (dataXColumnIndex == -1 || dataYColumnIndex == -1)
                {
                    MessageBox.Show("Data X or Data Y column not found.");
                    return;
                }

                List<Scatter> scatters = new List<Scatter>();
                foreach (DataRow row in dataTable.Rows)
                {
                    // Data X ve Data Y değerlerini almak için satırı belirli sütunlardan çıkart
                    string dataXString = row[dataXColumnIndex].ToString();
                    string dataYString = row[dataYColumnIndex].ToString();

                    // Veriyi dönüştürmeye çalışmadan önce boşlukları kaldır ve gereksiz karakterleri temizle
                    dataXString = dataXString.Trim();
                    dataYString = dataYString.Trim();

                    // Eğer veri boş veya null ise, işleme devam etme
                    if (string.IsNullOrWhiteSpace(dataXString) || string.IsNullOrWhiteSpace(dataYString))
                    {
                        MessageBox.Show("Invalid data format.");
                        return;
                    }

                    // Line nesnesini oluştur ve listeye ekle
                    Scatter scatter = new Scatter(selectedColorLineString, dataXString, dataYString, title, pointSize, scatterMarker,
                        dataXColumnName, dataYColumnName);
                    scatters.Add(scatter);
                }
                // Line listesini gönder
                SendScattersToServer(scatters);

                MessageBox.Show("Data sent to server successfully.");
            }
            else
            {
                MessageBox.Show("No data to send or not connected to server.");
            }
        }
        private void SendScattersToServer(List<Scatter> scatters)
        {
            StringBuilder dataToSend = new StringBuilder();
            bool firstLine = true;

            foreach (var scatter in scatters)
            {
                string rowData;

                if (firstLine)
                {
                    rowData = $"Scatter,{scatter.Color},{scatter.XData},{scatter.YData},{scatter.Title},{scatter.PointSize},{scatter.ScatterMarker},{scatter.Xlabel},{scatter.Ylabel}\n";
                    firstLine = false; // İlk satırdan sonra bu bayrağı kapat
                }
                else
                {
                    rowData = $"{scatter.XData},{scatter.YData}\n";
                }

                dataToSend.Append(rowData);
            }

            byte[] data = Encoding.ASCII.GetBytes(dataToSend.ToString());
            stream.Write(data, 0, data.Length);

        }

        private void btnScatterSelectColor_Click(object sender, EventArgs e)
        {
            // Renk seçim iletişim kutusunu oluştur
            ColorDialog colorDialog = new ColorDialog();

            // Kullanıcı bir renk seçerse
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // Seçilen rengi al ve arka plan rengini ayarla
                Color selectedColor = colorDialog.Color;
                selectedColorLineString = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}"; // Seçilen rengi dizeye dönüştür
                panelScatter.BackColor = selectedColor;
            }
        }

        private void barSendButton_Click(object sender, EventArgs e)
        {
            if (dataTable != null && stream != null)
            {
                // Data X ve Data Y sütunlarını al
                string dataXColumnName = bartxtDataX.Text;
                string dataYColumnName = bartxtDataY.Text;
                string title = barTitle.Text;
                string barWidth = barwidth.Text;
                string barHatch = barhatch.Text;


                // Sütunların indexlerini bul
                int dataXColumnIndex = dataTable.Columns.IndexOf(dataXColumnName);
                int dataYColumnIndex = dataTable.Columns.IndexOf(dataYColumnName);

                if (dataXColumnIndex == -1 || dataYColumnIndex == -1)
                {
                    MessageBox.Show("Data X or Data Y column not found.");
                    return;
                }

                List<Bar> bars = new List<Bar>();
                foreach (DataRow row in dataTable.Rows)
                {
                    // Data X ve Data Y değerlerini almak için satırı belirli sütunlardan çıkart
                    string dataXString = row[dataXColumnIndex].ToString();
                    string dataYString = row[dataYColumnIndex].ToString();

                    // Veriyi dönüştürmeye çalışmadan önce boşlukları kaldır ve gereksiz karakterleri temizle
                    dataXString = dataXString.Trim();
                    dataYString = dataYString.Trim();

                    // Eğer veri boş veya null ise, işleme devam etme
                    if (string.IsNullOrWhiteSpace(dataXString) || string.IsNullOrWhiteSpace(dataYString))
                    {
                        MessageBox.Show("Invalid data format.");
                        return;
                    }

                    // Line nesnesini oluştur ve listeye ekle
                    Bar bar = new Bar(selectedColorLineString, dataXString, dataYString, title, barWidth, barHatch,
                        dataXColumnName, dataYColumnName);
                    bars.Add(bar);
                }
                // Line listesini gönder
                SendBarsToServer(bars);

                MessageBox.Show("Data sent to server successfully.");
            }
            else
            {
                MessageBox.Show("No data to send or not connected to server.");
            }
        }
        private void SendBarsToServer(List<Bar> bars)
        {
            StringBuilder dataToSend = new StringBuilder();
            bool firstLine = true;

            foreach (var bar in bars)
            {
                string rowData;

                if (firstLine)
                {
                    rowData = $"Bar,{bar.Color},{bar.XData},{bar.YData},{bar.Title},{bar.BarWidth},{bar.BarHatch},{bar.Xlabel},{bar.Ylabel}\n";
                    firstLine = false; // İlk satırdan sonra bu bayrağı kapat
                }
                else
                {
                    rowData = $"{bar.XData},{bar.YData}\n";
                }

                dataToSend.Append(rowData);
            }

            byte[] data = Encoding.ASCII.GetBytes(dataToSend.ToString());
            stream.Write(data, 0, data.Length);
        }


        private void btnBarSelectColor_Click(object sender, EventArgs e)
        {
            // Renk seçim iletişim kutusunu oluştur
            ColorDialog colorDialog = new ColorDialog();

            // Kullanıcı bir renk seçerse
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // Seçilen rengi al ve arka plan rengini ayarla
                Color selectedColor = colorDialog.Color;
                selectedColorLineString = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}"; // Seçilen rengi dizeye dönüştür
                panelBar.BackColor = selectedColor;
            }
        }

        private void histogramSendButton_Click(object sender, EventArgs e)
        {
            if (dataTable != null && stream != null)
            {
                // Data X ve Data Y sütunlarını al
                string dataXColumnName = histtxtDataX.Text;
                string YLabel = "Frequency";
                string title = histTitle.Text;
                string histBinSize = BinSize.Text;
                bool histDensity = isDensity.Checked;
                bool histCumulative = isCumulative.Checked;


                // Sütunların indexlerini bul
                int dataXColumnIndex = dataTable.Columns.IndexOf(dataXColumnName);

                if (dataXColumnIndex == -1)
                {
                    MessageBox.Show("Data X or Data Y column not found.");
                    return;
                }

                List<Histogram> histograms = new List<Histogram>();
                foreach (DataRow row in dataTable.Rows)
                {
                    // Data X ve Data Y değerlerini almak için satırı belirli sütunlardan çıkart
                    string dataXString = row[dataXColumnIndex].ToString();

                    // Veriyi dönüştürmeye çalışmadan önce boşlukları kaldır ve gereksiz karakterleri temizle
                    dataXString = dataXString.Trim();

                    // Eğer veri boş veya null ise, işleme devam etme
                    if (string.IsNullOrWhiteSpace(dataXString))
                    {
                        MessageBox.Show("Invalid data format.");
                        return;
                    }

                    // Line nesnesini oluştur ve listeye ekle
                    Histogram histogram = new Histogram(selectedColorLineString, dataXString, title, histBinSize, histDensity,
                        histCumulative, dataXColumnName, YLabel);
                    histograms.Add(histogram);
                }
                // Line listesini gönder
                SendHistogramsToServer(histograms);

                MessageBox.Show("Data sent to server successfully.");
            }
            else
            {
                MessageBox.Show("No data to send or not connected to server.");
            }
        }
        private void SendHistogramsToServer(List<Histogram> histograms)
        {
            
            StringBuilder dataToSend = new StringBuilder();
            bool firstLine = true;

            foreach (var histogram in histograms)
            {
                string rowData;

                if (firstLine)
                {
                    rowData = $"Histogram,{histogram.Color},{histogram.XData},{histogram.Title},{histogram.BinSize},{histogram.HistDensity},{histogram.HistCum},{histogram.Xlabel},{histogram.Ylabel}\n";
                    firstLine = false; // İlk satırdan sonra bu bayrağı kapat
                }
                else
                {
                    rowData = $"{histogram.XData}\n";
                }

                dataToSend.Append(rowData);
            }

            byte[] data = Encoding.ASCII.GetBytes(dataToSend.ToString());
            stream.Write(data, 0, data.Length);
        }

        private void btnHistogramSelectColor_Click(object sender, EventArgs e)
        {
            // Renk seçim iletişim kutusunu oluştur
            ColorDialog colorDialog = new ColorDialog();

            // Kullanıcı bir renk seçerse
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // Seçilen rengi al ve arka plan rengini ayarla
                Color selectedColor = colorDialog.Color;
                selectedColorLineString = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}"; // Seçilen rengi dizeye dönüştür
                panelHist.BackColor = selectedColor;
            }
        }

        private void boxSendButton_Click(object sender, EventArgs e)
        {
            if (dataTable != null && stream != null)
            {
                // Data X ve Data Y sütunlarını al
                string dataXColumnName = boxtxtData.Text; 
                string XLabel = "Categories";
                string title = boxTitle.Text;
                string symbolStyle = Vertical.Text;
                bool vertical = Verical.Checked;
                string boxWidth = BoxWidth.Text;


                // Sütunların indexlerini bul
                int dataXColumnIndex = dataTable.Columns.IndexOf(dataXColumnName);

                if (dataXColumnIndex == -1)
                {
                    MessageBox.Show("Data X or Data Y column not found.");
                    return;
                }

                List<Box> boxes = new List<Box>();
                foreach (DataRow row in dataTable.Rows)
                {
                    // Data X ve Data Y değerlerini almak için satırı belirli sütunlardan çıkart
                    string dataXString = row[dataXColumnIndex].ToString();

                    // Veriyi dönüştürmeye çalışmadan önce boşlukları kaldır ve gereksiz karakterleri temizle
                    dataXString = dataXString.Trim();

                    // Eğer veri boş veya null ise, işleme devam etme
                    if (string.IsNullOrWhiteSpace(dataXString))
                    {
                        MessageBox.Show("Invalid data format.");
                        return;
                    }

                    // Line nesnesini oluştur ve listeye ekle
                    Box box = new Box(selectedColorLineString, dataXString, title, symbolStyle, vertical,
                        boxWidth, XLabel, dataXColumnName);
                    boxes.Add(box);
                }
                // Line listesini gönder
                SendBoxesToServer(boxes);

                MessageBox.Show("Data sent to server successfully.");
            }
            else
            {
                MessageBox.Show("No data to send or not connected to server.");
            }
        }
        private void SendBoxesToServer(List<Box> boxes)
        {

            StringBuilder dataToSend = new StringBuilder();
            bool firstLine = true;

            foreach (var box in boxes)
            {
                string rowData;

                if (firstLine)
                {
                    rowData = $"Box,{box.Color},{box.XData},{box.Title},{box.SymbolStyle},{box.Vertical},{box.BoxWidth},{box.Xlabel},{box.Ylabel}\n";
                    firstLine = false; // İlk satırdan sonra bu bayrağı kapat
                }
                else
                {
                    rowData = $"{box.XData}\n";
                }

                dataToSend.Append(rowData);
            }

            byte[] data = Encoding.ASCII.GetBytes(dataToSend.ToString());
            stream.Write(data, 0, data.Length);
        }

        private void btnBoxSelectColor_Click(object sender, EventArgs e)
        {
            // Renk seçim iletişim kutusunu oluştur
            ColorDialog colorDialog = new ColorDialog();

            // Kullanıcı bir renk seçerse
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                // Seçilen rengi al ve arka plan rengini ayarla
                Color selectedColor = colorDialog.Color;
                selectedColorLineString = $"#{selectedColor.R:X2}{selectedColor.G:X2}{selectedColor.B:X2}"; // Seçilen rengi dizeye dönüştür
                panelBox.BackColor = selectedColor;
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Form yüklenirken yapılacak işlemler
        }

    }
}
