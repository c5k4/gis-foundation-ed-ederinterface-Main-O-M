namespace Telvent.PGE.ED.Desktop.GDBM.CircuitProcessing
{
    public class CircuitSourceData
    {
        public string FeederId
        { get; private set; }

        public string CircuitColor
        { get; private set; }

        public string FeederType
        { get; private set; }

        public CircuitSourceData(string feederId, string circuitColor, string feederType)
        {
            FeederId = feederId;
            CircuitColor = circuitColor;
            FeederType = feederType;
        }
    }
}
