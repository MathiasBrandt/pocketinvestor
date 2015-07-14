
namespace StockData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class StockDataPoint
    {
        private uint value;

        private long timeStamp;

        public StockDataPoint(uint value, long timeStamp)
        {
            this.value = value;
            this.timeStamp = timeStamp;
        }

        public uint Value
        {
            get
            {
                return value;
            }
        }

        public long TimeStamp
        {
            get
            {
                return timeStamp;
            }
        }
    }
}
