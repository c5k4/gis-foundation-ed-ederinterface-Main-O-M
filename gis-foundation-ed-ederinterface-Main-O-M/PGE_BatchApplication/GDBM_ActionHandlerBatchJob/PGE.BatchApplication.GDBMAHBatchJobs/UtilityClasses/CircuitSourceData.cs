namespace PGE.Desktop.EDER.GDBM.CircuitProcessing
{
    public class CircuitSourceData
    {
        public string FeederId
        { get; private set; }

        public string CircuitColor
        { get; private set; }

        public string FeederType
        { get; private set; }

        public string Substation
        { get; private set; }

        public CircuitSourceData(string feederId, string circuitColor, string feederType, string substation)
        {
            FeederId = feederId;
            CircuitColor = circuitColor;
            FeederType = feederType;
            Substation = substation; //S2NN: BugFix for CIRCUITNAME AU
        }
    }
}
