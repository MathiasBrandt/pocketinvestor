// <copyright file="UserTest.cs" company="Hewlett-Packard">Copyright © Hewlett-Packard 2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.UserData;
using System.Collections.Generic;
using Server;

namespace Server.UserData
{
    [TestClass]
    [PexClass(typeof(User))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class UserTest
    {
        [PexMethod]
        public bool VerifyAndExtendLoginSession([PexAssumeUnderTest]User target)
        {
            bool result = target.VerifyAndExtendLoginSession();
            return result;
            // TODO: add assertions to method UserTest.VerifyAndExtendLoginSession(User)
        }
        [PexMethod]
        public void SaveData([PexAssumeUnderTest]User target, List<InvestmentData> data)
        {
            target.SaveData(data);
            // TODO: add assertions to method UserTest.SaveData(User, List`1<InvestmentData>)
        }
        [PexMethod]
        public string NameGet([PexAssumeUnderTest]User target)
        {
            string result = target.Name;
            return result;
            // TODO: add assertions to method UserTest.NameGet(User)
        }
        [PexMethod]
        public byte[] KeyFileGet([PexAssumeUnderTest]User target)
        {
            byte[] result = target.KeyFile;
            return result;
            // TODO: add assertions to method UserTest.KeyFileGet(User)
        }
        [PexMethod]
        public void IVSet([PexAssumeUnderTest]User target, byte[] value)
        {
            target.IV = value;
            // TODO: add assertions to method UserTest.IVSet(User, Byte[])
        }
        [PexMethod]
        public byte[] IVGet([PexAssumeUnderTest]User target)
        {
            byte[] result = target.IV;
            return result;
            // TODO: add assertions to method UserTest.IVGet(User)
        }
        [PexMethod]
        public List<InvestmentData> InvestmentDataGet([PexAssumeUnderTest]User target)
        {
            List<InvestmentData> result = target.InvestmentData;
            return result;
            // TODO: add assertions to method UserTest.InvestmentDataGet(User)
        }
        [PexMethod]
        public int IdGet([PexAssumeUnderTest]User target)
        {
            int result = target.Id;
            return result;
            // TODO: add assertions to method UserTest.IdGet(User)
        }
        [PexMethod]
        public decimal FundsGet([PexAssumeUnderTest]User target)
        {
            decimal result = target.Funds;
            return result;
            // TODO: add assertions to method UserTest.FundsGet(User)
        }
        [PexMethod]
        public DateTime ExpirationTimeGet([PexAssumeUnderTest]User target)
        {
            DateTime result = target.ExpirationTime;
            return result;
            // TODO: add assertions to method UserTest.ExpirationTimeGet(User)
        }
        [PexMethod]
        public User Constructor(
            string username,
            int id,
            decimal funds,
            List<InvestmentData> investments,
            byte[] keyFile,
            ServerClass server
        )
        {
            User target = new User(username, id, funds, investments, keyFile, server);
            return target;
            // TODO: add assertions to method UserTest.Constructor(String, Int32, Decimal, List`1<InvestmentData>, Byte[], ServerClass)
        }
        [PexMethod]
        public int BlockSizeGet([PexAssumeUnderTest]User target)
        {
            int result = target.BlockSize;
            return result;
            // TODO: add assertions to method UserTest.BlockSizeGet(User)
        }
    }
}
