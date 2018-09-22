using System;
using System.Timers;
using Unosquare.RaspberryIO.Gpio;

namespace Aqua_Control
{
    public class AquaPinController
    {
        private Timer timer_fill;
        private Timer timer_drain;
        private int time_fill_event = 0;
        private int time_drain_event = 0;

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

        public AquaPinController()
        {
            timer_fill = new Timer();
            timer_fill.Interval = TimeSpan.FromSeconds(10).TotalSeconds;
            timer_fill.Elapsed += Timer_fill_Elapsed;

            timer_drain = new Timer();
            timer_drain.Interval = TimeSpan.FromSeconds(5).TotalSeconds;
            timer_drain.Elapsed += Timer_drain_Elapsed;

            InitGPIO();
        }

        private void InitGPIO()
        {
            GpioController gpio = GpioController.Instance;

            //Initialize the fill pin
            Fillpin = gpio.Pin20;
            FillpinValue = GpioPinValue.High;
            Fillpin.Write(FillpinValue);
            Fillpin.PinMode = GpioPinDriveMode.Output;

            //Initialize the Waste_Pin
            Wastepin = gpio.Pin21;
            WastepinValue = GpioPinValue.High;
            Wastepin.Write(WastepinValue);
            Wastepin.PinMode = GpioPinDriveMode.Output;

            //Initialize the In_Pin
            Inpin = gpio.Pin19;
            InpinValue = GpioPinValue.High;
            Inpin.Write(InpinValue);
            Inpin.PinMode = GpioPinDriveMode.Output;

            //Initialize the Out_Pin
            Outpin = gpio.Pin26;
            OutpinValue = GpioPinValue.High;
            Outpin.Write(OutpinValue);
            Outpin.PinMode = GpioPinDriveMode.Output;

            //Initialize the Pump_Pin
            Pumppin = gpio.Pin21;
            PumppinValue = GpioPinValue.High;
            Pumppin.Write(PumppinValue);
            Pumppin.PinMode = GpioPinDriveMode.Output;
        }

        private void InitializePin(GpioPin gpioPin, GpioPinValue gpioPinValue, GpioPinDriveMode gpioPinDriveMode)
        {
            Fillpin = gpioPin;
            FillpinValue = gpioPinValue;
            Fillpin.Write(FillpinValue);
            Fillpin.PinMode = gpioPinDriveMode;
        }

        private void Timer_fill_Elapsed(object sender, ElapsedEventArgs e)
        {
            PumppinValue = GpioPinValue.High;
            Pumppin.Write(PumppinValue);

            InpinValue = GpioPinValue.High;
            Inpin.Write(InpinValue);

            FillpinValue = GpioPinValue.High;
            Fillpin.Write(FillpinValue);

            OutpinValue = GpioPinValue.High;
            Outpin.Write(OutpinValue);

            WastepinValue = GpioPinValue.High;
            Wastepin.Write(WastepinValue);

            timer_fill.Stop();
        }

        private void Timer_drain_Elapsed(object sender, ElapsedEventArgs e)
        {
            PumppinValue = GpioPinValue.High;
            Pumppin.Write(PumppinValue);

            InpinValue = GpioPinValue.High;
            Inpin.Write(InpinValue);

            FillpinValue = GpioPinValue.High;
            Fillpin.Write(FillpinValue);

            OutpinValue = GpioPinValue.High;
            Outpin.Write(OutpinValue);

            WastepinValue = GpioPinValue.High;
            Wastepin.Write(WastepinValue);
        }

        public void Drain()
        {
            timer_drain.Start();
            if (PumppinValue == GpioPinValue.High)
            {
                PumppinValue = GpioPinValue.Low;
                Pumppin.Write(PumppinValue);

                OutpinValue = GpioPinValue.Low;
                Outpin.Write(OutpinValue);

                WastepinValue = GpioPinValue.Low;
                Wastepin.Write(WastepinValue);

                InpinValue = GpioPinValue.High;
                Inpin.Write(InpinValue);

                FillpinValue = GpioPinValue.High;
                Fillpin.Write(FillpinValue);
            }
        }
    }
}
