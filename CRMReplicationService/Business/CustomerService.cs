// -----------------------------------------------------------------------
// <copyright file="CustomerService.cs" company="Nexcel">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.CRM.CRMReplication.Business
{
    using System;
    using System.Data;
    using DNAide.Core;
    using Fanex.CRM.CRMReplication.Core;
    using Fanex.CRM.CRMReplication.Core.Extensions;
    using Fanex.CRM.CRMReplication.Core.Helper;
    using Fanex.CRM.CRMReplication.DataAccess;
    using MongoDB.Driver;
    using MongoDB.Bson;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CustomerService : ICustomerService
    {
        
        public CustomerService()
        {
        }

        private string MaxTsTp {
            get
            {
                var maxTime = CacheService.Get(Constant.CacheKeys.MaxTsTmp) as string;
                return string.IsNullOrEmpty(maxTime) ? AppSettings.CustomerMaxTsTamp : maxTime;
                //return AppSettings.CustomerMaxTsTamp;
            }
        }

        #region MongoDb - All Actions

        public BsonDocument GetCustomerById(int customerId)
        {
            try
            {
                ICustomerAccess customerAccess = new CustomerAccess();
                return customerAccess.GetCustomerById(customerId);
            }
            catch (Exception ex)
            {
                LogExtensions.ExceptionHandler(ex);
                return null;                
            }
        }

        public void AddNewCustomer(int customerId, string customerName)
        {
            try
            {
                ICustomerAccess customerAccess = new CustomerAccess();
                customerAccess.AddNewCustomer(customerId, customerName);
            }
            catch (Exception ex)
            {
                LogExtensions.ExceptionHandler(ex);
            }
            
        }

        public SafeModeResult UpdateCustomer(int customerId, string customerName)
        {
            try
            {

                ICustomerAccess customerAccess = new CustomerAccess();
                return customerAccess.UpdateCutomer(customerId, customerName);

            }
            catch (Exception ex)
            {
                LogExtensions.ExceptionHandler(ex);
                return new SafeModeResult();   
            }
        }

        public SafeModeResult RemoveCustomerById(int customerId)
        {
            try
            {
                ICustomerAccess customerAccess = new CustomerAccess();
                return customerAccess.RemoveCustomerById(customerId);
            }
            catch (Exception ex)
            {
                LogExtensions.ExceptionHandler(ex);
                return new SafeModeResult();
            }
        }

        public void RemoveAllCustomers()
        {
            try
            {
                ICustomerAccess customerAccess = new CustomerAccess();
                customerAccess.RemoveAllCustomers();
            }
            catch (Exception ex)
            {
                LogExtensions.ExceptionHandler(ex);
            }
        }
        #endregion

        #region Db Notify

        public void ReplicateData()
        {
            var lastTsTamp = string.Empty;
            try
            {
                ICustomerAccess customerAccess = new CustomerAccess();
                var customers = customerAccess.SelAllCustomersChangedFromCRMSystem(MaxTsTp);

                if (customers != null && customers.Tables.Count > 0)
                {
                    byte[] maxTs = null;

                    if (customers.Tables[0].Rows != null && customers.Tables[0].Rows.Count > 0)
                    {
                        var customersUpdated = customers.Tables[0].Rows;
                        foreach (DataRow dr in customersUpdated)
                        {
                            var custId = CastAide.AsInt32(dr["CustId"]);
                            var custName = CastAide.AsString(dr["UserName"]).Replace(".", "2_2");

                            var custInfo = GetCustomerById(custId);
                            
                            if (custInfo != null)
                            {
                                UpdateCustomer(custId, custName);
                                
                                //foreach (var item in custInfo)
                                //{
                                //    UpdateCustomer(custId, custName);
                                //}
                            }
                            else {
                                AddNewCustomer(custId, custName);
                            }
                            
                            maxTs = dr["tstamp"] as byte[];
                        }

                        lastTsTamp = ByteUtils.ByteArrayToString(maxTs);
                        CacheService.Set(Constant.CacheKeys.MaxTsTmp, lastTsTamp);

                        if(CacheService.Get(Constant.CacheKeys.MaxTsTmp)==null)
                            AppSettings.CustomerMaxTsTamp = lastTsTamp;
                    
                    }
                    
                }
            }
            catch (Exception ex)
            {
                AppSettings.CustomerMaxTsTamp = lastTsTamp;
                LogExtensions.ExceptionHandler(ex);
            }
        }

        #endregion

    }
}