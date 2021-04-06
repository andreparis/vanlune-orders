using System;
using System.Collections.Generic;
using System.Text;

namespace Orders.Domain.Entities
{
    public class Link
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }

    public class Details
    {
        public string subtotal { get; set; }
        public string shipping { get; set; }
        public string insurance { get; set; }
        public string handling_fee { get; set; }
        public string shipping_discount { get; set; }
        public string discount { get; set; }
    }

    public class Amount
    {
        public string total { get; set; }
        public string currency { get; set; }
        public Details details { get; set; }
    }

    public class Payee
    {
        public string merchant_id { get; set; }
        public string email { get; set; }
    }

    public class ShippingAddress
    {
        public string recipient_name { get; set; }
        public string line1 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
        public bool default_address { get; set; }
        public bool preferred_address { get; set; }
        public bool primary_address { get; set; }
        public bool disable_for_transaction { get; set; }
    }

    public class ItemList
    {
        public ShippingAddress shipping_address { get; set; }
    }

    public class TransactionFee
    {
        public string value { get; set; }
        public string currency { get; set; }
    }

    public class Sale
    {
        public string id { get; set; }
        public string state { get; set; }
        public Amount amount { get; set; }
        public string payment_mode { get; set; }
        public string protection_eligibility { get; set; }
        public string protection_eligibility_type { get; set; }
        public TransactionFee transaction_fee { get; set; }
        public string parent_payment { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
        public List<Link> links { get; set; }
        public string soft_descriptor { get; set; }
    }

    public class RelatedResource
    {
        public Sale sale { get; set; }
    }

    public class Transaction
    {
        public Amount amount { get; set; }
        public Payee payee { get; set; }
        public string soft_descriptor { get; set; }
        public ItemList item_list { get; set; }
        public List<RelatedResource> related_resources { get; set; }
    }

    public class PayerInfo
    {
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string payer_id { get; set; }
        public ShippingAddress shipping_address { get; set; }
        public string phone { get; set; }
        public string country_code { get; set; }
    }

    public class Payer
    {
        public string payment_method { get; set; }
        public string status { get; set; }
        public PayerInfo payer_info { get; set; }
    }

    public class DisputedTransaction
    {
        public string seller_transaction_id { get; set; }
    }

    public class DisputedAmount
    {
        public string Value { get; set; }
        public string currency_code { get; set; }
    }

    public class TotalRefunded
    {
        public string Value { get; set; }
        public string currency { get; set; }
    }

    public class Resource
    {
        public DateTime update_time { get; set; }
        public DateTime create_time { get; set; }
        public List<Link> links { get; set; }
        public string id { get; set; }
        public string state { get; set; }
        public List<Transaction> transactions { get; set; }
        public string intent { get; set; }
        public Payer payer { get; set; }
        public string cart { get; set; }
        public string parent_payment { get; set; }
        public Amount amount { get; set; }
        public IEnumerable<DisputedTransaction> disputed_transactions { get; set; }
        public string reason { get; set; }
        public DisputedAmount dispute_amount { get; set; }
        public string sale_id { get; set; }
        public string summary { get; set; }
        public TotalRefunded total_refunded_amount { get; set; }
    }
}
