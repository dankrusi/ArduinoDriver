# ArduinoDriver

A cross-platform .NET library to connect, drive and debug an Arduino through a simple and highly Arduino syntax compatible request / response protocol running over the serial (USB) connection.

![ArduinoDriver](https://github.com/christophediericx/ArduinoDriver/blob/master/Images/ArduinoDriver.png)

## Summary ##
A two-line snippet can illustrate some of the library's features:
```csharp
var driver = new ArduinoDriver(ArduinoModel.Mega2560, "/dev/ttyACM0");
driver.Send(new ToneRequest(8, 200, 1000));
```

* This creates an ArduinoDriver for a specific Arduino Model (in this case an Uno) at it's port.

* Use the *Send* method on the driver in order to send a message to the Arduino, and receive a response. Most of the typical Arduino library methods have completely analogous request / counterparts:

  * AnalogReadRequest / AnalogReadResponse
  * AnalogWriteRequest / AnalogWriteResponse
  * DigitalReadRequest / DigitalReadResponse
  * DigitalWriteRequest / DigitalWriteResponse
  * PinModeRequest / PinModeResponse
  * ...

The protocol itself supports:
* Handshaking and version negotation
* High speed communication
* Fault tolerance and error detection (through fletcher-16 checksums)

## Compatibility ##

The library has been tested with the following configurations:

| Arduino Model | MCU           |
| ------------- |:-------------:|
| Mega 2560     | ATMega2560    |

> *If you have a need for this library to run on another Arduino model, feel free to open an issue on GitHub, it should be relatively straightforward to add support (for most).*

## How to use the .NET library ##

Download the source and add the project to your solution. There is no nuget package as this is a fork.

Protip: Use git submodule

```
git submodule add https://github.com/dankrusi/ArduinoDriver.git ArduinoDriver
```

Install the ArduinoListener sketch on your arduino device: https://github.com/dankrusi/ArduinoDriver/blob/master/Source/ArduinoDriver/ArduinoListener/ArduinoListener.ino

## Dependencies ##

The only dependency is NLog. This is how the fork https://github.com/dankrusi/ArduinoDriver varies from the original https://github.com/christophediericx/ArduinoDriver project. No need to compile a 3rd party SerialPortStream library, and the project will build and run out-of-the box on Ubuntu 14.04+, Windows, and OS X.

## Logging ##

The library channels log messages (in varying levels, from Info to Trace) via NLog. Optionally, add a nuget NLog dependency (and configuration file) in any project that uses ArduinoDriver in order to redirect these log messages to preferred log targets.

## Sample Code Project: Blink ##

This sample project uses the library above to blink the arduino's built-in LED on pin 13.

Don't forget to change the model and port to match your device.

```csharp
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
```

## Sample Code Project: Super Mario Bros "Underworld" theme ##

This sample project uses the library above to play this classic retro tune on an Arduino with C#.

One pin of the buzzer should be connected to digital pin 8. The other pin should be connected to GND.

Don't forget to change the following lines if you have another Arduino model attached:

```csharp
// ----------> CHANGE THIS!
private const ArduinoModel AttachedArduino = ArduinoModel.Mega2560;
private const string AttachedPort = "/dev/ttyACM0";
```
