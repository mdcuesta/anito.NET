using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Anito.Data;
using Anito.Data.SqlClient;
using Moq;
using NUnit.Framework;

namespace Anito.Test.SqlClient.Unit
{
    [TestFixture]
    public class CommandBuilderTests
    {
        [Test]
        public void CreateExecuteScalarCommandByExpression()
        {
            var provider = new Mock<IProvider>();

            var commandBuilder = new CommandBuilder(provider.Object);
        }
    }
}
