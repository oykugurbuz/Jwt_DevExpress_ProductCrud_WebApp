//using DevExpress.XtraReports.UI;
//using System.Text.Json;
//using WebAppDemo.Models.ReportModels;
//using WebAppDemo.Report;
//using DevExpress.XtraReports.Web.WebDocumentViewer;

//namespace WebAppDemo.Services
//{
//    public class CustomReportProvider : IWebDocumentViewerReportResolver //IWebDocumentViewerReportResolver interface’inden türeyen bir sınıf tanımlıyoruz. Bu interface, Resolve isimli bir metot içerir ve Viewer bir rapor istediğinde bu metot çağrılır.
//    {
//        private readonly IHttpContextAccessor _httpContextAccessor;

//        public CustomReportProvider(IHttpContextAccessor httpContextAccessor)
//        {
//            _httpContextAccessor = httpContextAccessor; //IHttpContextAccessor kullanarak Session verisine ulaşır.çünkü controller dışında httpcontext e ulaşmak mümkün değil
//        }
//        // hangi raporun gösterileceğini ve verilerin nereden alınacağını gösteren özelleşmiş sınıf
//        public XtraReport Resolve(string reportName)
//        {
//            if (reportName == "ProductReport")
//            {
//                var context = _httpContextAccessor.HttpContext;
//                var sessionData = context?.Session.GetString("ReportData"); //Session'dan "ReportData" anahtarı ile veriyi alıyoruz daha önce serialize edilmiş 

//                if (!string.IsNullOrEmpty(sessionData))
//                {
//                    var data = JsonSerializer.Deserialize<List<ReportViewModel>>(sessionData);
//                    var report = new ProductReport();
//                    report.DataSource = data;
//                    return report; //deserialize edip 
//                }
//                else
//                {
//                    return new ProductReport();
//                }
//            }

//            return null;
//        }
//    }
//}

using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using System.Text.Json;
using WebAppDemo.Models.ReportModels;
using WebAppDemo.Report;

namespace WebAppDemo.Services
{
    public class CustomReportProvider : IWebDocumentViewerReportResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomReportProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public XtraReport Resolve(string reportName)
        {
            if (reportName == "ProductReport")
            {
                var context = _httpContextAccessor.HttpContext;
                var sessionData = context?.Session.GetString("FilteredData");

                if (!string.IsNullOrEmpty(sessionData))
                {
                    var data = JsonSerializer.Deserialize<List<ReportViewModel>>(sessionData);
                    var report = new ProductReport
                    {
                        DataSource = data
                    };
                    return report;
                }
                else
                {
                    return new ProductReport(); // boş da olsa rapor döndür
                }
            }

            throw new ArgumentException("Geçersiz rapor adı", nameof(reportName));
        }
    }
}
