using Orders.Domain.Entities;
using Orders.Domain.Entities.ExProduct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Domain.Messaging.Email
{
    public static class OrdersTemplate
    {
        public static string GetOrderBody(IEnumerable<Order> orders)
        {
            var template = GetTemplateAsync("Orders.Domain.Messaging.Email.Templates.OrderTemplate.html");

            template = template.Replace("#{{tableOrders}}", GetTableOrders(orders));

            var totalPrice = orders.Sum(p => p.Price * p.Quantity * p.Variant.Factor * GetExtrasFactor(p.Customizes)).ToString();

            template = template.Replace("#{{price}}", totalPrice);

            if (orders.Any(o => o.Product.Discount > 0))
            {
                var productsDiscount = orders.Where(o => o.Product.Discount > 0);

                var amountReal = productsDiscount.Sum(p => p.Price * p.Quantity);
                var amountDiscount = productsDiscount.Sum(p => p.Price * p.Quantity * p.Discount) / 100;

                var discount = amountReal - amountDiscount;

                template = template.Replace("#{{discount}}", discount.ToString());
            }
            else
                template = template.Replace("#{{discount}}", "0");

            template = template.Replace("#{{total}}", totalPrice);

            var user = orders.FirstOrDefault().User;

            template = template.Replace("#{{userName}}", user.Name);
            template = template.Replace("#{{userEmail}}", user.Email);
            template = template.Replace("#{{userPhone}}", user.Phone);

            return template;
        }

        private static string GetTemplateAsync(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);
            var reader = new StreamReader(stream);
            string template = reader.ReadToEndAsync().Result;
            return template;
        }

        private static string GetTableOrders(IEnumerable<Order> orders)
        {
            var table = new StringBuilder();

            foreach (var order in orders)
            {
                table.Append("<tr>");

                table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
                table.Append($"<h5 style=\"font-size: 14px; color:#444;margin-top: 10px;\">{order.Id}</h5>");
                table.Append("</td>");

                table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
                table.Append($"<h5 style=\"margin-top: 15px; \">{order.Product.Title}</h5>");
                table.Append("</td>");

                table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
                table.Append($"<h5 style=\"font-size: 14px; color:#444;margin-top: 10px;\">{order.Quantity}</h5>");
                table.Append("</td>");

                table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
                table.Append($"<h5 style=\"font-size: 14px; color:#444;margin-top:15px\"><b>${Math.Round(order.Price, 2)}</b></h5>");
                table.Append("</td>");

                table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
                table.Append($"<h5 style=\"font-size: 14px; color:#444;margin-top:15px\"><b>{order.Variant.Name} x {order.Variant.Factor}</b></h5>");
                table.Append("</td>");

                table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
                table.Append($"<h5 style=\"font-size: 14px; color:#444;margin-top:15px\"><b>{GetExtrasAsStr(order.Customizes)}</b></h5>");
                table.Append("</td>");

                table.Append("</tr>");
            }

            return table.ToString();
        }

        private static string GetExtrasAsStr(IEnumerable<Customize> customizes)
        {
            var result = new StringBuilder();

            foreach(var customize in customizes)
            {
                var value = customize.Value.First();
                result.Append($"{customize.Name}: {value.Name} x {value.Factor}");
            }

            return result.ToString();
        }
    
        private static decimal GetExtrasFactor(IEnumerable<Customize> customizes)
        {
            var result = (decimal)1;

            foreach (var customize in customizes)
            {
                var value = customize.Value.First();
                result *= value.Factor;
            }

            return result;
        }
    }
}
