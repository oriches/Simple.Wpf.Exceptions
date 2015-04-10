using System;

namespace Simple.Wpf.Exceptions.Models
{
    public sealed class Memory : IEquatable<Memory>
    {
        public bool Equals(Memory other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return WorkingSetPrivate == other.WorkingSetPrivate && Managed == other.Managed;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Memory && Equals((Memory) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (WorkingSetPrivate.GetHashCode()*397) ^ Managed.GetHashCode();
            }
        }

        public static bool operator ==(Memory left, Memory right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Memory left, Memory right)
        {
            return !Equals(left, right);
        }

        public Memory(decimal workingSetPrivate, decimal managed)
        {
            WorkingSetPrivate = workingSetPrivate;
            Managed = managed;
        }

        public decimal WorkingSetPrivate { get; private set; }

        public decimal Managed { get; private set; }
    }
}
