namespace StockPrediction
{
    using System;
    using System.Linq;
    using Stock;

    public class Predictor : Server.StockData.StockPrediction
    {
        public void Predict(StockGroup stockGroup)
        {
            StockDataPoint[] dataPoints = stockGroup.DataPoints.ToArray();
            StockDataPoint[] predicted = new StockDataPoint[dataPoints.Length * 2];
            long currentTime = DateTime.Now.Ticks / 10000000;
            //(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;

            for (int i = 0; i < dataPoints.Length; i++)
            {
                predicted[i] = dataPoints[i];

                long newTimeStamp = 2 * currentTime - dataPoints[i].TimeStamp;
                StockDataPoint predictedPoint = new StockDataPoint(dataPoints[i].Value, newTimeStamp);
                predicted[predicted.Length - 1 - i] = predictedPoint;
            }

            stockGroup.DataPoints = predicted.ToList();
        }
    }
}
