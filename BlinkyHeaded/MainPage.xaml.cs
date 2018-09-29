using System;
using System.Threading;
using Windows.Devices.Enumeration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.I2c;
using Windows.System;
using System.Threading.Tasks;

namespace Aqua_ControlUWP
{

    struct OneRegisterRead
    {
        public int DataMSB;
        public int DataLSB;
    };
    struct CapMeasure1_1
    {
        public int CapReadLSB1_1;
        public int CapreadMSB1_1;
        public int CapReadLSB1_2;
        public int CapreadMSB1_2;
    };

    //struct CapMeasure1_2
    //{
    //    public int CapReadLSB1_2;
    //    public int CapreadMSB1_2;
    //};

    
    public sealed partial class MainPage : Page
    {
        // Initiate seeting and parameters for code.  There are two delcared timers using the DispatchTimer
        // as well as declaring color fill events for the xaml GUI screen.
        // Also declaring all the necessary variables for the different GPIO pins that are used on the
        // Raspberry Pi.

        private Timer periodicTimer;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush GreenBrush = new SolidColorBrush(Windows.UI.Colors.Green);

        private AquaGPIO _aquaGPIO;
        //Declared variables for I2C
        private I2cDevice I2CSensor;
        private const byte FDC1004_I2C_ADDR = 0x50;
        private const byte MEAS_ONE_CONTROL = 0x08;         // Address of the Measurement #1 register  
        private const byte MEAS_TWO_CONTROL = 0x09;         // Address of the Measurement #2 register    
        private const byte MEAS_THREE_CONTROL = 0x0A;       // Address of the Measurement #3 register 
        private const byte CIN1_CONTROL = 0x0D;             // Address of the Cin1 register
        private const byte FDC_CONF_CONTROL = 0x0C;         // Address of the FDC configuration register
        public float FinalCapMeasure1, FinalCapMeasure2, FinalCapMeasure3;

        public MainPage()
        {
            InitializeComponent();

            _aquaGPIO = new AquaGPIO();

            _aquaGPIO.DrainDone += _aquaGPIO_DrainDone;
            _aquaGPIO.FillDone += _aquaGPIO_FillDone;

            InitI2C();
        }

        private void _aquaGPIO_FillDone(object sender, EventArgs e)
        {
            SetButtonToRed();
        }

        private void _aquaGPIO_DrainDone(object sender, EventArgs e)
        {
            SetButtonToRed();
        }

        private void SetButtonToRed()
        {
            Pump_Display.Fill = redBrush;
            Solenoid_Fill_Display.Fill = redBrush;
            Solenoid_In_Display.Fill = redBrush;
            Solenoid_Out_Display.Fill = redBrush;
            Solenoid_Waste_Display.Fill = redBrush;
        }

        // Initialization for I2C FDC1004 Capaciatance Sensor 
        private async void InitI2C()
        {            
            I2cConnectionSettings settings = new I2cConnectionSettings(FDC1004_I2C_ADDR);
            settings.BusSpeed = I2cBusSpeed.FastMode;                       // 400KHz bus speed 
            I2cController controller = await Windows.Devices.I2c.I2cController.GetDefaultAsync();
            I2CSensor = controller.GetDevice(settings);    // Create an I2cDevice with our selected bus controller and I2C settings    
             /* 
             * Initialize the FDC1004 capacitance sensor:
             *
             * For this device, we create 3-byte write buffers: ==> this device uses 
             * The first byte is the register address we want to write to.
             * The second byte is the contents of the MSB that we want to write to the register. 
             * The third byte is the contents of the LSB that we want to write to the register.
             */
            byte[] WriteBuf_MeasurementOneFormat = new byte[] { MEAS_ONE_CONTROL, 0x1c, 0x00 };     // configs measurement 1 
            byte[] WriteBuf_MeasurementTwoFormat = new byte[] { MEAS_TWO_CONTROL, 0x3c, 0x00 };     // configs measurement 2                         
            byte[] WriteBuf_MeasurementThreeFormat = new byte[] { MEAS_THREE_CONTROL, 0x5c, 0x00 }; // configs measurement 3                        
            byte[] WriteBuf_Cin1 = new byte[] { CIN1_CONTROL, 0x30, 0x00 };             // Set Offset for Cin1 to "6"pF based on datsheet calculations
            byte[] WriteBuf_FDC_Config = new byte[] { FDC_CONF_CONTROL, 0x0D, 0xE0 };   //set to read at 400S/s with repeat and read at measurement #1,#2,#3
            // Write the register settings
            try
            {
                I2CSensor.Write(WriteBuf_MeasurementOneFormat);
                I2CSensor.Write(WriteBuf_MeasurementTwoFormat);                
                I2CSensor.Write(WriteBuf_MeasurementThreeFormat);
                I2CSensor.Write(WriteBuf_Cin1);
                I2CSensor.Write(WriteBuf_FDC_Config);
                Text_Status.Text = "Status: Running";
            }
            /* If the write fails display the error and stop running */
            catch (Exception ex)
            {
                Text_Status.Text = "Failed to communicate with device: " + ex.Message;
                return;
            }
            // Create timer to pull data from FDC1004 sensor at given interval
            periodicTimer = new Timer(this.TimerCallback, null, 0, 250);
        }

        private void TimerCallback(object state)
        {
            string xText, yText, zText, wText, meas1doneText;
            string statusText;
            
            try
            {
                byte[] FDCCongAddrBuf = new byte[] { 0x0C };
                byte[] Meas1AddrBufLSB = new byte[] { 0x00 };
                byte[] Meas1AddrBufMSB = new byte[] { 0x01 };
                byte[] Meas2AddrBufLSB = new byte[] { 0x02 };
                byte[] Meas2AddrBufMSB = new byte[] { 0x03 };
                byte[] Meas3AddrBufLSB = new byte[] { 0x04 };
                byte[] Meas3AddrBufMSB = new byte[] { 0x05 };

                OneRegisterRead DataIn = ReadOneReg(FDCCongAddrBuf);
                //int Measurement_1_Done = ((byte)(0x08 & DataIn.DataLSB) >> 3);
                //int Measurement_2_Done = ((byte)(0x04 & DataIn.DataLSB) >> 2);
                //int Measurement_3_Done = ((byte)(0x02 & DataIn.DataLSB) >> 1);
                meas1doneText = string.Format("Meas1 Done = {0:F3}", DataIn.DataLSB);

                //if (Measurement_1_Done == 1)
                //{
                //}

                //CapMeasure1_1 Capread1 = readcapsen1_1(Meas1AddrBufLSB);
                //CapMeasure1_2 Capread2 = readcapsen1_2(Meas1AddrBufMSB);
                //int CapLabel1_1 = Capread1.CapreadMSB1_1 * 256 + Capread1.CapReadLSB1_1;
                //int CapLabel1_2 = Capread2.CapreadMSB1_2 * 256 + Capread2.CapReadLSB1_2;
                //int CapLabel1Both = (CapLabel1_1 * 65536 + CapLabel1_2) >> 8;
                //FinalCapMeasure1 = CapLabel1Both / 524288;

                FinalCapMeasure1 = ReadCapSen1_1(Meas1AddrBufLSB, Meas1AddrBufMSB);
                FinalCapMeasure2 = ReadCapSen1_1(Meas2AddrBufLSB, Meas2AddrBufMSB);
                FinalCapMeasure3 = ReadCapSen1_1(Meas3AddrBufLSB, Meas3AddrBufMSB);

                double WaterHeight = (5 * ((FinalCapMeasure2 - 1.80) / (FinalCapMeasure1 - FinalCapMeasure3)));
                //xText = string.Format("Data1: {0:F3}", Capread1.CapReadLSB1_1);
                //yText = string.Format("Data2: {0:F3}", Capread1.CapreadMSB1_1);
                //zText = string.Format("Data3: {0:F0}", Capread2.CapReadLSB1_2);
                //wText = string.Format("Data4: {0:F0}", Capread2.CapreadMSB1_2);

                xText = string.Format("Data1: {0:F}", FinalCapMeasure1);
                yText = string.Format("Data2: {0:F}", FinalCapMeasure2);
                zText = string.Format("Data3: {0:F}", FinalCapMeasure3);
                wText = string.Format("Data4: {0:F}", FinalCapMeasure1);
                statusText = string.Format("Cpf: {0:F}", WaterHeight);
            }
            catch (Exception ex)
            {
                xText = "Data1: Error";
                yText = "Data2: Error";
                zText = "Data3: Error";
                wText = "Data4: Error";
                meas1doneText = "DataMeas: Err";
                statusText = "Failed to read from FDC1004: " + ex.Message;

                //I2CData1.Text = xText;
                //I2CData2.Text = yText;
                //I2CData3.Text = zText;
                //I2CData4.Text = wText;
                //Text_Status.Text = statusText;
            }

            /* UI updates must be invoked on the UI thread */
            Windows.Foundation.IAsyncAction task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                I2CData1.Text = xText;
                I2CData2.Text = yText;
                I2CData3.Text = zText;
                I2CData4.Text = wText;
                Measurement1Done.Text = meas1doneText;
                Text_Status.Text = statusText;
            });
        }

        // GUI click event for filling.  Will start the associated timer and set the selected GPIO pins LOW
        // to turn on the correct valve.
        private void Fill_Click(object sender, RoutedEventArgs e)
        {
            GpioPinValue pumppinValue = _aquaGPIO.Fill();
            if (pumppinValue == GpioPinValue.Low)
            {
                Pump_Display.Fill = GreenBrush;
                Solenoid_Fill_Display.Fill = GreenBrush;
                Solenoid_In_Display.Fill = GreenBrush;
                Solenoid_Out_Display.Fill = redBrush;
                Solenoid_Waste_Display.Fill = redBrush;
            }

        }

        // GUI click event for draining.  Will start the associated timer and set the selected GPIO pins LOW
        // to turn on the correct valves.
        private void Drain_Click(object sender, RoutedEventArgs e)
        {
            GpioPinValue pumppinValue = _aquaGPIO.Drain();
            if (pumppinValue == GpioPinValue.Low)
            {
                Pump_Display.Fill = GreenBrush;
                Solenoid_Fill_Display.Fill = redBrush;
                Solenoid_In_Display.Fill = redBrush;
                Solenoid_Out_Display.Fill = GreenBrush;
                Solenoid_Waste_Display.Fill = GreenBrush;                              
            }
        }

        private void MainPage_Unloaded(object sender, object args)
        {
          I2CSensor.Dispose();
        }

        private OneRegisterRead ReadOneReg(byte[] RegAddrBuf)
        {
            byte[] ReadBuf;
            ReadBuf = new byte[2];  // need to read the two bytes stored in the targeted register
            I2CSensor.WriteRead(RegAddrBuf, ReadBuf);
            // A little confusing but decided to parse the two bytes into separate bytes, convert to 'int'
            // then return this information so we can perform calculations.
            int RawData = BitConverter.ToInt16(ReadBuf, 0);

            OneRegisterRead OneRegReadDataOut;
            OneRegReadDataOut.DataMSB = 0xFF00 & RawData >> 8;
            OneRegReadDataOut.DataLSB = 0x00FF & RawData; // if reading Done bit, use this statement ==> ((byte)(0x08 & RawData) >> 3)

            return OneRegReadDataOut;            
        }

        private void Reset_I2C_Click(object sender, RoutedEventArgs e)
        {
            byte[] WriteBuf_FDC_Config = new byte[] { FDC_CONF_CONTROL, 0x8C, 0x00 };
            I2CSensor.Write(WriteBuf_FDC_Config);
            InitI2C();
        }

        private float ReadCapSen1_1(byte[] RegAddrBuf1, byte[] RegAddrBuf2)
        {
            byte[] ReadBuf1, ReadBuf2;

            ReadBuf1 = new byte[2];  /* We read 2 bytes sequentially  */
            ReadBuf2 = new byte[2];  /* We read 2 bytes sequentially  */
            I2CSensor.WriteRead(RegAddrBuf1, ReadBuf1);

            I2CSensor.WriteRead(RegAddrBuf2, ReadBuf2);
            /* In order to get the raw 16-bit data values, we need to separate the bytes */

            int RawData = BitConverter.ToInt16(ReadBuf1, 0);
            //int RawData = BitConverter.ToInt16(ReadBuf, 0);
            CapMeasure1_1 capMeas1_1;
            capMeas1_1.CapReadLSB1_1 = (0xFF00 & RawData) >> 8;
            capMeas1_1.CapreadMSB1_1 = 0x00FF & RawData;

            /* In order to get the raw 16-bit data values, we need to separate the bytes */

            RawData = BitConverter.ToInt16(ReadBuf2, 0);
            capMeas1_1.CapReadLSB1_2 = (0xFF00 & RawData) >> 8;
            capMeas1_1.CapreadMSB1_2 = 0x00FF & RawData;

            int CapLabel1_1 = (capMeas1_1.CapreadMSB1_1 * 256 + capMeas1_1.CapReadLSB1_1);
            int CapLabel1_2 = capMeas1_1.CapreadMSB1_2 * 256 + capMeas1_1.CapReadLSB1_2;
            int CapLabel1Both = (CapLabel1_1 * 256 + capMeas1_1.CapreadMSB1_2);
            float FinalCapMeasure = CapLabel1Both / 524288;

            return FinalCapMeasure;
        }

        //private CapMeasure1_1 readcapsen1_1(byte[] regaddrbuf)
        //{
        //    byte[] readbuf;

        //    readbuf = new byte[2];  /* we read 2 bytes sequentially  */
        //    I2CSensor.WriteRead(regaddrbuf, readbuf);

        //    /* in order to get the raw 16-bit data values, we need to separate the bytes */

        //    int rawdata = BitConverter.ToInt16(readbuf, 0);
        //    //int rawdata = bitconverter.toint16(readbuf, 0);
        //    int rawdata1lsb = (0xff00 & rawdata) >> 8;
        //    int rawdata2msb = 0x00ff & rawdata;

        //    CapMeasure1_1 capmeas1_1;
        //    capmeas1_1.CapreadMSB1_1 = rawdata2msb;
        //    capmeas1_1.CapReadLSB1_1 = rawdata1lsb;

        //    return capmeas1_1;
        //}

        //private CapMeasure1_2 readcapsen1_2(byte[] regaddrbuf)
        //{
        //    byte[] readbuf;

        //    readbuf = new byte[2];  /* we read 2 bytes sequentially  */
        //    I2CSensor.WriteRead(regaddrbuf, readbuf);

        //    /* in order to get the raw 16-bit data values, we need to separate the bytes */

        //    int rawdata = BitConverter.ToInt16(readbuf, 0);
        //    int rawdata1lsb = (0xff00 & rawdata) >> 8;
        //    int rawdata2msb = 0x00ff & rawdata;

        //    CapMeasure1_2 capmeas1_2;
        //    capmeas1_2.CapreadMSB1_2 = rawdata2msb;
        //    capmeas1_2.CapReadLSB1_2 = rawdata1lsb;

        //    return capmeas1_2;
        //}

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            // Clicking Button will Reset Raspberry Pi and perform system restart
            // If this is clicked during debug, the host computer looses connection
            ShutdownManager.BeginShutdown(ShutdownKind.Restart, TimeSpan.FromSeconds(0));
        }
    }

}