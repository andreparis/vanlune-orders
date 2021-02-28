using Orders.Domain.Entities;
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

            template = template.Replace("#{{tableOrders}}", GetTableOrders(orders.Select(a => a.Product)));

            var totalPrice = orders.Sum(p => p.Product.Price * p.Product.Quantity).ToString();

            template = template.Replace("#{{price}}", totalPrice);
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

        private static string GetTableOrders(IEnumerable<Product> products)
        {
            var table = new StringBuilder();

            foreach (var product in products)
            {
                table.Append("<tr>");

                table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
                table.Append($"<h5 style=\"margin-top: 15px; \">{product.Title}</h5>");
                table.Append("</td>");

                table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
                table.Append($"<h5 style=\"font-size: 14px; color:#444;margin-top: 10px;\">{product.Quantity}</h5>");
                table.Append("</td>");

                table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
                table.Append($"<h5 style=\"font-size: 14px; color:#444;margin-top:15px\"><b>${Math.Round(product.Price * product.Quantity, 2)}</b></h5>");
                table.Append("</td>");

                table.Append("</tr>");
            }

            return table.ToString();
        }
    }
}
