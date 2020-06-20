using System;

namespace DireBlood.Core.Jobs.Services
{
    public static class Guard
    {
        public static class Against
        {
            public static void Null<T>(T paramValue, string paramName) where T : class
            {
                if (paramValue is null)
                {
                    throw new ArgumentNullException(paramName);
                }
            }

            public static void NegativeOrZero(short paramValue, string paramName)
            {
                NegativeOrZero((int)paramValue, paramName);
            }

            public static void NegativeOrZero(ushort paramValue, string paramName)
            {
                NegativeOrZero((int)paramValue, paramName);
            }

            public static void NegativeOrZero(uint paramValue, string paramName)
            {
                NegativeOrZero((int)paramValue, paramName);
            }

            public static void NegativeOrZero(int paramValue, string paramName)
            {
                Negative(paramValue, paramName);
                Zero(paramValue, paramName);
            }

            public static void Negative(int paramValue, string paramName)
            {
                if (paramValue < 0)
                {
                    throw new ArgumentOutOfRangeException(paramName);
                }
            }

            public static void Zero(int paramValue, string paramName)
            {
                if (paramValue == 0)
                {
                    throw new ArgumentOutOfRangeException(paramName);
                }
            }

            public static void NullOrEmpty(string paramValue, string paramName)
            {
                if (string.IsNullOrEmpty(paramValue))
                {
                    throw new ArgumentNullException(paramName);
                }
            }
        }
    }
}