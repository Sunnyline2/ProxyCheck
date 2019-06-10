using System;

namespace DireBlood.Core.Utilities
{
    public static class Guard
    {
        public static void NotNullOrEmpty(this string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
