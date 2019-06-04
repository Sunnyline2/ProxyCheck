using System;

namespace DireBlood.EventArgs
{
    public class ProxyCheckingEventArgs : System.EventArgs
    {
        public int Current { get; set; }
        public int Count { get; set; }

        public int Good { get; set; }
        public int Bad { get; set; }

        public int GetPergentage()
        {
            return (int) Math.Round((double) (100 * Current) / Count);
        }
    }
}