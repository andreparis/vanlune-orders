using Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Orders.Domain.Messaging.Email
{
    public static class RefundTemplate
    {
        public static string GetRefundBody(string externalId, string summary, PayerInfo payer)
        {
            var template = GetTemplateAsync("Orders.Domain.Messaging.Email.Templates.RefundTemplate.html");

            template = template.Replace("#{{tableOrders}}", GetTableRefund(externalId, summary, payer));

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

        private static string GetTableRefund(string externalId, string summary, PayerInfo payer)
        {
            var table = new StringBuilder();

            table.Append("<tr>");

            table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
            table.Append($"<h5 style=\"font-size: 14px; color:#444;margin-top: 10px;\">{externalId}</h5>");
            table.Append("</td>");

            table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
            table.Append($"<h5 style=\"margin-top: 15px; \">{summary}</h5>");
            table.Append("</td>");

            table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
            table.Append($"<h5 style=\"font-size: 14px; color:#444;margin-top: 10px;\">{payer.email}</h5>");
            table.Append("</td>");

            table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
            table.Append($"<h5 style=\"font-size: 14px; color:#444;margin-top:15px\"><b>{payer.first_name}</b></h5>");
            table.Append("</td>");

            table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
            table.Append($"<h5 style=\"font-size: 14px; color:#444;margin-top:15px\"><b>{payer.phone}</b></h5>");
            table.Append("</td>");

            table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
            table.Append($"<h5 style=\"font-size: 14px; color:#444;margin-top:15px\"><b>{payer.country_code}</b></h5>");
            table.Append("</td>");

            table.Append("</tr>");

            return table.ToString();
        }
    }
}
