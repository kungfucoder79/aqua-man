using System;
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

namespace BlinkyHeaded
{

    public sealed partial class MainPage : Page
    {
        private DispatcherTimer timer_fill;
        private DispatcherTimer timer_drain;
        private int time_fill_event = 0;
        private int time_drain_event = 0;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush GreenBrush = new SolidColorBrush(Windows.UI.Colors.Green);

        private const int Fill_Pin = 20;
        private GpioPin Fillpin;
        private GpioPinValue FillpinValue;

        private const int Waste_Pin = 21;
        private GpioPin Wastepin;
        private GpioPinValue WastepinValue;

        private const int In_Pin = 19;
        private GpioPin Inpin;
        private GpioPinValue InpinValue;

        private const int Out_Pin = 26;
        private GpioPin Outpin;
        private GpioPinValue OutpinValue;

        private const int Pump_Pin = 16;
        private GpioPin Pumppin;
        private GpioPinValue PumppinValue;


        public MainPage()
        {
            this.InitializeComponent();

            timer_fill = new DispatcherTimer();
            timer_fill.Interval = TimeSpan.FromSeconds(10);
            timer_fill.Tick += Timer_fill_Tick;

            timer_drain = new DispatcherTimer();
            timer_drain.Interval = TimeSpan.FromSeconds(5);
            timer_drain.Tick += Timer_drain_Tick;

            InitGPIO();
                      
        }

        private void InitGPIO()
        {
            GpioController gpio = GpioController.GetDefault();
            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                Fillpin = null;
                GpioStatus.Text = "No GPIO controller on device.";
                return;
            }

            Fillpin = gpio.OpenPin(Fill_Pin);
            FillpinValue = GpioPinValue.High;
            Fillpin.Write(FillpinValue);
            Fillpin.SetDriveMode(GpioPinDriveMode.Output);

            Wastepin = gpio.OpenPin(Waste_Pin);
            WastepinValue = GpioPinValue.High;
            Wastepin.Write(WastepinValue);
            Wastepin.SetDriveMode(GpioPinDriveMode.Output);

            Inpin = gpio.OpenPin(In_Pin);
            InpinValue = GpioPinValue.High;
            Inpin.Write(InpinValue);
            Inpin.SetDriveMode(GpioPinDriveMode.Output);

            Outpin = gpio.OpenPin(Out_Pin);
            OutpinValue = GpioPinValue.High;
            Outpin.Write(OutpinValue);
            Outpin.SetDriveMode(GpioPinDriveMode.Output);

            Pumppin = gpio.OpenPin(Pump_Pin);
            PumppinValue = GpioPinValue.High;
            Pumppin.Write(PumppinValue);
            Pumppin.SetDriveMode(GpioPinDriveMode.Output);

            GpioStatus.Text = "GPIO pin initialized.";
        }

        private void Timer_fill_Tick(object sender, object e)
        {

            if (PumppinValue == GpioPinValue.High)
            {
                PumppinValue = GpioPinValue.Low;
                Pumppin.Write(PumppinValue);

                InpinValue = GpioPinValue.Low;
                Inpin.Write(InpinValue);

                FillpinValue = GpioPinValue.Low;
                Fillpin.Write(FillpinValue);

                OutpinValue = GpioPinValue.Low;
                Outpin.Write(OutpinValue);

                WastepinValue = GpioPinValue.Low;
                Wastepin.Write(WastepinValue);

                Pump_Display.Fill = redBrush;
                Solenoid_Fill_Display.Fill = redBrush;
                Solenoid_In_Display.Fill = redBrush;
                Solenoid_Out_Display.Fill = redBrush;
                Solenoid_Waste_Display.Fill = redBrush;

                if (time_fill_event > 0)
                {
                    timer_fill.Stop();
                    time_fill_event = 0;
                }

            }
            else
            {
                PumppinValue = GpioPinValue.High;
                Pumppin.Write(PumppinValue);

                InpinValue = GpioPinValue.High;
                Inpin.Write(InpinValue);

                FillpinValue = GpioPinValue.High;
                Fillpin.Write(FillpinValue);

                OutpinValue = GpioPinValue.Low;
                Outpin.Write(OutpinValue);

                WastepinValue = GpioPinValue.Low;
                Wastepin.Write(WastepinValue);

                Pump_Display.Fill = GreenBrush;
                Solenoid_Fill_Display.Fill = GreenBrush;
                Solenoid_In_Display.Fill = GreenBrush;
                Solenoid_Out_Display.Fill = redBrush;
                Solenoid_Waste_Display.Fill = redBrush;

                time_fill_event++;
            }
        }
        private void Timer_drain_Tick(object sender, object e)
        {
            if (PumppinValue == GpioPinValue.High)
            {
                PumppinValue = GpioPinValue.Low;
                Pumppin.Write(PumppinValue);

                OutpinValue = GpioPinValue.Low;
                Outpin.Write(OutpinValue);

                WastepinValue = GpioPinValue.Low;
                Wastepin.Write(WastepinValue);

                InpinValue = GpioPinValue.Low;
                Inpin.Write(InpinValue);

                FillpinValue = GpioPinValue.Low;
                Fillpin.Write(FillpinValue);

                Pump_Display.Fill = redBrush;
                Solenoid_Fill_Display.Fill = redBrush;
                Solenoid_In_Display.Fill = redBrush;
                Solenoid_Out_Display.Fill = redBrush;
                Solenoid_Waste_Display.Fill = redBrush;

                if (time_drain_event > 0)
                {
                    timer_drain.Stop();
                    time_drain_event = 0;
                }
            }
            else
            {
                PumppinValue = GpioPinValue.High;
                Pumppin.Write(PumppinValue);

                InpinValue = GpioPinValue.Low;
                Inpin.Write(InpinValue);

                FillpinValue = GpioPinValue.Low;
                Fillpin.Write(FillpinValue);

                OutpinValue = GpioPinValue.High;
                Outpin.Write(OutpinValue);

                WastepinValue = GpioPinValue.High;
                Wastepin.Write(WastepinValue);

                Pump_Display.Fill = GreenBrush;
                Solenoid_Fill_Display.Fill = redBrush;
                Solenoid_In_Display.Fill = redBrush;
                Solenoid_Out_Display.Fill = GreenBrush;
                Solenoid_Waste_Display.Fill = GreenBrush;

                time_drain_event++;

            }
        }

        private void Fill_Click(object sender, RoutedEventArgs e)
        {
            timer_fill.Start();
        }


        private void Drain_Click(object sender, RoutedEventArgs e)
        {
            timer_drain.Start();
        }
    }

}