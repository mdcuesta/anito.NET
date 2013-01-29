using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using Anito.Test.Schema;
using Anito.Test.Schema.BusinessObjects;
using Anito.Data;

namespace Anito.Test.Forms
{
    public partial class Host : Form
    {
        public Host()
        {
            InitializeComponent();
        }

        public static void Main(string[] args)
        {
            Host form = new Host();
            form.ShowDialog();
            
        }

        private void Host_Load(object sender, EventArgs e)
        {
            //Use Default
            //ISession session = ProviderFactory.GetSession();

            ////var res = session.GetT<Entity>(p => !p.Name.Contains("CUESTA"));

            ////var res = session.GetT<Entity>(p => !SqlMethod.Like(p.Name, "_ICHAEL DELA CUEST_"));

            ////var res = session.GetT<Entity>(p => !p.Name.Like("_ICHAEL DELA CUEST_"));

            ////DateTime to;
            ////DateTime from;

            ////DateTime.TryParse("2011-10-19 14:56:09.241", out from);
            ////DateTime.TryParse("2011-10-20 14:56:09.249", out to);
            
            //////SELECT Entity.* FROM Entity WHERE DateRegistered BETWEEN(from, to)
            ////var lst = session.GetList<List<Entity>, Entity>(p => p.DateRegistered.Value.Between(from, to));

            ////var lst1 = session.GetList<List<Entity>, Entity>(p => p.ID.In(1, 2, 3, 4, 5));
            
            ////var lst2 = session.GetList<List<Entity>, Entity>(k => k.Name.In("MICHAEL DELA CUESTA", "CARMICHAEL"));

            

            
            //////SqlMethod.Between<int>(2, 1, 3);
            //////var op = SqlMethod.In(4, 1, 2, 3, 4, 5);

            //////int xp = 10;
            ////int x = session.ExecuteScalar<Entity, int>(m => m.ID, l => l.ID == 2);

            //var entity = session.GetT<Entity>(t => t.EntityCode == "ENT9230001");

            //var orders = entity.Orders;


            //SalesOrder order = session.GetT<SalesOrder>("ORD-000001");
            //var details = order.Details;

            //order.OrderDate = DateTime.Now;

            ////session.BeginTransaction();

            //foreach (SalesOrderDetail detail in order.Details)
            //    detail.QuantityOrdered = 13;
            
            
            //order.Save();
            
            //session.CommitTransaction();

            DataSessionExecuteScalar();
            DataSessionCount();
            DataSessionGet();
            DataSessionGetWithRelations();
            DataSessionUpdate();
            DataSessionInsert();  
            Close();
        }

        private void DataSessionCount()
        {
            IProvider provider = ProviderFactory.GetProvider("SqlClient");
            ISession session = ProviderFactory.GetSession(provider);
            int all = session.Count<Entity>();
            int idlessthan10 = session.Count<Entity>(ent => ent.ID < 10);

        }
        private void DataSessionGet()
        {
            IProvider provider = ProviderFactory.GetProvider("SqlClient");
            ISession session = ProviderFactory.GetSession(provider);

            //GetT(Expression)
            Entity entity1  = session.GetT<Entity>(e => e.ID == 1);

            Console.WriteLine("Start");
            entity1.Name = "Test";
            Entity entity2 = session.GetT<Entity>(e => e.EntityCode == "ENT9230001");
            Entity entity3 = session.GetT<Entity>(e => e.EmailAddress == "michael.dcuesta@gmail.com");
            
            //GetT(key)
            Entity entity4 = session.GetT<Entity>("ENT9230001");

            //GetList - All
            var list1 = session.GetList<List<Entity>, Entity>();

            //GetList(Expression)
            var list2 = session.GetList<List<Entity>, Entity>(e => e.ID < 20);
            
            //GetList(Procedure) ; there should also be a GetT<T>(Procedure)
            var list3 = session.GetList<List<Entity>, Entity>(StoredProcedure.GetEntityByEntityCode("ENT9230002"));
            var list4 = session.GetList<List<Entity>, Entity>(StoredProcedure.GetEntityByState("NEW YORK"));

            //PagedList
            var list5 = session.GetPagedList<List<Entity>, Entity>(10, 1);
            var list6 = session.GetPagedList<List<Entity>, Entity>(20, Page.Last);

            //PagedList(Expression)
            var list7 = session.GetPagedList<List<Entity>, Entity>(20, 1, e => e.State == "NEW YORK");


        }

        private void DataSessionGetWithRelations()
        {
            IProvider provider = ProviderFactory.GetProvider("SqlClient");
            ISession session = ProviderFactory.GetSession(provider);

            
            SalesOrder order = session.GetT<SalesOrder>("ORD-000001");

            var entity = order.Customer;
            //SalesOrder 1 to M SalesOrderDetails
            List<SalesOrderDetail> salesOrderDetails = order.Details;
            
            //SalesOrder 1 to 1 Entity (Customer)
            Entity customer = order.Customer;
            
            //SalesOrderDetail 1 to 1 Item
            Item itm1 = order.Details[0].Item;
            Item itm2 = order.Details[1].Item;
            Item itm3 = order.Details[2].Item;
            Item itm4 = order.Details[3].Item;
            order.BalanceRate = order.BalanceRate + 1;
            
            order.Save();
            
        }

        private void DataSessionInsert()
        {
            IProvider provider = ProviderFactory.GetProvider("SqlClient"); //MySqlClient //SqlClient //SQLiteClient
            ISession session = ProviderFactory.GetSession(provider);
            
            Entity newEntity1 = new Entity();
            newEntity1.EntityCode = Guid.NewGuid().ToString().Substring(0, 10);
            newEntity1.Name = "Test Insert Name";
            newEntity1.Identifier = Guid.NewGuid();

            
            //Insert
            session.Insert<Entity>(ref newEntity1);

            //Entity newEntity2 = new Entity(session);
            //newEntity2.EntityCode = Guid.NewGuid().ToString().Substring(0, 10);
            //newEntity2.Name = "Test Insert Name";
            
            ////Data Object Save
            //newEntity2.Save();

            
        }

        private void DataSessionUpdate()
        {
            IProvider provider = ProviderFactory.GetProvider("SqlClient"); 
            ISession session = ProviderFactory.GetSession(provider);

            Entity entity1 = session.GetT<Entity>("ENT9230001");
            entity1.Name = "MICHAEL DELA CUESTA";
            
            //this will cause an error on update depending on the database maximum date and time
            //entity1.DateRegistered = default(DateTime);
            entity1.Save();
        }

        private void DataSessionExecuteScalar()
        {
            var provider = ProviderFactory.GetProvider("SqlClient");
            var session = ProviderFactory.GetSession(provider);

            var id = session.ExecuteScalar<Entity, int>(x => x.ID, y => y.ID == 1);
            var name = session.ExecuteScalar<Entity, string>(x => x.Name, y => y.ID == 1);
        }

    }
}
