/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

using Anito.Data.Schema;
using System.Collections.Generic;

namespace Anito.Data
{
    internal interface ISessionContainer
    {
        ISession DataSession{ get; set; }

        //List<object> KeyValues { get; set; }
        TypeRelation Relation { get; set; }

        void AddReferenceValue(object value);
    }
}
