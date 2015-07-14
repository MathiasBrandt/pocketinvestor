
namespace Server.UserData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Security.Cryptography;

    /// <summary>
    /// Represents a single user session. Not directly related to the actual user data.
    /// </summary>
    public class User
    {
        #region Constants and Fields

        private DateTime expires;

        private decimal funds;

        private int id;

        private List<InvestmentData> investments;

        // aes initialization vector.
        private byte[] iv;

        private byte[] keyFile;

        private ServerClass server;

        private String username;

        #endregion

        #region Constructors and Destructors

        public User(String username, int id, decimal funds, List<InvestmentData> investments, byte[] keyFile, byte[] iv, ServerClass server)
        {
            Contract.Requires(!ReferenceEquals(server, null));
            Contract.Requires(iv.Length * 8 == 128); // Aes block size is always 128.
            Contract.Ensures(iv.Equals(IV));

            this.username = username;
            this.id = id;
            this.funds = funds;
            this.investments = investments;
            this.keyFile = keyFile;
            this.iv = iv;
            this.server = server;

            expires = DateTime.Now.AddHours(1);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the expiration time for this user session.
        /// </summary>
        public DateTime ExpirationTime
        {
            get
            {
                return expires;
            }
        }

        /// <summary>
        /// Gets the user's investment funds.
        /// </summary>
        public decimal Funds
        {
            get { return funds; }
        }

        /// <summary>
        /// Gets the user's initialization vector.
        /// </summary>
        public byte[] IV
        {
            get { return iv; }
        }

        /// <summary>
        /// Gets the user's id.
        /// </summary>
        public int Id
        {
            get
            {
                return id;
            }
        }

        /// <summary>
        /// Gets the user'r private inestment data.
        /// </summary>
        public List<InvestmentData> InvestmentData
        {
            get { return investments; }
        }

        /// <summary>
        /// Gets the user's key file.
        /// </summary>
        public byte[] KeyFile
        {
            get { return keyFile; }
        }

        /// <summary>
        /// Gets the user's name.
        /// </summary>
        public String Name
        {
            get { return username; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Saves the specified investment data.
        /// </summary>
        /// <param name="data">The data to save.</param>
        public void SaveData(List<InvestmentData> data)
        {
            Contract.Requires(!ReferenceEquals(data, null));
            Contract.Requires(data.Count > 0);
            Contract.Requires(data.Count == InvestmentData.Count);
            Contract.Requires(Contract.ForAll(data, id => !ReferenceEquals(id, null)));
            Contract.Requires(Contract.ForAll(InvestmentData, id => !ReferenceEquals(id.StockGroup, null)));
            Contract.Requires(Contract.ForAll(InvestmentData, id =>
                data.Where(d => d.StockGroup.Equals(id.StockGroup)).Count() == 1));
            Contract.Requires(data.Sum(d => d.InvestmentPercentage) <= 100);
            Contract.Ensures(Contract.ForAll(InvestmentData, id =>
                data.Where(d => d.StockGroup.Equals(id.StockGroup) &&
                    d.InvestmentPercentage == id.InvestmentPercentage).Count() == 1));
            
            investments.ForEach(i => i.InvestmentPercentage = data.First(
                compare => compare.StockGroup.Equals(i.StockGroup)).InvestmentPercentage);

            server.SaveUserInvestmentData(this);
        }

        /// <summary>
        /// Extends the login session with five minutes if it hasn't allready expired.
        /// </summary>
        /// <returns>True if it hasn't expired.</returns>
        public bool VerifyAndExtendLoginSession()
        {
            if (expires.CompareTo(DateTime.Now) < 0) return false;
            if (expires.CompareTo(DateTime.Now.AddMinutes(-5)) < 0)
                expires = DateTime.Now.AddMinutes(5);
            return true;
        }

        #endregion

        #region Methods

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(InvestmentData.Sum(d => d.InvestmentPercentage) <= 100);
            Contract.Invariant(!String.IsNullOrWhiteSpace(Name));
            Contract.Invariant(!ReferenceEquals(InvestmentData, null));
            Contract.Invariant(!ReferenceEquals(KeyFile, null));
            Contract.Invariant(!ReferenceEquals(IV, null));
        }

        #endregion
    }
}
