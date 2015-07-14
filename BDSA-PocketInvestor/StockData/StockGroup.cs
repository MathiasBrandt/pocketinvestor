
namespace StockData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;

    public class StockGroup
    {
        private List<StockDataPoint> dataPoints;

        private String name;

        public StockGroup(string name, List<StockDataPoint> dataPoints)
        {
            this.dataPoints = dataPoints;
            this.name = name;
        }

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

        public string Name
        {
            get
            {
                return name;
            }
        }
    }
}
