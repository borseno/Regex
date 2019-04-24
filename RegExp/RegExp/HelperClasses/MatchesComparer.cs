using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RegExp.HelperClasses
{
    public static class MatchesComparer
    {
        public static bool Equals(IEnumerable<Match> m1, IEnumerable<Match> m2)
        {
            if (m1 == null || m2 == null)
                return false;

            var m1Arr = m1.ToArray();
            var m2Arr = m2.ToArray();

            if (m1Arr.Length == 0 || m2Arr.Length == 0)
                return false;

            if (m1Arr.Length != m2Arr.Length)
                return false;

            for (int i = 0; i < m1Arr.Length; i++)
            {
                if (!Equals(m1Arr[i], m2Arr[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Equals(Match m1, Match m2)
        {
            if (m1 == null || m2 == null)
                return false;

            return m1.Value == m2.Value
                   && m1.Length == m2.Length
                   && m1.Index == m2.Index
                   && m1.Success == m2.Success;
        }
    }
}
