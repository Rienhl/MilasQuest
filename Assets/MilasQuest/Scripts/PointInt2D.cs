using System;

namespace MilasQuest.Grids
{
    //https://www.codeproject.com/Articles/812678/Performance-Considerations-of-Class-Design-and-Gen
    //and
    //https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode?redirectedfrom=MSDN&view=netcore-3.1#System_Object_GetHashCode
    //explain the need for overriding the Equals and GetHashCode methods when using structs.
    public struct PointInt2D : IEquatable<PointInt2D>
    {
        public int X;
        public int Y;

        public bool Equals(PointInt2D other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != this.GetType())
                return false;
            return this.Equals((PointInt2D)obj);
        }

        public override int GetHashCode()
        {
            return ShiftAndWrap(X.GetHashCode(), 2) ^ Y.GetHashCode();
        }

        private int ShiftAndWrap(int value, int positions)
        {
            positions = positions & 0x1F;
            uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
            uint wrapped = number >> (32 - positions);
            return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
        }

        /// <summary>
        /// This should be used for debugging purposes.
        /// If string representations of this struct are needed in tight loops
        /// or during each frame during gameplay,
        /// implement a version that reuses a StringBuilder instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "[" + X + ", " + Y + "]";
        }

        public static bool operator ==(PointInt2D a, PointInt2D b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(PointInt2D a, PointInt2D b)
        {
            return !a.Equals(b);
        }

        public static PointInt2D operator +(PointInt2D a, PointInt2D b)
        {
            return new PointInt2D() { X = a.X + b.X, Y = a.Y + b.Y };
        }
        
        public static PointInt2D operator -(PointInt2D a, PointInt2D b)
        {
            return new PointInt2D() { X = a.X - b.X, Y = a.Y - b.Y };
        }
    }
}