using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZiDingYiXieYi
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        //声明serialport对象
       SerialPort serialPort = null;

        



        public MainWindow()
        {
            InitializeComponent();

            //实例化

            serialPort = new SerialPort("COM1",9600,Parity.None,8,StopBits.One);

            //设置相关的串口名称，波特率，校验位，数据位，停止位
            //读写缓存区大小，读写超时,判断连接状态 


            //当接收到数据的时候触发事件
            serialPort.DataReceived += SerialPort_DataReceived;
             
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //seriport.BytesToRead是串口能读到的最大字节数
            byte[] readBytes = new byte[serialPort.BytesToRead];

            //将内容整体读入字节数组中
            serialPort.Read(readBytes, 0, readBytes.Length);

            //将原数组中的十进制ASCII码解码

            string mes = Encoding.ASCII.GetString(readBytes);

            //将UI代码放在主线程执行
            this.Dispatcher.Invoke(() =>
            {
                //在文本框中显示接收到的内容
                this.mestxt.Text = mes;

            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //若有异常，则报错
            try
            { //打开动作
                serialPort.Open();
                MessageBox.Show("串口已打开");
            }
            catch (Exception ex) {
            
            MessageBox.Show(ex.Message,"异常");
            }
            
            


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //操作同一个串口发送

            //串口发送字符串
            serialPort.Write("A");

            string txt = "Message";
            //将字节数组重新编码
            byte[] bytes= Encoding.ASCII.GetBytes(txt);
            //发送数据
            serialPort.Write(bytes, 0, bytes.Length);




                //停顿等待消息发送完成
                Thread.Sleep(3000);

                //seriport.BytesToRead是串口能读到的最大字节数
                byte[] readBytes = new byte[serialPort.BytesToRead];

                //将内容整体读入字节数组中
                serialPort.Read(readBytes, 0, readBytes.Length);

                //将原数组中的十进制ASCII码解码

                string mes = Encoding.ASCII.GetString(readBytes);

                //在文本框中显示接收到的内容
                this.mestxt.Text = mes; 


        }

        private void mestxt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
