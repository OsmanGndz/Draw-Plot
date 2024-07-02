using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;

namespace OBP_Project
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        public Form1()
        {
            InitializeComponent();
            UpdateConnectionStatus(false);
        }
        private void UpdateConnectionStatus(bool isConnected)
        {
            if (isConnected)
            {

                panel1.BackColor = System.Drawing.Color.Green;
            }
            else
            {

                panel1.BackColor = System.Drawing.Color.Red;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void startButton_Click(object sender, EventArgs e)
        {
            string serverAddress = "localhost";
            int port = 8000;
            bool isConnected = ConnectionManager.Instance.Connect(serverAddress, port);

            UpdateConnectionStatus(isConnected);

            if (isConnected)
            {
                // Form2'yi oluştur
                Form2 form2 = new Form2();
                form2.Show();

                // Bu formu gizle
                this.Hide();
            }
            else
            {
                MessageBox.Show("Error connecting to server.");
            }
        }
    }
}

