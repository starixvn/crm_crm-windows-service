// -----------------------------------------------------------------------
// <copyright file="ICustomerService.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.CRM.CRMReplication.Business
{

    using MongoDB.Driver;
    using MongoDB.Bson;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICustomerService
    {
        BsonDocument GetCustomerById(int customerId);

        void AddNewCustomer(int customerId, string customerName);

        SafeModeResult UpdateCustomer(int customerId, string customerName);

        SafeModeResult RemoveCustomerById(int customerId);

        void RemoveAllCustomers();

        void ReplicateData();
    }
}
