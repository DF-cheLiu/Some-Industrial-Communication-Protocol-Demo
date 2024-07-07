using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using modBusDLL;

namespace easyProjectPractice
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.comboBox1.DataSource=SerialPort.GetPortNames();
        }

        private ModBusRTU modBusRTU = new ModBusRTU();



        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            modBusRTU.Connect(this.comboBox1.Text);

            MessageBox.Show("连接成功");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            modBusRTU.Disconnect();
            MessageBox.Show("断开连接");
        }

        private void button3_Click(object sender, EventArgs e)
        {

            byte[] data = modBusRTU.ReadKeepRegisters(1, 2, 1);

       

            textBox1.Text = (data[0] * 256 + data[1]).ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
