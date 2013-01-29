using System;

namespace Anito.Data.Exceptions
{
    public class UnableToCreateCommandException : Exception
    {
        private const string MESSAGE_FORMAT = "Provider {0} is unable to create command";
        private readonly string m_message;

        public override string Message
        {
            get { return m_message; }
        }

        public UnableToCreateCommandException(string provider)
        {
            m_message = string.Format(MESSAGE_FORMAT, provider);
        }
    }
}
