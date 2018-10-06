using Unosquare.RaspberryIO.Gpio;

namespace Aqua_Control
{
    public interface IAquaPinController
    {
        void Drain();

        void Fill();

        void FeedMe(int numberOfTimes);
    }
}