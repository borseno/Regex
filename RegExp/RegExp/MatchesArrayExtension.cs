using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RegExp
{
    public static class MatchesArrayExtensions
    {
        //  <summary>
        //      Checks if the this array has all elements in the same order as in value array in the start of the this array.
        //  </summary>
        public static bool ContainsInStart(this Match[] container, Match[] value)
        {
            if (container == null || value == null)
                return false;
            if (container.Length == 0 || value.Length == 0)
                return false;
            if (container.Length < value.Length)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (!MatchesComparer.Equals(container[i], value[i]))
                    return false;
            }

            return true;
        }
    }
}
