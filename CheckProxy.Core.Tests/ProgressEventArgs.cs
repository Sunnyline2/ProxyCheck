namespace CheckProxy.Core.Tests
{
    public class ProgressEventArgs
    {
        public int Current { get; set; }
        public int Max { get; set; }

        public override string ToString()
        {
            return $"{Current}/{Max}";
        }
    }
}