using Unosquare.RaspberryIO.Gpio;

namespace Aqua_Control
{
    public interface IAquaI2CController
    {
        OneRegisterRead DataIn { get; }
        float FinalCapMeasure1 { get; }
        float FinalCapMeasure2 { get; }
        float FinalCapMeasure3 { get; }
        I2CDevice I2CSensor { get; }

        double GetWaterHeight();
        void Reset();
    }
}