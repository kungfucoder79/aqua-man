using Unosquare.RaspberryIO.Gpio;

namespace Aqua_Control
{
    public interface IAquaI2CController
    {
        OneRegisterRead DataIn { get; }
        float FinalCapMeasure1 { get; }
        float FinalCapMeasure2 { get; }
        float FinalCapMeasure3 { get; }

        /// <summary>
        /// Gets the current instance of <see cref="I2CDevice"/>
        /// </summary>
        I2CDevice I2CSensor { get; }

        /// <summary>
        /// Gets the current water height read from the sensor
        /// </summary>
        double WaterHeight { get; }

        /// <summary>
        /// Gets the change in height in the tank.
        /// </summary>
        double DeltaHeight { get; }

        /// <summary>
        /// Total volume of the tank
        /// </summary>
        double TankVolume { get; }

        /// <summary>
        /// Calabrates the water level sensor
        /// </summary>
        void CalabrateSensor();

        /// <summary>
        /// Resets the <see cref="I2CSensor"/>
        /// </summary>
        void Reset();

        /// <summary>
        /// Updates the tank specification from the user
        /// </summary>
        /// <param name="tankWidth"></param>
        /// <param name="tankHeight"></param>
        /// <param name="tankDepth"></param>
        void UpdateTankSpecs(double tankWidth, double tankHeight, double tankDepth);
    }
}