using Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Orders.Domain.Messaging.Email
{
    public static class DisputeTemplate
    {
        public static string GetDisputeBody(string externalId, string summary, string reason, string amount)
        {
            var template = GetTemplateAsync("Orders.Domain.Messaging.Email.Templates.DisputeTemplate.html");

            template = template.Replace("#{{tableOrders}}", GetTableDispute(externalId, summary, reason, amount));

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

        private static string GetTableDispute(string externalId, string summary, string reason, string amount)
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
            table.Append($"<h5 style=\"font-size: 14px; color:#444;margin-top: 10px;\">{reason}</h5>");
            table.Append("</td>");

            table.Append("<td valign=\"top\" style=\"padding-left: 15px; \">");
            table.Append($"<h5 style=\"font-size: 14px; color:#444;margin-top:15px\"><b>${amount}</b></h5>");
            table.Append("</td>");

            table.Append("</tr>");

            return table.ToString();
        }

    }
}
