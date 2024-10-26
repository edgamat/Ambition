using System.Diagnostics;

using Ambition.Accounting.Invoices;
using Ambition.Domain;

namespace Ambition.Accounting
{
    internal static class ActivityExtensions
    {
        public static void EnrichWithMaintenancePlan(this Activity activity, MaintenancePlanCreated maintenancePlan)
        {
            activity.SetTag(DiagnosticNames.MaintenancePlanId, maintenancePlan.Id);
            activity.SetTag(DiagnosticNames.MaintenancePlanProductId, maintenancePlan.ProductId);
            activity.SetTag(DiagnosticNames.MaintenancePlanCustomerId, maintenancePlan.CustomerId);
            activity.SetTag(DiagnosticNames.UserName, maintenancePlan.CreatedBy);
        }

        public static void EnrichWithInvoice(this Activity activity, Invoice invoice)
        {
            activity.SetTag(DiagnosticNames.InvoiceId, invoice.Id);
            activity.SetTag(DiagnosticNames.InvoiceNumber, invoice.Number);
            activity.SetTag(DiagnosticNames.InvoiceCustomerId, invoice.CustomerId);
        }
    }
}
