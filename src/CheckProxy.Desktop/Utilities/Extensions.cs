using DireBlood.Core.Jobs;
using System.IO;
using System.Threading.Tasks;

namespace CheckProxy.Desktop.Utilities
{
    public static class Extensions
    {
        public static int CountLines(this StreamReader reader)
        {
            int lines = 0;
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