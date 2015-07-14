namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SubmitTest
    {
        private Dictionary<String, int> malformed;

        [TestInitialize]
        public void Initialize()
        {
            malformed = new Dictionary<string, int>();
            Communication.Instance.StockGroups.ForEach(sg => malformed.Add(sg, Communication.Instance.Investments(sg)));
        }

        [TestCleanup]
        public void CleanUp()
        {
            malformed.Clear();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void OneToMany()
        {
            malformed.Add("not a stock group", 0);
            Communication.Instance.SubmitData(malformed);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void EmptyDictionary()
        {
            malformed.Clear();
            Communication.Instance.SubmitData(malformed);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void OneToFew()
        {
            malformed.Remove(Communication.Instance.StockGroups[0]);
            Communication.Instance.SubmitData(malformed);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SumMoreThan100()
        {
            Communication.Instance.StockGroups.ForEach(sg => malformed[sg] = (100 / Communication.Instance.StockGroups.Count) + 1);
            Communication.Instance.SubmitData(malformed);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SumLessThan0()
        {
            Communication.Instance.StockGroups.ForEach(sg => malformed[sg] = -1);
            Communication.Instance.SubmitData(malformed);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DuplicatedStockGroup()
        {
            malformed.Remove(Communication.Instance.StockGroups[1]);
            malformed.Add(Communication.Instance.StockGroups[0], 0);
            Communication.Instance.SubmitData(malformed);
        }
    }
}
