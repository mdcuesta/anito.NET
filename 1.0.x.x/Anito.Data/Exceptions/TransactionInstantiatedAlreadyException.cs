using System;

namespace Anito.Data.Exceptions
{
    public class TransactionInstantiatedAlreadyException : Exception
    {
        private const string ERROR_MESSAGE = "A transaction is already initiated";

        public override string Message
        {
            get
            {
                return ERROR_MESSAGE;
            }
        }
    }
}
