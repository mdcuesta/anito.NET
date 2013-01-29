/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;

namespace Anito.Data.Extensions
{
    public static class QueryExtensions
    {
        #region Like
        public static bool Like(this string value, string likeExpression)
        {
            return SqlMethod.Like(value, likeExpression);
        }
        #endregion

        #region Between
        public static bool Between<T>(this T value, T from, T to)
            where T : IComparable
        {
            return SqlMethod.Between(value, from, to);
        }
        #endregion

        #region In
        public static bool In<T>(this T value, params T[] values)
        {
            return SqlMethod.In(value, values);
        }
        #endregion
    }
}
