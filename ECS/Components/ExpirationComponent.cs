using System;
namespace holmgang.Desktop
{
    public class ExpirationComponent : Component
    {
        public double timeLeft; // in seconds

        public ExpirationComponent(double timeLeft)
        {
            this.timeLeft = timeLeft;
        }
    }
}
