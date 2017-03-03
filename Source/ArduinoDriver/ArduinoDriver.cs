using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using ArduinoDriver.SerialEngines;
using ArduinoDriver.SerialProtocol;
using NLog;

namespace ArduinoDriver
{
    /// <summary>
    /// An ArduinoDriver can be used to communicate with an attached Arduino by way of sending requests and receiving responses.
    /// These messages are sent over a live serial connection (via a serial protocol) to the Arduino. The required listener can be 
    /// automatically deployed to the Arduino.
    /// </summary>
    public class ArduinoDriver : IDisposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IList<ArduinoModel> alwaysRedeployListeners = 
            new List<ArduinoModel> { ArduinoModel.NanoR3 };
        private const int defaultRebootGraceTime = 2000;
        private readonly IDictionary<ArduinoModel, int> rebootGraceTimes = 
            new Dictionary<ArduinoModel, int>
            {
                {ArduinoModel.Micro, 8000},
                {ArduinoModel.Mega2560, 4000}
            };
        private const int CurrentProtocolMajorVersion = 1;
        private const int CurrentProtocolMinorVersion = 0;
		private const int DriverBaudRate = 115200; //115200
        private ArduinoDriverSerialPort port;
        private ArduinoDriverConfiguration config;
        private Func<ISerialPortEngine> engineFunc;



        /// <summary>
        /// Creates a new ArduinoDriver instance for a specified portName.
        /// </summary>
        /// <param name="arduinoModel"></param>
        /// <param name="portName">The COM portname to create the ArduinoDriver instance for.</param>
        /// <param name="autoBootStrap">Determines if an listener is automatically deployed to the Arduino if required.</param>
        public ArduinoDriver(ArduinoModel arduinoModel, string portName)
        {
            Initialize(new ArduinoDriverConfiguration
            {
                ArduinoModel = arduinoModel,
                PortName = portName
            });
        }

        /// <summary>
        /// Sends an Analog Read Request to the Arduino.
        /// </summary>
        /// <param name="request">Analog Read Request</param>
        /// <returns>The Analog Read Response</returns>
        public AnalogReadResponse Send(AnalogReadRequest request)
        {
            return (AnalogReadResponse) InternalSend(request);
        }

        /// <summary>
        /// Sends an Analog Write Request to the Arduino.
        /// </summary>
        /// <param name="request">Analog Write Request</param>
        /// <returns>The Analog Write Response</returns>
        public AnalogWriteResponse Send(AnalogWriteRequest request)
        {
            return (AnalogWriteResponse) InternalSend(request);
        }

        /// <summary>
        /// Sends an Digital Read Request to the Arduino.
        /// </summary>
        /// <param name="request">Digital Read Request</param>
        /// <returns>The Digital Read Response</returns>
        public DigitalReadResponse Send(DigitalReadRequest request)
        {
            return (DigitalReadResponse) InternalSend(request);
        }

        /// <summary>
        /// Sends an Digital Write Request to the Arduino.
        /// </summary>
        /// <param name="request">Digital Write Request</param>
        /// <returns>The Digital Write Response</returns>
        public DigitalWriteReponse Send(DigitalWriteRequest request)
        {
            return (DigitalWriteReponse) InternalSend(request);
        }

        /// <summary>
        /// Sends an PinMode Request to the Arduino.
        /// </summary>
        /// <param name="request">PinMode Request</param>
        /// <returns>The PinMode Response</returns>
        public PinModeResponse Send(PinModeRequest request)
        {
            return (PinModeResponse) InternalSend(request);
        }

        /// <summary>
        /// Sends an Tone Request to the Arduino.
        /// </summary>
        /// <param name="request">Tone Request</param>
        /// <returns>The Tone Response</returns>
        public ToneResponse Send(ToneRequest request)
        {
            return (ToneResponse) InternalSend(request);
        }

        /// <summary>
        /// Sends an NoTone Request to the Arduino.
        /// </summary>
        /// <param name="request">NoTone Request</param>
        /// <returns>The NoTone Response</returns>
        public NoToneResponse Send(NoToneRequest request)
        {
            return (NoToneResponse) InternalSend(request);
        }

        /// <summary>
        /// Disposes the ArduinoDriver instance.
        /// </summary>
        public void Dispose()
        {
            try
            {
                port.Close();
                port.Dispose();
            }
            catch (Exception)
            {
                // Ignore
            }
        }

        #region Private Methods

        private void Initialize(ArduinoDriverConfiguration config)
        {
            logger.Info("Instantiating ArduinoDriver: {0} - {1}...", config.ArduinoModel, config.PortName);

            this.config = config;

            engineFunc = () => new DefaultSerialPort { PortName = config.PortName, BaudRate = DriverBaudRate };

			if(!config.AutoBootstrap)
				InitializeWithoutAutoBootstrap();
			else
				throw new NotImplementedException();
        }

        private void InitializeWithoutAutoBootstrap()
        {
            // Without auto bootstrap, we just try to send a handshake request (the listener should already be
            // deployed). If that fails, we try nothing else.
            logger.Info("Initiating handshake...");
            InitializePort();
            var handshakeResponse = ExecuteHandshake();
            if (handshakeResponse == null)
            {
                port.Close();
                port.Dispose();
                throw new IOException(
                    string.Format(
                        "Unable to get a handshake ACK when sending a handshake request to the Arduino on port {0}. "
                        +
                        "Pass 'true' for optional parameter autoBootStrap in one of the ArduinoDriver constructors to "
                        + "automatically configure the Arduino (please note: this will overwrite the existing sketch "
                        + "on the Arduino).", config.PortName));
            }
        }

        private void InitializePort()
        {
            var engine = engineFunc();
            var portName = engine.PortName;
            var baudRate = engine.BaudRate;
            logger.Debug("Initializing port {0} - {1}...", portName, baudRate);
            engine.WriteTimeout = 100;
            engine.ReadTimeout = 100;
            port = new ArduinoDriverSerialPort(engine);
            port.Open();
			System.Threading.Thread.Sleep(1000);
        }

        private ArduinoResponse InternalSend(ArduinoRequest request)
        {
            return port.Send(request);
        }

        private HandShakeResponse ExecuteHandshake()
        {
            var response = port.Send(new HandShakeRequest(), 1);
            return response as HandShakeResponse;            
        }

        private static void ExecuteAutoBootStrap(ArduinoModel arduinoModel, string portName)
        {
			throw new NotImplementedException();
        }

        #endregion
    }
}
