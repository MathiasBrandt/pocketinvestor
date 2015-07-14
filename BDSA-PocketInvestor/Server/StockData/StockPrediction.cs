namespace Server.StockData
{
    using System.Diagnostics.Contracts;

    using Stock;

    /// <summary>
    /// Interface for plugins to predict stock group trends.
    /// </summary>
    [ContractClass(typeof(PredictionContracts))]
    public interface StockPrediction
    {
        /// <summary>
        /// Modifies the specified stock group to contain the predicted stock data points.
        /// </summary>
        /// <param name="stockGroup">The stockGroup</param>
        void Predict(StockGroup stockGroup);
    }
}
