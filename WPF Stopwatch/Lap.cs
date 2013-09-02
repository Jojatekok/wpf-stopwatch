namespace WpfStopwatch
{
    public class Lap
    {
        public uint Id { get; private set; }
        public string TimeInterval { get; private set; }

        public Lap(uint id, string timeInterval)
        {
            Id = id;
            TimeInterval = timeInterval;
        }
    }
}
