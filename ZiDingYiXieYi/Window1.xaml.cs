using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZiDingYiXieYi
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        //声明socket对象
        Socket socket = null;

        public Window1()
        {
            InitializeComponent();

            //实例化对象,地址族，套接字类型，协议
            socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        }

        //连接客户端按钮事件
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //测试连接是否有异常
            try {
                //连接服务器,服务器加端口号
                socket.Connect("192.168.57.8", 6666);
                //在异步线程中处理接收事件
                Task.Run(() =>
                {
                    while (true)
                    {   //创建接收回传的数组
                        byte[] respBytes = new byte[1024];
                        //将接收到的内容放入数组中
                        socket.Receive(respBytes);
                        //解码
                        string msg = Encoding.ASCII.GetString(respBytes);
                        //文本框显示 
                        //包裹于UI主线程
                        this.Dispatcher.Invoke(() =>
                        {
                            this.messageTXT.Text = msg;
                        });
                        
                    }
                });
                MessageBox.Show("服务器连接成功");
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message,"错误提示");
            }
            
        }


        //主动接收并回传的demo
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //定义发送数据的数组
            string txt = "发送测试？";
            //发送数据
            byte[]data=Encoding.UTF8.GetBytes(txt);
            //发送数据 
            socket.Send(data);
            
            MessageBox.Show("数据发送成功");
            //初始化接收数组
            byte[] respBytes = new byte[500];
            
           //接收发送的数组(此处默认客户端会自动回传)
           socket.Receive(respBytes);

            string msg=Encoding.UTF8.GetString(respBytes);
            //this.messageTXT.Text = msg;

        }
    }
}
