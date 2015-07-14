namespace Server.UserData
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents private investment data for a single stock group.
    /// </summary>
    public class InvestmentData
    {
        #region Constants and Fields

        private uint investmentPercentage;

        private String stockGroup;

        #endregion

        #region Constructors and Destructors

        public InvestmentData(String stockGroup, uint investmentPercentage)
        {
            Contract.Requires(investmentPercentage <= 100);
            Contract.Requires(investmentPercentage >= 0);

            this.investmentPercentage = investmentPercentage;
            this.stockGroup = stockGroup;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the percentage invested in this stock group.
        /// </summary>
        public uint InvestmentPercentage
        {
            get
            {
                return investmentPercentage;
            }
            set
            {
                Contract.Requires(value <= 100);
                Contract.Ensures(value == InvestmentPercentage);
                investmentPercentage = value;
            }
        }

        /// <summary>
        /// Gets the name of the associated stock group.
        /// </summary>
        public string StockGroup
        {
            get
            {
                return stockGroup;
            }
        }

        #endregion

        #region Methods

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(InvestmentPercentage <= 100);
        }

        #endregion
    }
}
