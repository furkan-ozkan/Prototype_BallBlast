namespace HappyEvents
{
    [System.Serializable]
    public struct EventLog
    {
        public string EventName;
        public object Payload;
        public string Timestamp;
        public string StackTrace;
        public double ExecutionTime;

        public EventLog(string name, object payload, string trace, double time)
        {
            EventName = name;
            Payload = payload;
            Timestamp = System.DateTime.Now.ToString("HH:mm:ss");
            StackTrace = trace;
            ExecutionTime = time;
        }
    }
}