using System.Threading;
using ArduinoDriver.SerialProtocol;

namespace ArduinoDriver.Samples.Blink
{
	/// <summary>
	/// This is a very simple test program that makes the arduino blink using the digital pin 13.
	/// </summary>
	internal class Program
	{
		// ----------> CHANGE THIS!
		private const ArduinoModel AttachedArduino = ArduinoModel.Mega2560;
		private const string AttachedPort = "/dev/ttyACM0";

		private const int DigitalPinBlink = 13;


		private static void Main(string[] args)
		{
			using (var driver = new ArduinoDriver(AttachedArduino, AttachedPort))
			{
				driver.Send(new PinModeRequest(DigitalPinBlink, PinMode.Output));
				for (var i = 0; i < 20; i++)
				{
					driver.Send(new DigitalWriteRequest(DigitalPinBlink, DigitalValue.High));
					Thread.Sleep(1000);
					driver.Send(new DigitalWriteRequest(DigitalPinBlink, DigitalValue.Low));
					Thread.Sleep(1000);
				}
			}

		}
	}
}