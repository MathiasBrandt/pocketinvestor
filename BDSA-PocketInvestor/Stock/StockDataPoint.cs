
namespace Stock
{
    /// <summary>
    /// Represents the price of a stock group at a specifiec point in time.
    /// </summary>
    public class StockDataPoint
    {
        #region Constants and Fields

        private long timeStamp;

        private uint value;

        #endregion

        #region Constructors and Destructors

        public StockDataPoint(uint value, long timeStamp)
        {
            this.value = value;
            this.timeStamp = timeStamp;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the time stamp of this stock data point.
        /// </summary>
        public long TimeStamp
        {
            get
            {
                return timeStamp;
            }
        }

        /// <summary>
        /// Gets the value of this stock data point.
        /// </summary>
        public uint Value
        {
            get
            {
                return value;
            }
        }

        #endregion
    }
}
