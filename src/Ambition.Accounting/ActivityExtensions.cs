using System.Diagnostics;

using Ambition.Accounting.Invoices;
using Ambition.Domain;

namespace Ambition.Accounting
{
    internal static class ActivityExtensions
    {
        public static void EnrichWithMaintenancePlan(this Activity activity, MaintenancePlanCreated maintenancePlan)
        {
            activity.SetTag(DiagnosticsNames.MaintenancePlanId, maintenancePlan.Id);
            activity.SetTag(DiagnosticsNames.MaintenancePlanProductId, maintenancePlan.ProductId);
            activity.SetTag(DiagnosticsNames.MaintenancePlanCustomerId, maintenancePlan.CustomerId);
            activity.SetTag(DiagnosticsNames.UserName, maintenancePlan.CreatedBy);
        }

        public static void EnrichWithInvoice(this Activity activity, Invoice invoice)
        {
            activity.SetTag(DiagnosticsNames.InvoiceId, invoice.Id);
            activity.SetTag(DiagnosticsNames.InvoiceNumber, invoice.Number);
            activity.SetTag(DiagnosticsNames.InvoiceCustomerId, invoice.CustomerId);
        }
    }
}
