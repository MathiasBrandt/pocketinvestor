// <copyright file="UserTest.SaveData.g.cs" company="Hewlett-Packard">Copyright � Hewlett-Packard 2011</copyright>
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
using Microsoft.Pex.Engine.Exceptions;
using Server.UserData.Moles;
using Microsoft.Moles.Framework.Behaviors;

namespace Server.UserData
{
    public partial class UserTest
    {
[TestMethod]
[PexGeneratedBy(typeof(UserTest))]
[PexRaisedException(typeof(NullReferenceException))]
public void SaveDataThrowsNullReferenceException772()
{
    SInvestmentData sInvestmentData;
    List<InvestmentData> list;
    User user;
    sInvestmentData = PexInvariant.CreateInstance<SInvestmentData>();
    PexInvariant.SetField<uint>((object)sInvestmentData, "investmentPercentage", 0u);
    PexInvariant.SetField<string>((object)sInvestmentData, "stockGroup", "");
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
    this.SaveData(user, list);
}
[TestMethod]
[PexGeneratedBy(typeof(UserTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void SaveDataThrowsContractException805()
{
    try
    {
      List<InvestmentData> list;
      User user;
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
      this.SaveData(user, (List<InvestmentData>)null);
      throw 
        new AssertFailedException("expected an exception of type ContractException");
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
[TestMethod]
[PexGeneratedBy(typeof(UserTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void SaveDataThrowsContractException686()
{
    try
    {
      InvestmentData investmentData;
      List<InvestmentData> list;
      User user;
      investmentData = PexInvariant.CreateInstance<InvestmentData>();
      PexInvariant.SetField<uint>
          ((object)investmentData, "investmentPercentage", 0u);
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
      this.SaveData(user, (List<InvestmentData>)null);
      throw 
        new AssertFailedException("expected an exception of type ContractException");
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
[TestMethod]
[PexGeneratedBy(typeof(UserTest))]
[PexRaisedException(typeof(NullReferenceException))]
public void SaveDataThrowsNullReferenceException721()
{
    InvestmentData investmentData;
    List<InvestmentData> list;
    User user;
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
    this.SaveData(user, list);
}
[TestMethod]
[PexGeneratedBy(typeof(UserTest))]
[PexRaisedException(typeof(NullReferenceException))]
public void SaveDataThrowsNullReferenceException230()
{
    InvestmentData investmentData;
    List<InvestmentData> list;
    User user;
    investmentData = PexInvariant.CreateInstance<InvestmentData>();
    PexInvariant.SetField<uint>((object)investmentData, "investmentPercentage", 0u);
    PexInvariant.SetField<string>((object)investmentData, "stockGroup", "");
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
    this.SaveData(user, list);
}
[TestMethod]
[PexGeneratedBy(typeof(UserTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void SaveDataThrowsContractException986()
{
    try
    {
      InvestmentData investmentData;
      List<InvestmentData> list;
      User user;
      investmentData = PexInvariant.CreateInstance<InvestmentData>();
      PexInvariant.SetField<uint>
          ((object)investmentData, "investmentPercentage", 0u);
      PexInvariant.SetField<string>((object)investmentData, "stockGroup", "");
      PexInvariant.CheckInvariant((object)investmentData);
      InvestmentData[] investmentDatas = new InvestmentData[2];
      investmentDatas[0] = investmentData;
      investmentDatas[1] = investmentData;
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
      this.SaveData(user, list);
      throw 
        new AssertFailedException("expected an exception of type ContractException");
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
[TestMethod]
[PexGeneratedBy(typeof(UserTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void SaveDataThrowsContractException561()
{
    try
    {
      SInvestmentData sInvestmentData;
      List<InvestmentData> list;
      User user;
      sInvestmentData = PexInvariant.CreateInstance<SInvestmentData>();
      PexInvariant.SetField<uint>
          ((object)sInvestmentData, "investmentPercentage", 0u);
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
      this.SaveData(user, (List<InvestmentData>)null);
      throw 
        new AssertFailedException("expected an exception of type ContractException");
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
[TestMethod]
[PexGeneratedBy(typeof(UserTest))]
[PexRaisedContractException(PexExceptionState.Expected)]
public void SaveDataThrowsContractException192()
{
    try
    {
      InvestmentData investmentData;
      List<InvestmentData> list;
      User user;
      SInvestmentData sInvestmentData;
      List<InvestmentData> list1;
      investmentData = PexInvariant.CreateInstance<InvestmentData>();
      PexInvariant.SetField<uint>
          ((object)investmentData, "investmentPercentage", 0u);
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
      sInvestmentData = PexInvariant.CreateInstance<SInvestmentData>();
      PexInvariant.SetField<uint>
          ((object)sInvestmentData, "investmentPercentage", 0u);
      PexInvariant.SetField<string>((object)sInvestmentData, "stockGroup", "");
      PexInvariant.SetField<bool>((object)sInvestmentData, "__callBase", false);
      PexInvariant.SetField<IBehavior>
          ((object)sInvestmentData, "__instanceBehavior", (IBehavior)null);
      PexInvariant.CheckInvariant((object)sInvestmentData);
      InvestmentData[] investmentDatas1 = new InvestmentData[1];
      investmentDatas1[0] = (InvestmentData)sInvestmentData;
      list1 =
        new List<InvestmentData>((IEnumerable<InvestmentData>)investmentDatas1);
      this.SaveData(user, list1);
      throw 
        new AssertFailedException("expected an exception of type ContractException");
    }
    catch(Exception ex)
    {
      if (!PexContract.IsContractException(ex))
        throw ex;
    }
}
    }
}
