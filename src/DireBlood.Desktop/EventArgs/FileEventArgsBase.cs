using System;

namespace DireBlood.EventArgs
{
    public abstract class FileEventArgsBase : System.EventArgs
    {
        public int Current { get; set; }
        public int Count { get; set; }

        public int GetPercentage
        {
            get
            {
                return (int)Math.Round((double)(100 * Current) / Count);
            }
        }
    }
}