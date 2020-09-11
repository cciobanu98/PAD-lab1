using System;

namespace MessageBrocker.Core.Messages
{
    [Serializable]
    public class Message
    {
        private string _topic;
        private string _data;

        public string Topic
        {
            get => _topic;
            set => _topic = value ?? throw new ArgumentNullException(nameof(_topic));
        }

        public string Data
        {
            get => _data;
            set => _data = value ?? throw new ArgumentNullException(nameof(_data));
        }


        public Message(string topic, string data)
        {
            _topic = topic ?? throw new ArgumentNullException(nameof(topic));
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public Message()
        {

        }

        public override string ToString()
        {
            return $"Topic: {Topic}. Data: {Data}";
        }
    }
}
