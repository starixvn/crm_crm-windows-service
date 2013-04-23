// -----------------------------------------------------------------------
// <copyright file="ICustomerAccess.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.CRM.CRMReplication.DataAccess
{
    using System.Data;

    using MongoDB.Driver;
    using MongoDB.Bson;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICustomerAccess
    {
        BsonDocument GetCustomerById(int customerId);

        void AddNewCustomer(int customerId, string customerName);

        SafeModeResult UpdateCutomer(int customerId, string customerName);

        SafeModeResult RemoveCustomerById(int customerId);

        void RemoveAllCustomers();

        DataSet SelAllCustomersChangedFromCRMSystem(string maxTimesTamp);
    }
}
