using System;

namespace CheckProxy.Desktop.EventArgs
{
    public class FileReadingEventArgs : System.EventArgs
    {
        public int Current { get; set; }
        public int Count { get; set; }

        public int GetPergentage()
        {
            return (int) Math.Round((double) (100 * Current) / Count);
        }
    }
}