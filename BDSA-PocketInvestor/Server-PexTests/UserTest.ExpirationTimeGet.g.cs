// <copyright file="UserTest.ExpirationTimeGet.g.cs" company="Hewlett-Packard">Copyright � Hewlett-Packard 2011</copyright>
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
using Server.UserData.Moles;
using Microsoft.Moles.Framework.Behaviors;

namespace Server.UserData
{
    public partial class UserTest
    {
[TestMethod]
[PexGeneratedBy(typeof(UserTest))]
public void ExpirationTimeGet733()
{
    InvestmentData investmentData;
    List<InvestmentData> list;
    User user;
    DateTime dt;
    investmentData = PexInvariant.CreateInstance<InvestmentData>();
    PexInvariant.SetField<uint>((object)investmentData, "investmentPercentage", 0u);
    PexInvariant.SetField<string>
        ((object)investmentData, "stockGroup", (string)null);
    PexInvariant.CheckInvariant((object)investmentData);
    InvestmentData[] investmentDatas = new InvestmentData[1];
    investmentDatas[0] = investmentData;
    list = new List<InvestmentData>((IEnumerable<InvestmentData>)investmentDatas);
    user = PexInvariant.CreateInstance<User>();
    PexInvariant.SetField<int>((object)user, "blockSize", 0);
    PexInvariant.SetField<DateTime>((object)user, "expires", default(DateTime));
    PexInvariant.SetField<decimal>((object)user, "funds", default(decimal));
    PexInvariant.SetField<int>((object)user, "id", 0);
    PexInvariant.SetField<List<InvestmentData>>((object)user, "investments", list);
    PexInvariant.SetField<byte[]>((object)user, "iv", (byte[])null);
    PexInvariant.SetField<byte[]>((object)user, "keyFile", (byte[])null);
    PexInvariant.SetField<ServerClass>((object)user, "server", (ServerClass)null);
    PexInvariant.SetField<string>((object)user, "username", (string)null);
    PexInvariant.CheckInvariant((object)user);
    dt = this.ExpirationTimeGet(user);
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Monday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Unspecified, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1, dt.Year);
    Assert.IsNotNull((object)user);
    Assert.AreEqual<int>(0, user.BlockSize);
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
    Assert.IsNull(user.IV);
    Assert.AreEqual<int>(0, user.Id);
    Assert.IsNotNull(user.InvestmentData);
    Assert.AreEqual<int>(1, user.InvestmentData.Capacity);
    Assert.AreEqual<int>(1, user.InvestmentData.Count);
    Assert.IsNull(user.KeyFile);
    Assert.AreEqual<string>((string)null, user.Name);
}
[TestMethod]
[PexGeneratedBy(typeof(UserTest))]
public void ExpirationTimeGet527()
{
    List<InvestmentData> list;
    User user;
    DateTime dt;
    InvestmentData[] investmentDatas = new InvestmentData[0];
    list = new List<InvestmentData>((IEnumerable<InvestmentData>)investmentDatas);
    user = PexInvariant.CreateInstance<User>();
    PexInvariant.SetField<int>((object)user, "blockSize", 0);
    PexInvariant.SetField<DateTime>((object)user, "expires", default(DateTime));
    PexInvariant.SetField<decimal>((object)user, "funds", default(decimal));
    PexInvariant.SetField<int>((object)user, "id", 0);
    PexInvariant.SetField<List<InvestmentData>>((object)user, "investments", list);
    PexInvariant.SetField<byte[]>((object)user, "iv", (byte[])null);
    PexInvariant.SetField<byte[]>((object)user, "keyFile", (byte[])null);
    PexInvariant.SetField<ServerClass>((object)user, "server", (ServerClass)null);
    PexInvariant.SetField<string>((object)user, "username", (string)null);
    PexInvariant.CheckInvariant((object)user);
    dt = this.ExpirationTimeGet(user);
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Monday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Unspecified, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1, dt.Year);
    Assert.IsNotNull((object)user);
    Assert.AreEqual<int>(0, user.BlockSize);
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
    Assert.IsNull(user.IV);
    Assert.AreEqual<int>(0, user.Id);
    Assert.IsNotNull(user.InvestmentData);
    Assert.AreEqual<int>(0, user.InvestmentData.Capacity);
    Assert.AreEqual<int>(0, user.InvestmentData.Count);
    Assert.IsNull(user.KeyFile);
    Assert.AreEqual<string>((string)null, user.Name);
}
[TestMethod]
[PexGeneratedBy(typeof(UserTest))]
public void ExpirationTimeGet743()
{
    SInvestmentData sInvestmentData;
    List<InvestmentData> list;
    User user;
    DateTime dt;
    sInvestmentData = PexInvariant.CreateInstance<SInvestmentData>();
    PexInvariant.SetField<uint>((object)sInvestmentData, "investmentPercentage", 0u);
    PexInvariant.SetField<string>
        ((object)sInvestmentData, "stockGroup", (string)null);
    PexInvariant.SetField<bool>((object)sInvestmentData, "__callBase", false);
    PexInvariant.SetField<IBehavior>
        ((object)sInvestmentData, "__instanceBehavior", (IBehavior)null);
    PexInvariant.CheckInvariant((object)sInvestmentData);
    InvestmentData[] investmentDatas = new InvestmentData[1];
    investmentDatas[0] = (InvestmentData)sInvestmentData;
    list = new List<InvestmentData>((IEnumerable<InvestmentData>)investmentDatas);
    user = PexInvariant.CreateInstance<User>();
    PexInvariant.SetField<int>((object)user, "blockSize", 0);
    PexInvariant.SetField<DateTime>((object)user, "expires", default(DateTime));
    PexInvariant.SetField<decimal>((object)user, "funds", default(decimal));
    PexInvariant.SetField<int>((object)user, "id", 0);
    PexInvariant.SetField<List<InvestmentData>>((object)user, "investments", list);
    PexInvariant.SetField<byte[]>((object)user, "iv", (byte[])null);
    PexInvariant.SetField<byte[]>((object)user, "keyFile", (byte[])null);
    PexInvariant.SetField<ServerClass>((object)user, "server", (ServerClass)null);
    PexInvariant.SetField<string>((object)user, "username", (string)null);
    PexInvariant.CheckInvariant((object)user);
    dt = this.ExpirationTimeGet(user);
    Assert.AreEqual<int>(1, dt.Day);
    Assert.AreEqual<DayOfWeek>(DayOfWeek.Monday, dt.DayOfWeek);
    Assert.AreEqual<int>(1, dt.DayOfYear);
    Assert.AreEqual<int>(0, dt.Hour);
    Assert.AreEqual<DateTimeKind>(DateTimeKind.Unspecified, dt.Kind);
    Assert.AreEqual<int>(0, dt.Millisecond);
    Assert.AreEqual<int>(0, dt.Minute);
    Assert.AreEqual<int>(1, dt.Month);
    Assert.AreEqual<int>(0, dt.Second);
    Assert.AreEqual<int>(1, dt.Year);
    Assert.IsNotNull((object)user);
    Assert.AreEqual<int>(0, user.BlockSize);
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
    Assert.IsNull(user.IV);
    Assert.AreEqual<int>(0, user.Id);
    Assert.IsNotNull(user.InvestmentData);
    Assert.AreEqual<int>(1, user.InvestmentData.Capacity);
    Assert.AreEqual<int>(1, user.InvestmentData.Count);
    Assert.IsNull(user.KeyFile);
    Assert.AreEqual<string>((string)null, user.Name);
}
    }
}
