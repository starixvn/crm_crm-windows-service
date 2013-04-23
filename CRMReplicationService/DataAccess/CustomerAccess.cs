// -----------------------------------------------------------------------
// <copyright file="CustomerAccess.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.CRM.CRMReplication.DataAccess
{
    using System.Data;

    using Fanex.Data;

    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CustomerAccess : ICustomerAccess
    {

        public BsonDocument GetCustomerById(int customerId)
        {
            
            var server = MongoServer.Create(Core.AppSettings.MongoConnectionString);
            var crmDb = server.GetDatabase(Core.AppSettings.MongoDbName);
            
            IMongoQuery query = new QueryDocument();
            query = Query.And(query, Query.EQ("_id", BsonValue.Create(customerId)));
            var customerTable = crmDb.GetCollection(Core.AppSettings.TableCustomerName).FindOne(query);
            return customerTable;
        }

        public void AddNewCustomer(int customerId, string customerName)
        {
            
            var server = MongoServer.Create(Core.AppSettings.MongoConnectionString);
            var crmDb = server.GetDatabase(Core.AppSettings.MongoDbName);

            var customers = crmDb.GetCollection<BsonDocument>(Core.AppSettings.TableCustomerName);
            
            //add new customer
            var customer = new BsonDocument()
            {
                //{"CustId", customerId},
                {"UserName", customerName.ToLower()},
                {"SearchCount", 1},
                {"_id", customerId}
            };
            
            customers.Insert(customer);
            
        }

        public SafeModeResult UpdateCutomer(int customerId, string customerName)
        {
            
            var server = MongoServer.Create(Core.AppSettings.MongoConnectionString);
            var crmDb = server.GetDatabase(Core.AppSettings.MongoDbName);
            var customers = crmDb.GetCollection<BsonDocument>(Core.AppSettings.TableCustomerName);

            var query = Query.And(Query.EQ("_id", customerId));
            var update = Update.Set("UserName", customerName.ToLower());
            return customers.Update(query, update);

        }

        public SafeModeResult RemoveCustomerById(int customerId)
        {
            
            var server = MongoServer.Create(Core.AppSettings.MongoConnectionString);
            var crmDb = server.GetDatabase(Core.AppSettings.MongoDbName);
            var customers = crmDb.GetCollection<BsonDocument>(Core.AppSettings.TableCustomerName);
            var query = Query.And(Query.EQ("_id", customerId));
            return customers.Remove(query);
        }

        public void RemoveAllCustomers()
        {
            var server = MongoServer.Create(Core.AppSettings.MongoConnectionString);
            var crmDb = server.GetDatabase(Core.AppSettings.MongoDbName);

            var customers = crmDb.GetCollection<BsonDocument>(Core.AppSettings.TableCustomerName);
            customers.RemoveAll();
        }

        public DataSet SelAllCustomersChangedFromCRMSystem(string maxTimesTamp)
        {
            using (var objDb = new ObjectDb("SelCustomersByTimesTamp"))
            {
                objDb.AddInParameter("tstamp", DbType.Binary, Core.Helper.ByteUtils.GetBytes(maxTimesTamp));
                return objDb.ExecuteDataSet();
            }
        }

    }
}
