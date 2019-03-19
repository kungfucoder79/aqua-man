using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Aqua_ControlUWP
{
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
        private AquaI2C _aquaI2C;
        

        public MainPage()
        {
            InitializeComponent();

            _aquaI2C = new AquaI2C();

            _aquaGPIO = new AquaGPIO();

            _aquaGPIO.DrainDone += _aquaGPIO_DrainDone;
            _aquaGPIO.FillDone += _aquaGPIO_FillDone;

            periodicTimer = new Timer(I2CCheck, null, 0, 250);
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

        private void I2CCheck(object state)
        {
            string xText, yText, zText, wText, meas1doneText;
            string statusText;

            try
            {
                double waterHeight = _aquaI2C.GetWaterHeight();

                meas1doneText = string.Format("Meas1 Done = {0:F3}", _aquaI2C.DataIn.DataLSB);

                xText = string.Format("Data1: {0:F}", _aquaI2C.FinalCapMeasure1);
                yText = string.Format("Data2: {0:F}", _aquaI2C.FinalCapMeasure2);
                zText = string.Format("Data3: {0:F}", _aquaI2C.FinalCapMeasure3);
                wText = string.Format("Data4: {0:F}", _aquaI2C.FinalCapMeasure1);
                statusText = string.Format("Cpf: {0:F}", waterHeight);
            }
            catch (Exception ex)
            {
                xText = "Data1: Error";
                yText = "Data2: Error";
                zText = "Data3: Error";
                wText = "Data4: Error";
                meas1doneText = "DataMeas: Err";
                statusText = "Failed to read from FDC1004: " + ex.Message;
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
            SetGUItoFill();
        }

        private void SetGUItoFill()
        {
            Pump_Display.Fill = GreenBrush;
            Solenoid_Fill_Display.Fill = GreenBrush;
            Solenoid_In_Display.Fill = GreenBrush;
            Solenoid_Out_Display.Fill = redBrush;
            Solenoid_Waste_Display.Fill = redBrush;
        }

        // GUI click event for draining.  Will start the associated timer and set the selected GPIO pins LOW
        // to turn on the correct valves.
        private void Drain_Click(object sender, RoutedEventArgs e)
        {
            GpioPinValue pumppinValue = _aquaGPIO.Drain();
            SetGUItoDrain();
        }

        private void SetGUItoDrain()
        {
            Pump_Display.Fill = GreenBrush;
            Solenoid_Fill_Display.Fill = redBrush;
            Solenoid_In_Display.Fill = redBrush;
            Solenoid_Out_Display.Fill = GreenBrush;
            Solenoid_Waste_Display.Fill = GreenBrush;
        }

        private void MainPage_Unloaded(object sender, object args)
        {
            _aquaI2C.I2CSensor.Dispose();
        }

        
        private void Reset_I2C_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _aquaI2C.Reset();
            }
            catch (Exception ex)
            {
                Text_Status.Text = "Failed to communicate with device: " + ex.Message;
            }
        }

        private void Feeder_Button_Click(object sender, RoutedEventArgs e)
        {
            _aquaGPIO.FeedMe(6400);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            // Clicking Button will Reset Raspberry Pi and perform system restart
            // If this is clicked during debug, the host computer looses connection
            ShutdownManager.BeginShutdown(ShutdownKind.Restart, TimeSpan.FromSeconds(0));
        }
    }

}