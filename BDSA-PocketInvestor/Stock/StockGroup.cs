
namespace Stock
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a single stock group.
    /// </summary>
    public class StockGroup
    {
        #region Constants and Fields

        private List<StockDataPoint> dataPoints;

        private String name;

        #endregion

        #region Constructors and Destructors

        public StockGroup(string name, List<StockDataPoint> dataPoints)
        {
            this.dataPoints = dataPoints;
            this.name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the list of the data points in this stockGroup.
        /// </summary>
        public List<StockDataPoint> DataPoints
        {
            get
            {
                return dataPoints;
            }
            set
            {
                Contract.Requires(!ReferenceEquals(value, null));
                Contract.Ensures(ReferenceEquals(value, DataPoints));
                dataPoints = value;
            }
        }

        /// <summary>
        /// Gets the name of this stock group.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        #endregion
    }
}
