/////////////////////////////////////////////////////////////////////////////////////////////////////
// Original Code by : Michael Dela Cuesta (michael.dcuesta@gmail.com)                              //
// Source Code Available : http://anito.codeplex.com                                               //
//                                                                                                 // 
// This source code is made available under the terms of the Microsoft Public License (MS-PL)      // 
///////////////////////////////////////////////////////////////////////////////////////////////////// 

using System.Data.Common;

namespace Anito.Data
{
    public delegate T ToTDelegate<out T>(DbDataReader reader);
    public delegate object ToObjectDelegate(DbDataReader reader);

    public delegate T ToDataObjectDelegate<out T>(DbDataReader reader, ISession session);
    public delegate IDataObject ToDataObjectDelegate(DbDataReader reader, ISession session);

    internal delegate object CreateISessionContainerDelegate(ISession session);
    internal delegate IProvider CreateIProviderDelegate();
}
