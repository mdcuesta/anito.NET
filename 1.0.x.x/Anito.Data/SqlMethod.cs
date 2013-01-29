/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;
using System.Linq;

namespace Anito.Data
{
    public sealed class SqlMethod
    {
        #region Like
        public static bool Like(string column, string likeExpression)
        {
            return default(bool);
        }
        #endregion

        #region Between
        public static bool Between<T>(T value, T from, T to)
            where T : IComparable
        {
            return value.CompareTo(from) == 1 && value.CompareTo(to) < 0;
        }
        #endregion

        #region In
        public static bool In<T>(T column, params T[] values)
        {
            if(Equals(values, null))
                throw new ArgumentNullException("values");

            var res = values.Count(x => Equals(x, column));

            return res > 0;
        }
        #endregion
    }
}
