namespace MessageBrocker.RPC.Brocker
{
    public class Subscriber
    {
        public string Topic { get; set; }

        public string Host { get; set; }

        public Subscriber(string topic, string host)
        {
            Topic = topic;
            Host = host;
        }
    }
}
