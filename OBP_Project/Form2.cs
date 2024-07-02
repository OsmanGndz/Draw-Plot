using System;
using System.Data;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;

namespace OBP_Project
{
    public partial class Form2 : Form
    {
        private bool _changesMade = false;
        private DataTable _dataTable;
        private NetworkStream stream;

        public Form2()
        {
            InitializeComponent();
            stream = ConnectionManager.Instance.Stream;
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
            this.FormClosing += Form2_FormClosing; // Form1_FormClosing olarak belirtilmişti, Form2_FormClosing olmalı
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv";
            openFileDialog.Title = "Select the csv file";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                _dataTable = LoadCsv(filePath);
                dataGridView1.DataSource = _dataTable;
                ConnectionManager.Instance.DataTable = _dataTable; // Veriyi ConnectionManager'a aktar
            }
        }

        private DataTable LoadCsv(string filePath)
        {
            DataTable dataTable = new DataTable();
            using (StreamReader reader = new StreamReader(filePath))
            {
                string[] headers = reader.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dataTable.Columns.Add(header);
                }

                while (!reader.EndOfStream)
                {
                    string[] rows = reader.ReadLine().Split(',');
                    dataTable.Rows.Add(rows);
                }
            }
            return dataTable;
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var changedCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            _changesMade = true;
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var editedCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            _changesMade = true;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e) // Method adı düzeltildi
        {
            if (_changesMade)
            {
                DialogResult result = MessageBox.Show("Değişiklikleri onaylıyor musunuz?", "Onay", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Yes)
                {
                    buttonSaveFile_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void buttonSaveFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.Title = "Save CSV File";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                SaveCsv(filePath);
            }
        }

        private void SaveCsv(string filePath)
        {
            DataTable dataTable = (DataTable)dataGridView1.DataSource;

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    writer.Write(dataTable.Columns[i]);
                    if (i < dataTable.Columns.Count - 1)
                    {
                        writer.Write(",");
                    }
                }
                writer.WriteLine();

                foreach (DataRow row in dataTable.Rows)
                {
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        writer.Write(row[i].ToString());
                        if (i < dataTable.Columns.Count - 1)
                        {
                            writer.Write(",");
                        }
                    }
                    writer.WriteLine();
                }
            }
        }

        private void buttonDrawPlots_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(_dataTable); // DataTable parametresi kaldırıldı
            form3.Show();
            this.Hide();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
