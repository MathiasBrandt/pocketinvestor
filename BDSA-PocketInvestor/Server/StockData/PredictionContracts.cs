namespace Server.StockData
{
    using System.Diagnostics.Contracts;

    using Stock;

    [ContractClassFor(typeof(StockPrediction))]
    abstract class PredictionContracts : StockPrediction
    {
        #region Public Methods

        public void Predict(StockGroup stockGroup)
        {
            Contract.Requires(!ReferenceEquals(stockGroup, null));
            Contract.Requires(stockGroup.DataPoints.Count > 0);
        }

        #endregion
    }
}
