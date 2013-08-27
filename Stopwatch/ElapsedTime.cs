namespace Stopwatch
{
    class ElapsedTime
    {

        public short Milliseconds { get; private set; }
        public byte Seconds { get; private set; }
        public byte Minutes { get; private set; }
        public byte Hours { get; private set; }

        public void AddOneMillisecond()
        {
            if (Milliseconds != 999) {
                Milliseconds += 1;
            } else {

                if (Seconds != 59) {
                    Seconds += 1;
                } else {

                    if (Minutes != 59) {
                        Minutes += 1;
                    } else {

                        if (Hours != 99) {
                            Hours += 1;
                        } else {
                            Hours = 0;
                        }

                        Minutes = 0;
                    }

                    Seconds = 0;
                }

                Milliseconds = 0;
            }
        }

        public void Reset()
        {
            Milliseconds = 0;
            Seconds = 0;
            Minutes = 0;
            Hours = 0;
        }

        public override string ToString()
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3:000}", Hours, Minutes, Seconds, Milliseconds);
        }
        
    }
}
