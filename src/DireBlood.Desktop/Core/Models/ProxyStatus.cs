using System;
using GalaSoft.MvvmLight;

namespace DireBlood.Core.Models
{
    public class ProxyStatus : ViewModelBase
    {
        private bool isResponding;

        private bool wasVerified;

        private DateTime? wasVerifiedAt;

        public bool IsResponding
        {
            get => isResponding;
            set => Set(ref isResponding, value);
        }

        public bool WasVerified
        {
            get => wasVerified;
            set => Set(ref wasVerified, value);
        }

        public DateTime? WasVerifiedAt
        {
            get => wasVerifiedAt;
            set => Set(ref wasVerifiedAt, value);
        }
    }
}