using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.DataTypes
{
    partial class Fish_Types
    {
        public static bool operator ==(Fish_Types a, Fish_Types b)
        {

            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Name == b.Name;
        }

        public static bool operator !=(Fish_Types a, Fish_Types b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            if (Name == null) return 0;
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Fish_Types other = obj as Fish_Types;
            return other != null && other.Name == this.Name;
        }
    }
}
