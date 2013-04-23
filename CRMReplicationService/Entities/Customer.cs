// -----------------------------------------------------------------------
// <copyright file="Customer.cs" company="Nexcel">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.CRM.CRMReplication.Entities
{
    using MongoDB.Bson;

    /// <summary>
    /// Customer Object
    /// </summary>
    public class Customer
    {
        public ObjectId _id { get; set; }
        public int CustId { get; set; }
        public string CustomerName { get; set; }
    }
}
