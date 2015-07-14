// <copyright file="InvestmentDataTest.cs" company="Hewlett-Packard">Copyright © Hewlett-Packard 2011</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.UserData;

namespace Server.UserData
{
    [TestClass]
    [PexClass(typeof(InvestmentData))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class InvestmentDataTest
    {
        [PexMethod]
        public string StockGroupGet([PexAssumeUnderTest]InvestmentData target)
        {
            string result = target.StockGroup;
            return result;
            // TODO: add assertions to method InvestmentDataTest.StockGroupGet(InvestmentData)
        }
        [PexMethod]
        public void InvestmentPercentageSet([PexAssumeUnderTest]InvestmentData target, uint value)
        {
            target.InvestmentPercentage = value;
            // TODO: add assertions to method InvestmentDataTest.InvestmentPercentageSet(InvestmentData, UInt32)
        }
        [PexMethod]
        public uint InvestmentPercentageGet([PexAssumeUnderTest]InvestmentData target)
        {
            uint result = target.InvestmentPercentage;
            return result;
            // TODO: add assertions to method InvestmentDataTest.InvestmentPercentageGet(InvestmentData)
        }
        [PexMethod]
        public InvestmentData Constructor(string stockGroup, uint investmentPercentage)
        {
            InvestmentData target = new InvestmentData(stockGroup, investmentPercentage);
            return target;
            // TODO: add assertions to method InvestmentDataTest.Constructor(String, UInt32)
        }
    }
}
