using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace L2Bot_Controller
{
    class LoCoMoCo
    {
        const byte STOP = 0x7F;
        const byte FLOAT = 0x0F;
        const byte FORWARD = 0x6f;
        const byte BACKWARD = 0x5F;
        SerialPort _serialPort;

        public LoCoMoCo(String port)
        {
            try
            {
                _serialPort = new SerialPort(port);
                _serialPort.BaudRate = 2400;
                _serialPort.DataBits = 8;
                _serialPort.Parity = Parity.None;
                _serialPort.StopBits = StopBits.Two;
                _serialPort.Open();
            }
            catch
            {

            }
        }

        public void move(byte left, byte right)
        {
            try
            {
                byte[] buffer = { 0x01, left, right };
                _serialPort.Write(buffer, 0, 3);
            }
            catch
            {

            }
        }

        public void stop()
        {
            move(STOP, STOP);
        }

        public void floatstop()
        {
            move(FLOAT, FLOAT);
        }

        public void forward()
        {
            move(FORWARD, FORWARD);
        }

        public void backward()
        {
            move(BACKWARD, BACKWARD);
        }

        public void turnright()
        {
            move(FORWARD, STOP);
        }

        public void turnleft()
        {
            move(STOP, FORWARD);
        }

        public void close()
        {
            _serialPort.Close();
        }

    }
}
