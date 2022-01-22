namespace Dugan {

    /// <summary>
    /// A simple class for performing timed actions, as opposed to using coroutines.
    /// Once set up, just run timeout on an update loop while passing the delta time.
    /// When finished, runover will hold the difference between the end time and actual time.
    /// </summary>

    public class Timeout {
        
        /// <summary>
        /// Delagate for when the timer ends. Parameter is for the runover time.
        /// </summary>
        /// <param name="runover"></param>
        public delegate void Alarm ( float runover );
        /// <summary>
        /// A handy Alarm delegate for other objects to subscribe to the end of the alarm.
        /// </summary>
        public Alarm OnAlarm;


        public float timeoutTime = 0;
        public float currentTime = 0;
        public bool running { get; private set; }


        public float runover {
            get {
                return currentTime - timeoutTime;
            }
        }

        public float NormalizedTime {
            get {
                return currentTime / timeoutTime;
            }
            set {
                currentTime = value * timeoutTime;
            }
        }

        public Timeout ( float timeoutTime, bool started = false ) {
            this.timeoutTime = timeoutTime;
            running = started;
        }

        /// <summary>
        /// Begins the timeout from whatever position it was previously.
        /// </summary>
        public void Start () {
            running = true;
        }

        /// <summary>
        /// Stops the timeout from running.
        /// </summary>
        public void Pause () {
            running = false;
        }

        /// <summary>
        /// Stops the timeout from running and resets the time.
        /// </summary>
        public void Reset () {
            running = false;
            currentTime = 0;
        }

        /// <summary>
        /// Resets the timeout and starts it running.
        /// </summary>
        public void ReStart () {
            currentTime = 0;
            running = true;
        }

        /// <summary>
        /// Required to make the timeout work. Passing in the delta time is needed as well. Can pass scaled delta time too!
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public bool Tick ( float deltaTime ) {
            if (running) {
                currentTime += deltaTime;
                if (currentTime >= timeoutTime) {
                    OnAlarm?.Invoke(runover);
                    running = false;
                    return true;
                }
                return false;
            }
            return false;
        }

        public float GetNormalizedSlice(float startA, float endA) {
            float a = NormalizedTime;

            if (a < startA) {
                return 0;
            }
            if (a > endA) {
                return 1;
            }

            return (a - startA) / (endA - startA);
        }
    }
}