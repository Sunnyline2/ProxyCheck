using System.IO;

namespace DireBlood.Utilities
{
    public static class Extensions
    {
        public static int CountLines(this StreamReader reader)
        {
            var lines = 0;
            while (!reader.EndOfStream)
            {
                reader.ReadLine();
                lines++;
            }

            reader.BaseStream.Position = 0;
            return lines;
        }
    }
}