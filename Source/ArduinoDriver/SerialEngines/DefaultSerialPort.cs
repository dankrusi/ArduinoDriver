using System.IO.Ports;

namespace ArduinoDriver.SerialEngines
{
    public class DefaultSerialPort : SerialPort, ISerialPortEngine
    {
		public void Flush() {
			base.BaseStream.Flush();
		}
    }
}
