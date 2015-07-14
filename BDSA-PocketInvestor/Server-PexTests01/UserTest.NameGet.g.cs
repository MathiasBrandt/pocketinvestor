// <copyright file="UserTest.NameGet.g.cs" company="Hewlett-Packard">Copyright � Hewlett-Packard 2011</copyright>
// <auto-generated>
// This file contains automatically generated unit tests.
// Do NOT modify this file manually.
// 
// When Pex is invoked again,
// it might remove or update any previously generated unit tests.
// 
// If the contents of this file becomes outdated, e.g. if it does not
// compile anymore, you may delete this file and invoke Pex again.
// </auto-generated>
using System;
using System.Collections.Generic;
using Microsoft.Pex.Framework.Explorable;
using Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace Server.UserData
{
    public partial class UserTest
    {
[TestMethod]
[PexGeneratedBy(typeof(UserTest))]
public void NameGet878()
{
    List<InvestmentData> list;
    User user;
    string s;
    InvestmentData[] investmentDatas = new InvestmentData[0];
    list = new List<InvestmentData>((IEnumerable<InvestmentData>)investmentDatas);
    byte[] bs = new byte[0];
    byte[] bs1 = new byte[0];
    user = PexInvariant.CreateInstance<User>();
    PexInvariant.SetField<DateTime>((object)user, "expires", default(DateTime));
    PexInvariant.SetField<decimal>((object)user, "funds", default(decimal));
    PexInvariant.SetField<int>((object)user, "id", 0);
    PexInvariant.SetField<List<InvestmentData>>((object)user, "investments", list);
    PexInvariant.SetField<byte[]>((object)user, "iv", bs);
    PexInvariant.SetField<byte[]>((object)user, "keyFile", bs1);
    PexInvariant.SetField<ServerClass>((object)user, "server", (ServerClass)null);
    PexInvariant.SetField<string>((object)user, "username", "\u0100");
    PexInvariant.CheckInvariant((object)user);
    s = this.NameGet(user);
    Assert.AreEqual<string>("\u0100", s);
    Assert.IsNotNull((object)user);
    Assert.AreEqual<int>(1, user.ExpirationTime.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Monday, user.ExpirationTime.DayOfWeek);
    Assert.AreEqual<int>(1, user.ExpirationTime.DayOfYear);
    Assert.AreEqual<int>(0, user.ExpirationTime.Hour);
    Assert.AreEqual<DateTimeKind>
        (DateTimeKind.Unspecified, user.ExpirationTime.Kind);
    Assert.AreEqual<int>(0, user.ExpirationTime.Millisecond);
    Assert.AreEqual<int>(0, user.ExpirationTime.Minute);
    Assert.AreEqual<int>(1, user.ExpirationTime.Month);
    Assert.AreEqual<int>(0, user.ExpirationTime.Second);
    Assert.AreEqual<int>(1, user.ExpirationTime.Year);
    Assert.AreEqual<decimal>(default(decimal), user.Funds);
    Assert.IsNotNull(user.IV);
    Assert.AreEqual<int>(0, user.IV.Length);
    Assert.AreEqual<int>(0, user.Id);
    Assert.IsNotNull(user.InvestmentData);
    Assert.AreEqual<int>(0, user.InvestmentData.Capacity);
    Assert.AreEqual<int>(0, user.InvestmentData.Count);
    Assert.IsNotNull(user.KeyFile);
    Assert.AreEqual<int>(0, user.KeyFile.Length);
    Assert.AreEqual<string>("\u0100", user.Name);
}
    }
}
