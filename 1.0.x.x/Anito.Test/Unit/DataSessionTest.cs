using Anito.Data;
using Anito.Data.Exceptions;
using Anito.Data.Schema;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;
using Anito.Test.Mocks;
using Anito.Test.Entities;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using DataSession = Anito.Data.DataSession;
using ICommand = Anito.Test.Mocks.ICommand;
using IProvider = Anito.Test.Mocks.IProvider;
using TestSession = Anito.Test.Mocks.DataSession;
using Procedure = Anito.Data.Procedure;

namespace Anito.Test.Unit
{
    [TestFixture]
    public class DataSessionTest
    {
        #region GetT
        [Test]
        public void GetTByExpression()
        {

            Expression<Func<Customer, bool>> expression = e => e.ID == 1;

            var customer = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };

            var reader = new DbDataReader();
            reader.Setup(r => r.Read()).Returns(true);

            var provider = new IProvider();

            var command = new ICommand();

            var schema = new TypeTable(typeof(Customer));

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);

            provider.CommandBuilder.Setup(c => c.CreateGetTCommand<Customer>(expression)).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteReader(command.Object)).Returns(reader.Object);

            provider.Mapper.Setup(m => m.GetTMappingMethod<Customer>()).Returns(d => customer);

            var target = new DataSession(provider.Object);

            var actual = target.GetT(expression);

            Assert.AreEqual(actual, customer);
        }

        [Test]
        public void GetTByExpressionThrowsArgumentNullException()
        {
            try
            {
                var provider = new IProvider();
                var session = new DataSession(provider.Object);

                Expression expression = null;
                session.GetT<Customer>(expression);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentNullException);
            }
        }

        [Test]
        public void GetTByExpressionThrowsUnableToCreateCommandException()
        {
            Expression<Func<Customer, bool>> expression = e => e.ID == 1;
            var provider = new IProvider();
            provider.CommandBuilder.Setup(c => c.CreateGetTCommand<Customer>(expression)).Returns(default(Data.ICommand));

            try
            {
                var target = new DataSession(provider.Object);
                target.GetT(expression);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UnableToCreateCommandException);
            }
        }

        [Test]
        public void GetTByKey()
        {

            var customer = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };

            var reader = new DbDataReader();
            reader.Setup(r => r.Read()).Returns(true);

            var provider = new IProvider();

            var command = new ICommand();

            var schema = new TypeTable(typeof(Customer));

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);

            provider.CommandBuilder.Setup(c => c.CreateGetObjectByKeyCommand<Customer>()).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteReader(command.Object)).Returns(reader.Object);

            provider.Mapper.Setup(m => m.GetTMappingMethod<Customer>()).Returns(d => customer);

            var target = new DataSession(provider.Object);

            var actual = target.GetT<Customer>("CUS0000001");

            Assert.AreEqual(actual, customer);
        }

        [Test]
        public void GetTByKeyThrowsArgumentNullException()
        {
            try
            {
                var provider = new IProvider();
                var session = new DataSession(provider.Object);
                object[] keyValues = null;
                session.GetT<Customer>(keyValues);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentNullException);
            }
        }

        [Test]
        public void GetTByKeyThrowsUnableToCreateCommandException()
        {
            var provider = new IProvider();
            provider.CommandBuilder.Setup(c => c.CreateGetObjectByKeyCommand<Customer>()).Returns(default(Data.ICommand));

            try
            {
                var target = new DataSession(provider.Object);
                target.GetT<Customer>("CUS00001");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UnableToCreateCommandException);
            }
        }

        [Test]
        public void GetTByProcedure()
        {
            var procedure = new Mock<Procedure>("TestProcedure");

            var customer = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };

            var reader = new DbDataReader();
            reader.Setup(r => r.Read()).Returns(true);

            var provider = new IProvider();

            var command = new ICommand();

            var schema = new TypeTable(typeof(Customer));

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);

            provider.CommandBuilder.Setup(c => c.CreateCommandFromProcedure(procedure.Object)).Returns(command.Object);

            provider.CommandExecutor.Setup(c => c.ExecuteReader(command.Object)).Returns(reader.Object);

            provider.Mapper.Setup(m => m.GetTMappingMethod<Customer>()).Returns(d => customer);

            var target = new DataSession(provider.Object);

            var actual = target.GetT<Customer>(procedure.Object);

            Assert.AreEqual(actual, customer);

        }

        [Test]
        public void GetTByProcedureThrowsArgumentNullException()
        {
            try
            {
                var provider = new IProvider();
                var session = new DataSession(provider.Object);

                Procedure procedure = null;
                session.GetT<Customer>(procedure);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentNullException);
            }
        }

        [Test]
        public void GetTByProcedureThrowsUnableToCreateCommandException()
        {
            var provider = new IProvider();
            var procedure = new Procedure("TestProcedure");
            provider.CommandBuilder.Setup(c => c.CreateCommandFromProcedure(procedure)).Returns(default(Data.ICommand));

            try
            {
                var session = new DataSession(provider.Object);
                session.GetT<Customer>(procedure);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UnableToCreateCommandException);
            }
        }
        #endregion

        #region GetList

        [Test]
        public void GetListByKey()
        {
            var customer1 = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };
            var customer2 = new Customer
            {
                ID = 2,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 2,
                Name = "Customer2",
                ProfileID = 2
            };
            var customer3 = new Customer
            {
                ID = 3,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 3,
                Name = "Customer3",
                ProfileID = 3
            };

            var customerQueue = new Queue<Customer>(new[] { customer1, customer2, customer3 });
            var readerResults = new Queue<bool>(new[] { true, true, true, false });

            var reader = new DbDataReader();
            reader.Setup(r => r.Read()).Returns(readerResults.Dequeue);

            var provider = new IProvider();

            var command = new ICommand();

            var schema = new TypeTable(typeof(Customer));

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);

            provider.CommandBuilder.Setup(c => c.CreateGetObjectByKeyCommand<Customer>()).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteReader(command.Object)).Returns(reader.Object);

            provider.Mapper.Setup(m => m.GetTMappingMethod<Customer>()).Returns(d => customerQueue.Dequeue());

            var target = new DataSession(provider.Object);

            var actual = target.GetList<List<Customer>, Customer>("Test");

            Assert.IsTrue(actual[0].ID == 1 &&
                actual[1].ID == 2 && actual[2].ID == 3);
        }

        [Test]
        public void GetListByKeyThrowsArgumentNullException()
        {
            try
            {
                var provider = new IProvider();
                var session = new DataSession(provider.Object);

                object[] keys = null;
                session.GetList<List<Customer>, Customer>(keys);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentNullException);
            }
        }

        [Test]
        public void GetListByKeyThrowsUnableToCreateCommandException()
        {
            var provider = new IProvider();
            provider.CommandBuilder.Setup(c => c.CreateGetObjectByKeyCommand<Customer>()).Returns(default(Data.ICommand));

            try
            {
                var target = new DataSession(provider.Object);
                target.GetList<List<Customer>, Customer>("Key");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UnableToCreateCommandException);
            }
        }

        [Test]
        public void GetListByExpression()
        {
            Expression<Func<Customer, bool>> expression = e => e.ID < 4;

            var customer1 = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };
            var customer2 = new Customer
            {
                ID = 2,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 2,
                Name = "Customer2",
                ProfileID = 2
            };
            var customer3 = new Customer
            {
                ID = 3,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 3,
                Name = "Customer3",
                ProfileID = 3
            };

            var customerQueue = new Queue<Customer>(new[] { customer1, customer2, customer3 });
            var readerResults = new Queue<bool>(new[] { true, true, true, false });

            var reader = new DbDataReader();
            reader.Setup(r => r.Read()).Returns(readerResults.Dequeue);

            var provider = new IProvider();

            var command = new ICommand();

            var schema = new TypeTable(typeof(Customer));

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);

            provider.CommandBuilder.Setup(c => c.CreateGetListCommand<Customer>(expression)).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteReader(command.Object)).Returns(reader.Object);

            provider.Mapper.Setup(m => m.GetTMappingMethod<Customer>()).Returns(d => customerQueue.Dequeue());

            var target = new DataSession(provider.Object);

            var actual = target.GetList<List<Customer>, Customer>(expression);

            Assert.IsTrue(actual[0].ID == 1 &&
                actual[1].ID == 2 && actual[2].ID == 3);
        }

        [Test]
        public void GetListByExpressionThrowsArgumentNullException()
        {
            try
            {
                var provider = new IProvider();
                var session = new DataSession(provider.Object);

                Expression<Customer> expression = null;
                session.GetList<List<Customer>, Customer>(expression);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentNullException);
            }
        }

        [Test]
        public void GetListByExpressionThrowsUnableToCreateCommandException()
        {
            Expression<Func<Customer, bool>> expression = e => e.ID < 4;
            var provider = new IProvider();
            provider.CommandBuilder.Setup(c => c.CreateGetListCommand<Customer>(expression)).Returns(default(Data.ICommand));

            try
            {
                var target = new DataSession(provider.Object);
                target.GetList<List<Customer>, Customer>(expression);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UnableToCreateCommandException);
            }
        }

        [Test]
        public void GetList()
        {
            var customer1 = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };
            var customer2 = new Customer
            {
                ID = 2,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 2,
                Name = "Customer2",
                ProfileID = 2
            };
            var customer3 = new Customer
            {
                ID = 3,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 3,
                Name = "Customer3",
                ProfileID = 3
            };

            var customerQueue = new Queue<Customer>(new[] { customer1, customer2, customer3 });
            var readerResults = new Queue<bool>(new[] { true, true, true, false });

            var reader = new DbDataReader();
            reader.Setup(r => r.Read()).Returns(readerResults.Dequeue);

            var provider = new IProvider();

            var command = new ICommand();

            var schema = new TypeTable(typeof(Customer));

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);

            provider.CommandBuilder.Setup(c => c.CreateGetListCommand<Customer>()).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteReader(command.Object)).Returns(reader.Object);

            provider.Mapper.Setup(m => m.GetTMappingMethod<Customer>()).Returns(d => customerQueue.Dequeue());

            var target = new DataSession(provider.Object);

            var actual = target.GetList<List<Customer>, Customer>();

            Assert.IsTrue(actual[0].ID == 1 &&
                actual[1].ID == 2 && actual[2].ID == 3);
        }

        [Test]
        public void GetListThrowsUnableToCreateCommandException()
        {
            var provider = new IProvider();
            provider.CommandBuilder.Setup(c => c.CreateGetListCommand<Customer>()).Returns(default(Data.ICommand));

            try
            {
                var target = new DataSession(provider.Object);
                target.GetList<List<Customer>, Customer>();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UnableToCreateCommandException);
            }
        }

        [Test]
        public void GetListByProcedure()
        {
            var procedure = new Mock<Procedure>("TestProcedure");

            var customer1 = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };
            var customer2 = new Customer
            {
                ID = 2,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 2,
                Name = "Customer2",
                ProfileID = 2
            };
            var customer3 = new Customer
            {
                ID = 3,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 3,
                Name = "Customer3",
                ProfileID = 3
            };

            var customerQueue = new Queue<Customer>(new[] { customer1, customer2, customer3 });
            var readerResults = new Queue<bool>(new[] { true, true, true, false });

            var reader = new DbDataReader();
            reader.Setup(r => r.Read()).Returns(readerResults.Dequeue);

            var provider = new IProvider();

            var command = new ICommand();

            var schema = new TypeTable(typeof(Customer));

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);

            provider.CommandBuilder.Setup(c => c.CreateCommandFromProcedure(procedure.Object)).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteReader(command.Object)).Returns(reader.Object);

            provider.Mapper.Setup(m => m.GetTMappingMethod<Customer>()).Returns(d => customerQueue.Dequeue());

            var target = new DataSession(provider.Object);

            var actual = target.GetList<List<Customer>, Customer>(procedure.Object);

            Assert.IsTrue(actual[0].ID == 1 &&
                actual[1].ID == 2 && actual[2].ID == 3);

        }

        [Test]
        public void GetListByProcedureThrowsArgumentNullException()
        {
            try
            {
                var provider = new IProvider();
                var session = new DataSession(provider.Object);

                Procedure procedure = null;
                session.GetList<List<Customer>, Customer>(procedure);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentNullException);
            }
        }
        
        [Test]
        public void GetListByProcedureThrowsUnableToCreateCommandException()
        {
            var procedure = new Mock<Procedure>("TestProcedure");
            var provider = new IProvider();
            provider.CommandBuilder.Setup(c => c.CreateCommandFromProcedure(procedure.Object)).Returns(default(Data.ICommand));

            try
            {
                var target = new DataSession(provider.Object);
                target.GetList<List<Customer>, Customer>(procedure.Object);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UnableToCreateCommandException);
            }
        }

        #endregion

        #region GetPagedList
        [Test]
        public void GetPagedList()
        {
            var customer1 = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };
            var customer2 = new Customer
            {
                ID = 2,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 2,
                Name = "Customer2",
                ProfileID = 2
            };
            var customer3 = new Customer
            {
                ID = 3,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 3,
                Name = "Customer3",
                ProfileID = 3
            };

            var customerQueue = new Queue<Customer>(new[] { customer1, customer2, customer3 });
            var readerResults = new Queue<bool>(new[] { true, true, true, false });

            var reader = new DbDataReader();
            reader.Setup(r => r.Read()).Returns(readerResults.Dequeue);

            var provider = new IProvider();

            var command = new ICommand();

            var schema = new TypeTable(typeof(Customer));

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);

            provider.CommandBuilder.Setup(c => c.CreateGetListByPageCommand<Customer>()).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteReader(command.Object)).Returns(reader.Object);

            provider.Mapper.Setup(m => m.GetTMappingMethod<Customer>()).Returns(d => customerQueue.Dequeue());

            var target = new DataSession(provider.Object);

            var actual = target.GetPagedList<List<Customer>, Customer>(10, 1);

            Assert.IsTrue(actual[0].ID == 1 &&
                actual[1].ID == 2 && actual[2].ID == 3);
            
        }

        [Test]
        public void GetPagedListThrowsUnableToCreateCommandException()
        {
            var provider = new IProvider();
            provider.CommandBuilder.Setup(c => c.CreateGetListByPageCommand<Customer>()).Returns(default(Data.ICommand));

            try
            {
                var target = new DataSession(provider.Object);
                target.GetPagedList<List<Customer>, Customer>(10, 1);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UnableToCreateCommandException);
            }
        }

        [Test]
        public void GetPagedListByExpression()
        {
            Expression<Func<Customer, bool>> expression = e => e.ID < 4;

            var customer1 = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };
            var customer2 = new Customer
            {
                ID = 2,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 2,
                Name = "Customer2",
                ProfileID = 2
            };
            var customer3 = new Customer
            {
                ID = 3,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 3,
                Name = "Customer3",
                ProfileID = 3
            };

            var customerQueue = new Queue<Customer>(new[] { customer1, customer2, customer3 });
            var readerResults = new Queue<bool>(new[] { true, true, true, false });

            var reader = new DbDataReader();
            reader.Setup(r => r.Read()).Returns(readerResults.Dequeue);

            var provider = new IProvider();

            var command = new ICommand();

            var schema = new TypeTable(typeof(Customer));

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);

            provider.CommandBuilder.Setup(c => c.CreateGetListByPageCommand<Customer>(expression)).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteReader(command.Object)).Returns(reader.Object);

            provider.Mapper.Setup(m => m.GetTMappingMethod<Customer>()).Returns(d => customerQueue.Dequeue());

            var target = new DataSession(provider.Object);

            var actual = target.GetPagedList<List<Customer>, Customer>(10, 1, expression);

            Assert.IsTrue(actual[0].ID == 1 &&
                actual[1].ID == 2 && actual[2].ID == 3);

        }

        [Test]
        public void GetPagedListByExpressionThrowsArgumentNullException()
        {
            try
            {
                var provider = new IProvider();
                var session = new DataSession(provider.Object);

                Expression<Customer> expression = null;
                session.GetPagedList<List<Customer>, Customer>(10, 1, expression);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentNullException);
            }
        }

        [Test]
        public void GetPagedListByExpressionThrowsUnableToCreateCommandException()
        {
            Expression<Func<Customer, bool>> expression = e => e.ID < 4;
            var provider = new IProvider();
            provider.CommandBuilder.Setup(c => c.CreateGetListByPageCommand<Customer>(expression)).Returns(default(Data.ICommand));

            try
            {
                var target = new DataSession(provider.Object);
                target.GetPagedList<List<Customer>, Customer>(10, 1, expression);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UnableToCreateCommandException);
            }
        }
     
        #endregion

        #region PAgedListByPageEnum
        [Test]
        public void GetFirstPagedListByPageEnum()
        {
            GetPagedListByPageEnum(10, Data.Page.First);
        }

        [Test]
        public void GetMiddlePagedListByPageEnum()
        {
            GetPagedListByPageEnum(10, Data.Page.Middle);
        }

        [Test]
        public void GetLastPagedListByPageEnum()
        {
            GetPagedListByPageEnum(10, Data.Page.Last);
        }

        [Test]
        public void GetPagedListByPageEnumPageSizeEqualRowcount()
        {
            GetPagedListByPageEnum(3, Data.Page.Last);
        }

        private static void GetPagedListByPageEnum(int pageSize, Data.Page page)
        {
            var customer1 = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };
            var customer2 = new Customer
            {
                ID = 2,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 2,
                Name = "Customer2",
                ProfileID = 2
            };
            var customer3 = new Customer
            {
                ID = 3,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 3,
                Name = "Customer3",
                ProfileID = 3
            };

            var customerQueue = new Queue<Customer>(new[] { customer1, customer2, customer3 });
            var readerResults = new Queue<bool>(new[] { true, true, true, false });

            var reader = new DbDataReader();
            reader.Setup(r => r.Read()).Returns(readerResults.Dequeue);

            var provider = new IProvider();

            var command = new ICommand();

            var schema = new TypeTable(typeof(Customer));

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);

            provider.CommandBuilder.Setup(c => c.CreateGetListByPageCommand<Customer>()).Returns(command.Object);
            provider.CommandBuilder.Setup(c => c.CreateCountCommand<Customer>()).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteCount(command.Object)).Returns(3);
            provider.CommandExecutor.Setup(c => c.ExecuteReader(command.Object)).Returns(reader.Object);


            provider.Mapper.Setup(m => m.GetTMappingMethod<Customer>()).Returns(d => customerQueue.Dequeue());

            var target = new DataSession(provider.Object);

            var actual = target.GetPagedList<List<Customer>, Customer>(pageSize, page);

            Assert.IsTrue(actual[0].ID == 1 &&
                actual[1].ID == 2 && actual[2].ID == 3);
        }
        #endregion

        #region PagedListByPageEnumUsingExpressions
        [Test]
        public void GetFirstPagedListByExpression()
        {
            Expression<Func<Customer, bool>> expression = x => x.Balance > 100;
            GetPagedListByPageEnumExpression(4, Data.Page.First, expression);
        }

        [Test]
        public void GetLastPagedListByExpression()
        {
            Expression<Func<Customer, bool>> expression = x => x.Balance > 100;
            GetPagedListByPageEnumExpression(4, Data.Page.Last, expression);
        }

        [Test]
        public void GetMiddlePagedListByExpression()
        {
            Expression<Func<Customer, bool>> expression = x => x.Balance > 100;
            GetPagedListByPageEnumExpression(4, Data.Page.Middle, expression);
        }

        [Test]
        public void GetPagedListByExpressionPageSizeEqualRowCount()
        {
            Expression<Func<Customer, bool>> expression = x => x.Balance > 100;
            GetPagedListByPageEnumExpression(3, Data.Page.Middle, expression);
        }

        [Test]
        public void GetPagedListByPageEnumExpressionThrowsArgumentNullException()
        {
            Expression<Func<Customer, bool>> expression = null;
            GetPagedListByPageEnumExpression(4, Data.Page.Middle, expression);
        }

        private static void GetPagedListByPageEnumExpression(int pageSize, Data.Page page, Expression<Func<Customer, bool>> expression)
        {
            var customer1 = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };
            var customer2 = new Customer
            {
                ID = 2,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 2,
                Name = "Customer2",
                ProfileID = 2
            };
            var customer3 = new Customer
            {
                ID = 3,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 3,
                Name = "Customer3",
                ProfileID = 3
            };

            var customerQueue = new Queue<Customer>(new[] { customer1, customer2, customer3 });
            var readerResults = new Queue<bool>(new[] { true, true, true, false });

            var reader = new DbDataReader();
            reader.Setup(r => r.Read()).Returns(readerResults.Dequeue);

            var provider = new IProvider();

            var command = new ICommand();

            var schema = new TypeTable(typeof(Customer));

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);

            provider.CommandBuilder.Setup(c => c.CreateGetListByPageCommand<Customer>(expression)).Returns(command.Object);
            provider.CommandBuilder.Setup(c => c.CreateCountCommand<Customer>(expression)).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteCount(command.Object)).Returns(3);
            provider.CommandExecutor.Setup(c => c.ExecuteReader(command.Object)).Returns(reader.Object);


            provider.Mapper.Setup(m => m.GetTMappingMethod<Customer>()).Returns(d => customerQueue.Dequeue());

            var target = new DataSession(provider.Object);

            try
            {
                var actual = target.GetPagedList<List<Customer>, Customer>(pageSize, page, expression);

                Assert.IsTrue(actual[0].ID == 1 &&
                    actual[1].ID == 2 && actual[2].ID == 3);
            }
            catch (Exception ex)
            {

                Assert.IsTrue(ex is ArgumentNullException);
            }

        }
        #endregion

        #region Count
        
        [Test]
        public void CountAll()
        {
            var provider = new IProvider();
            var command = new ICommand();
            var schema = new TypeTable(typeof(Customer));

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);
            provider.CommandBuilder.Setup(c => c.CreateCountCommand<Customer>()).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteCount(command.Object)).Returns(10);

            var target = new DataSession(provider.Object);
            var result = target.Count<Customer>();
            Assert.IsTrue(result == 10);
        }

        [Test]
        public void CountAllWithTransactionInitiated()
        {
            var provider = new IProvider();
            var command = new ICommand();
            var schema = new TypeTable(typeof(Customer));

            var transaction = new Mock<ITransaction>();

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);
            provider.CommandBuilder.Setup(c => c.CreateCountCommand<Customer>()).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.InitiateTransaction()).Returns(transaction.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteCount(command.Object, transaction.Object)).Returns(10);

            var target = new DataSession(provider.Object);
            target.BeginTransaction();
            var result = target.Count<Customer>();
            target.CommitTransaction();
            Assert.IsTrue(result == 10);
        }

        [Test]
        public void CountUsingLambda()
        {
            var provider = new IProvider();
            var command = new ICommand();
            var schema = new TypeTable(typeof(Customer));

            Expression<Func<Customer, bool>> expression = x => x.ID < 10;

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);
            provider.CommandBuilder.Setup(c => c.CreateCountCommand<Customer>(expression.Body)).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteCount(command.Object)).Returns(10);

            var target = new DataSession(provider.Object);
            var result = target.Count(expression);
            Assert.IsTrue(result == 10);
        }

        [Test]
        public void CountUsingLambdaWithTransactionInitiated()
        {
            var provider = new IProvider();
            var command = new ICommand();
            var schema = new TypeTable(typeof(Customer));
            var transaction = new Mock<ITransaction>();

            Expression<Func<Customer, bool>> expression = x => x.ID < 10;

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);
            provider.CommandBuilder.Setup(c => c.CreateCountCommand<Customer>(expression.Body)).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.InitiateTransaction()).Returns(transaction.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteCount(command.Object, transaction.Object)).Returns(10);

            var target = new DataSession(provider.Object);
            target.BeginTransaction();
            var result = target.Count(expression);
            target.CommitTransaction();
            Assert.IsTrue(result == 10);
        }

        [Test]
        public void CountUsingLambdaThrowsArgumentNullException()
        {
            var provider = new IProvider();
            var command = new ICommand();
            var schema = new TypeTable(typeof(Customer));

            Expression<Func<Customer, bool>> expression = null;

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);
            provider.CommandBuilder.Setup(c => c.CreateCountCommand<Customer>(expression)).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteCount(command.Object)).Returns(10);

            try
            {
                var target = new DataSession(provider.Object);
                target.Count(expression);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentNullException);   
            }
            
        }

        [Test]
        public void CountUsingLambdaThrowsUnableToCreateCommandException()
        {
            Expression<Func<Customer, bool>> expression = e => e.ID < 4;
            var provider = new IProvider();
            provider.CommandBuilder.Setup(c => c.CreateCountCommand<Customer>(expression)).Returns(default(Data.ICommand));

            try
            {
                var target = new DataSession(provider.Object);
                target.Count(expression);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UnableToCreateCommandException);
            }
        }

        [Test]
        public void CountUsingExpression()
        {
            var provider = new IProvider();
            var command = new ICommand();
            var schema = new TypeTable(typeof(Customer));

            Expression<Func<Customer, bool>> expression = x => x.ID < 10;

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);
            provider.CommandBuilder.Setup(c => c.CreateCountCommand<Customer>(expression)).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteCount(command.Object)).Returns(10);

            var target = new DataSession(provider.Object);
            var result = target.Count<Customer>(expression as Expression);
            Assert.IsTrue(result == 10);
        }

        [Test]
        public void CountUsingExpressionWithTransactionInitiated()
        {
            var provider = new IProvider();
            var command = new ICommand();
            var schema = new TypeTable(typeof(Customer));
            var transaction = new Mock<ITransaction>();

            Expression<Func<Customer, bool>> expression = x => x.ID < 10;

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);
            provider.CommandBuilder.Setup(c => c.CreateCountCommand<Customer>(expression)).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.InitiateTransaction()).Returns(transaction.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteCount(command.Object, transaction.Object)).Returns(10);

            var target = new DataSession(provider.Object);
            target.BeginTransaction();
            var result = target.Count<Customer>(expression as Expression);
            target.CommitTransaction();
            Assert.IsTrue(result == 10);
        }

        [Test]
        public void CountUsingExpressionThrowsArgumentNullException()
        {
            var provider = new IProvider();
            var command = new ICommand();
            var schema = new TypeTable(typeof(Customer));

            Expression expression = null;

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);
            provider.CommandBuilder.Setup(c => c.CreateCountCommand<Customer>(expression)).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteCount(command.Object)).Returns(10);

            try
            {
                var target = new DataSession(provider.Object);
                target.Count<Customer>(expression);
            }
            catch (Exception ex)
            {
                
                Assert.IsTrue(ex is ArgumentNullException);
            }
            
            
        }

        [Test]
        public void CountUsingExpressionThrowsUnableToCreateCommandException()
        {
            Expression<Func<Customer, bool>> expression = e => e.ID < 4;
            var provider = new IProvider();
            provider.CommandBuilder.Setup(c => c.CreateCountCommand<Customer>(expression)).Returns(default(Data.ICommand));

            try
            {
                var target = new DataSession(provider.Object);
                target.Count<Customer>(expression as Expression);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UnableToCreateCommandException);
            }
        }

        #endregion

        #region Transaction

        [Test]
        public void BeginTransactionCallsExecutorInitiatesTransaction()
        {
            var provider = new IProvider();
            var transaction = new Mock<ITransaction>();
            provider.CommandExecutor.Setup(c => c.InitiateTransaction()).Returns(transaction.Object);
            
            var target = new DataSession(provider.Object);
            target.BeginTransaction();
            provider.CommandExecutor.Verify(c => c.InitiateTransaction());
        }

        [Test]
        public void InitiateTransactionMoreThanOnceThrowsTransactionInstantiatedAlreadyException()
        {
            var provider = new IProvider();
            var transaction = new Mock<ITransaction>();
            provider.CommandExecutor.Setup(c => c.InitiateTransaction()).Returns(transaction.Object);

            var target = new DataSession(provider.Object);
            try
            {
                target.BeginTransaction();
                target.BeginTransaction();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is TransactionInstantiatedAlreadyException);
            }
        }

        [Test]
        public void CommitTransactionCallsExecutorCommitTransaction()
        {
            var provider = new IProvider();
            var transaction = new Mock<ITransaction>();
            provider.CommandExecutor.Setup(c => c.InitiateTransaction()).Returns(transaction.Object);

            var target = new DataSession(provider.Object);
            target.BeginTransaction();
            target.CommitTransaction();
            provider.CommandExecutor.Verify(c => c.InitiateTransaction());
            provider.CommandExecutor.Verify(c => c.CommitTransaction(transaction.Object));
        }

        [Test]
        public void CommitTransactionWithoutBeginTransactionThrowsTransactionNotInstantiatedException()
        {
            var provider = new IProvider();
            var transaction = new Mock<ITransaction>();
            provider.CommandExecutor.Setup(c => c.InitiateTransaction()).Returns(transaction.Object);

            var target = new DataSession(provider.Object);
            try
            {
                target.CommitTransaction();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is TransactionNotInstantiatedException);
            }
        }

        [Test]
        public void RollbackTransactionCallsExecutorRollBackTransaction()
        {
            var provider = new IProvider();
            var transaction = new Mock<ITransaction>();
            provider.CommandExecutor.Setup(c => c.InitiateTransaction()).Returns(transaction.Object);

            var target = new DataSession(provider.Object);
            target.BeginTransaction();
            target.RollBackTransaction();
            provider.CommandExecutor.Verify(c => c.InitiateTransaction());
            provider.CommandExecutor.Verify(c => c.RollbackTransaction(transaction.Object));
        }

        [Test]
        public void RollbackTransactionWithoutBeginTransactionThrowsTransactionNotInstantiatedException()
        {
            var provider = new IProvider();
            var transaction = new Mock<ITransaction>();
            provider.CommandExecutor.Setup(c => c.InitiateTransaction()).Returns(transaction.Object);

            var target = new DataSession(provider.Object);
            try
            {
                target.RollBackTransaction();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is TransactionNotInstantiatedException);
            } 
        }
        #endregion

        #region Save
        
        [Test]
        public void SaveThrowsArgumentNullException()
        {
            try
            {
                var provider = new IProvider();
                var session = new DataSession(provider.Object);
                session.Save(null);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is ArgumentNullException);
            }
        }

        [Test]
        public void SaveRollBacksTransactionWhenExceptionThrown()
        {
            var customer = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };

            var provider = new IProvider();
            var transaction = new Mock<ITransaction>();

            provider.CommandExecutor.Setup(c => c.InitiateTransaction()).Returns(transaction.Object);
            provider.Setup(p => p.GetEntityStatus(customer)).Returns(EntityStatus.Insert);
            provider.CommandBuilder.Setup(c => c.CreateInsertCommand(customer)).Returns(default(Data.ICommand));

            var target = new DataSession(provider.Object);
            try
            {
                target.BeginTransaction();
                target.Save(customer);
            }
            catch (Exception)
            {
                Assert.IsTrue(!target.IsTransactionInitiated);
                
            }
        }

        [Test]
        public void SaveInsert()
        {
            var customer = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };

            var provider = new IProvider();
            var transaction = new Mock<ITransaction>();
            var command = new ICommand();
            var schema = new TypeTable(typeof(Customer));

            var reader = new DbDataReader();
            reader.Setup(r => r.Read()).Returns(true);
            

            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);
            provider.CommandExecutor.Setup(c => c.InitiateTransaction()).Returns(transaction.Object);
            provider.Setup(p => p.GetEntityStatus(customer)).Returns(EntityStatus.Insert);
            provider.CommandBuilder.Setup(c => c.CreateInsertCommand(customer)).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteReader(command.Object, transaction.Object)).Returns(
                reader.Object);

            provider.Mapper.Setup(m => m.GetObjectMappingMethod(typeof(Customer))).Returns(d => customer);

            var target = new DataSession(provider.Object);
            target.Save(customer);

            provider.CommandExecutor.Verify(c => c.InitiateTransaction());
            provider.CommandExecutor.Verify(c => c.CommitTransaction(transaction.Object));

            Assert.IsTrue(customer.ID == 1);
        }

        [Test]
        public void SaveInsertThrowsUnableToCreateCommandException()
        {
            var customer = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };

            var provider = new IProvider();
            var transaction = new Mock<ITransaction>();
            var schema = new TypeTable(typeof(Customer));

            var reader = new DbDataReader();
            reader.Setup(r => r.Read()).Returns(true);


            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);
            provider.CommandExecutor.Setup(c => c.InitiateTransaction()).Returns(transaction.Object);
            provider.Setup(p => p.GetEntityStatus(customer)).Returns(EntityStatus.Insert);
            provider.CommandBuilder.Setup(c => c.CreateInsertCommand(customer)).Returns(default(Data.ICommand));
            
            try
            {
                var target = new DataSession(provider.Object);
                target.Save(customer);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UnableToCreateCommandException);
            }
            
        }

        [Test]
        public void SaveUpdate()
        {
            var customer = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };

            var provider = new IProvider();
            var transaction = new Mock<ITransaction>();
            var command = new ICommand();
            var schema = new TypeTable(typeof(Customer));

            var reader = new DbDataReader();
            reader.Setup(r => r.Read()).Returns(true);


            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);
            provider.CommandExecutor.Setup(c => c.InitiateTransaction()).Returns(transaction.Object);
            provider.Setup(p => p.GetEntityStatus(customer)).Returns(EntityStatus.Update);
            provider.CommandBuilder.Setup(c => c.CreateUpdateCommand(customer)).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteReader(command.Object, transaction.Object)).Returns(
                reader.Object);

            provider.Mapper.Setup(m => m.GetObjectMappingMethod(typeof(Customer))).Returns(d => customer);

            var target = new DataSession(provider.Object);
            target.Save(customer);

            provider.CommandExecutor.Verify(c => c.InitiateTransaction());
            provider.CommandExecutor.Verify(c => c.CommitTransaction(transaction.Object));

            Assert.IsTrue(customer.ID == 1);
        }

        [Test]
        public void SaveUpdateThrowsUnableToCreateCommandException()
        {
            var customer = new Customer
            {
                ID = 1,
                Balance = 0,
                BalanceRate = 0,
                DefaultContactID = 1,
                Name = "Customer1",
                ProfileID = 1
            };

            var provider = new IProvider();
            var transaction = new Mock<ITransaction>();
            var schema = new TypeTable(typeof(Customer));

            var reader = new DbDataReader();
            reader.Setup(r => r.Read()).Returns(true);


            provider.Setup(p => p.GetSchema(typeof(Customer))).Returns(schema);
            provider.CommandExecutor.Setup(c => c.InitiateTransaction()).Returns(transaction.Object);
            provider.Setup(p => p.GetEntityStatus(customer)).Returns(EntityStatus.Update);
            provider.CommandBuilder.Setup(c => c.CreateUpdateCommand(customer)).Returns(default(Data.ICommand));

            try
            {
                var target = new DataSession(provider.Object);
                target.Save(customer);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UnableToCreateCommandException);
            }

        }
        #endregion  

        #region Execute

        #region ExecuteScalar
        [Test]
        public void ExecuteScalarByExpressionThrowsArgumentNullException()
        {
            var provider = new IProvider();
            var target = new DataSession(provider.Object);
            Expression<Func<Customer, bool>> expression = null;
            Expression<Func<Customer, object>> column = null;

            var result = false;
            try
            {
                target.ExecuteScalar(x => x.ID, expression);
            }
            catch (Exception ex)
            {
                result = ex is ArgumentNullException;
            }

            try
            {
                target.ExecuteScalar(column, x => x.ID == 1);
            }
            catch (Exception ex)
            {
                result = ex is ArgumentNullException;
            }

            Assert.IsTrue(result);
        }

        [Test]
        public void ExecuteScalarByExpressionThrowsUnableToCreateCommandException()
        {
            try
            {
                Expression<Func<Customer, bool>> expression = x => x.ID == 1;
                Expression<Func<Customer, object>> column = c => c.ID;
                var provider = new IProvider();
                provider.CommandBuilder.Setup(c => c.CreateExecuteScalarCommand<Customer>(column, expression)).Returns(
                    default(Data.ICommand));
                
                var target = new DataSession(provider.Object);
                target.ExecuteScalar(column, expression);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is UnableToCreateCommandException);
            }
        }

        [Test]
        public void ExecuteScalarByExpressionThrowsInvalidCastException()
        {
            Expression<Func<Customer, bool>> expression = x => x.ID == 1;
            Expression<Func<Customer, int>> column = c => c.ID;
            var provider = new IProvider();
            var command = new ICommand();

            provider.CommandBuilder.Setup(c => c.CreateExecuteScalarCommand<Customer>(column, expression)).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteScalar(command.Object)).Returns("HelloWorld");

            try
            {
                var target = new DataSession(provider.Object);
                var result = target.ExecuteScalar(column, expression);

                provider.CommandExecutor.Verify(c => c.FinalizeCommand(command.Object));
             
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is InvalidCastException);
            }
            
        }

        [Test]
        public void ExecuteScalarByExpression()
        {
            Expression<Func<Customer, bool>> expression = x => x.ID == 1;
            Expression<Func<Customer, int>> column = c => c.ID;
            var provider = new IProvider();
            var command = new ICommand();

            provider.CommandBuilder.Setup(c => c.CreateExecuteScalarCommand<Customer>(column, expression)).Returns(command.Object);
            provider.CommandExecutor.Setup(c => c.ExecuteScalar(command.Object)).Returns(2);

            var target = new DataSession(provider.Object);
            var result = target.ExecuteScalar(column, expression);

            provider.CommandExecutor.Verify(c => c.FinalizeCommand(command.Object));
            Assert.AreEqual(result, 2);
        }
        #endregion
        
        #endregion
    }
}
