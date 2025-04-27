using DinkToPdf;
using DinkToPdf.Contracts;

namespace GctlInfoSysTask.Services
{
    public class PdfGeneratorService
    {
        private readonly IConverter _converter;

        public PdfGeneratorService(IConverter converter)
        {
            _converter = converter;
        }
        //public byte[] GgeneratePdfFromHtml(string htmlContent)
        //{
        //    var doc = new HtmlToPdfDocument()
        //    {
        //        GlobalSettings =
        //        {
        //            PaperSize= PaperKind.A4,
        //            Orientation=Orientation.Portrait,
        //        },
        //        Objects =
        //        {
        //            new ObjectSettings()
        //            {
        //                HtmlContent = htmlContent,
        //            }
        //        }
        //    };
        //    return _converter.Convert(doc);
        //}
        //public byte[] GgeneratePdfFromHtml(string htmlContent)
        //{
        //    var doc = new HtmlToPdfDocument()
        //    {
        //        GlobalSettings = new GlobalSettings
        //        {
        //            PaperSize = PaperKind.A4,
        //            Orientation = Orientation.Portrait,
        //        },
        //        Objects =
        //{
        //    new ObjectSettings()
        //    {
        //        HtmlContent = htmlContent,
        //        FooterSettings = new FooterSettings
        //        {
        //            FontSize = 9,
        //            Right = "Page [page] of [toPage]", 
        //            Line = true,
        //            Spacing = 2.812
        //        }
        //    }
        //}
        //    };

        //    return _converter.Convert(doc);
        //}

        public byte[] GgeneratePdfFromHtml(string htmlContent)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                },
                Objects =
        {
            new ObjectSettings()
            {
                HtmlContent = htmlContent,
                FooterSettings = new FooterSettings
                {
                    FontSize = 9,
                    Right = "Page [page] of [toPage]",
                    Line = true,
                    Spacing = 2.812
                }
            }
        }
            };

            return _converter.Convert(doc);
        }

    }
}
