using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace modBusDLL
{
    public class ModBusRTU
    {

        private SerialPort serialPort = null;

        public ModBusRTU()
        {
            serialPort = new SerialPort();
        }

        //连接方法

        //端口号，波特率，数据位，校验位，停止位
        public void Connect(string portName,int baudRate=9600,int dataBits=8,
                            Parity parity=Parity.None,StopBits stopBits=StopBits.One)
        { 
        if (serialPort.IsOpen) {
            
                serialPort.Close();
            }
        serialPort.BaudRate = baudRate;
        serialPort.PortName = portName;
        serialPort.Parity = parity;
        serialPort.StopBits = stopBits;
        serialPort.DataBits = dataBits;

            serialPort.Open();

            
        }

        //断开连接方法
        public void Disconnect()
        {
            if (serialPort.IsOpen)
            {

                serialPort.Close();
            }
        }

        //读消息方法
        public byte[] ReadKeepRegisters(byte devAdd,ushort start,ushort length)
        { 
        //拼接报文，发送报文，接受报文，校验报文，解析报文

            //创建一个字节集合
            List<byte> ret = new List<byte>();

            //协议格式：站地址+功能码+起始寄存器地址+寄存器数量+CRC

            //站地址
            ret.Add(devAdd);

            //功能码
            ret.Add(0x03);
 

            //起始寄存器地址
            //高位地址
            ret.Add((byte)(start / 256));
            //低位地址
            ret.Add((byte)(start % 256));


            //寄存器数量
            //高位地址
            ret.Add((byte)(length / 256));
            //低位地址
            ret.Add((byte)(length % 256));





            byte[] crc= CRCCalc(ret.ToArray());
            ret.AddRange(crc);





            //发送报文
            serialPort.Write(ret.ToArray(), 0, ret.Count);

            Thread.Sleep(50);
            
            //接受长度
            int byteCount = serialPort.BytesToRead;

            byte[] data = new byte[byteCount];

            //读入data
            serialPort.Read(data, 0, data.Length);

            byte[] result = new byte[length*2]; 
        
            Array.Copy(data,3,result, 0,length*2);

            return result;
        
        }

        #region 16位CRC校验
        /// <summary>
        /// CRC校验，参数data为byte数组
        /// </summary>
        /// <param name="data">校验数据，字节数组</param>
        /// <returns>字节0是高8位，字节1是低8位</returns>
        public static byte[] CRCCalc(byte[] data)
        {
            //crc计算赋初始值
            int crc = 0xffff;
            for (int i = 0; i < data.Length; i++)
            {
                crc = crc ^ data[i];
                for (int j = 0; j < 8; j++)
                {
                    int temp;
                    temp = crc & 1;
                    crc = crc >> 1;
                    crc = crc & 0x7fff;
                    if (temp == 1)
                    {
                        crc = crc ^ 0xa001;
                    }
                    crc = crc & 0xffff;
                }
            }
            //CRC寄存器的高低位进行互换
            byte[] crc16 = new byte[2];
            //CRC寄存器的高8位变成低8位，
            crc16[1] = (byte)((crc >> 8) & 0xff);
            //CRC寄存器的低8位变成高8位
            crc16[0] = (byte)(crc & 0xff);
            return crc16;
        }

        /// <summary>
        /// CRC校验，参数为空格或逗号间隔的字符串
        /// </summary>
        /// <param name="data">校验数据，逗号或空格间隔的16进制字符串(带有0x或0X也可以),逗号与空格不能混用</param>
        /// <returns>字节0是高8位，字节1是低8位</returns>
        public static byte[] CRCCalc(string data)
        {
            //分隔符是空格还是逗号进行分类，并去除输入字符串中的多余空格
            IEnumerable<string> datac = data.Contains(",") ? data.Replace(" ", "").Replace("0x", "").Replace("0X", "").Trim().Split(',') : data.Replace("0x", "").Replace("0X", "").Split(' ').ToList().Where(u => u != "");
            List<byte> bytedata = new List<byte>();
            foreach (string str in datac)
            {
                bytedata.Add(byte.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier));
            }
            byte[] crcbuf = bytedata.ToArray();
            //crc计算赋初始值
            return CRCCalc(crcbuf);
        }


        /// <summary>
        ///  CRC校验，截取data中的一段进行CRC16校验
        /// </summary>
        /// <param name="data">校验数据，字节数组</param>
        /// <param name="offset">从头开始偏移几个byte</param>
        /// <param name="length">偏移后取几个字节byte</param>
        /// <returns>字节0是高8位，字节1是低8位</returns>
        public static byte[] CRCCalc(byte[] data, int offset, int length)
        {
            byte[] Tdata = data.Skip(offset).Take(length).ToArray();
            return CRCCalc(Tdata);
        }

        #endregion
    }
}
