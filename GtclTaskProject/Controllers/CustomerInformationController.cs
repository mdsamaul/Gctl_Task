using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using GtclTaskProject.Models;
using System.Drawing;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace GtclTaskProject.Controllers
{
    public class CustomerInformationController : Controller
    {
        
        public IActionResult Index()
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            var customer = _db.Customers.Include(d => d.DeliveryAddresses).ToList();
            return View(customer);
        }

        public IActionResult PreviewCustomer(int id)
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            var customer = _db.Customers.FirstOrDefault(c => c.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            return PartialView("_CustomerPreview", customer);
        }

        #region CSV Export
        [HttpPost]
        public IActionResult ExportToCsv()
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            var customers = _db.Customers.ToList();
            StringBuilder csv = new StringBuilder();

            // Add headers
            csv.AppendLine("Customer Name,Address,Business Start,Customer Type,Phone,Email,Credit Limit");

            // Add data rows
            foreach (var customer in customers)
            {
                // Use quotes around fields to handle commas within data
                csv.AppendLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\"",
                    customer.CustomerName,
                    customer.Address,
                    customer.BusinessStart?.ToString("yyyy-MM-dd"),
                    customer.CustomerType?.ToString(),
                    customer.Phone,
                    customer.Email,
                    customer.CreditLimit));
            }

            byte[] bytes = Encoding.UTF8.GetBytes(csv.ToString());
            string fileName = $"Customer_Data_{DateTime.Now:yyyy-MM-dd}.csv";
            return File(bytes, "text/csv", fileName);
        }
        #endregion

        #region Excel Export (XML-Based)
        [HttpPost]
        public IActionResult ExportToExcel()
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            var customers = _db.Customers.ToList();

            // Generate Excel XML
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (XmlTextWriter xw = new XmlTextWriter(sw))
            {
                xw.WriteStartDocument();
                xw.WriteProcessingInstruction("mso-application", "progid=\"Excel.Sheet\"");

                // Start Workbook
                xw.WriteStartElement("Workbook");
                xw.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:spreadsheet");
                xw.WriteAttributeString("xmlns:o", "urn:schemas-microsoft-com:office:office");
                xw.WriteAttributeString("xmlns:x", "urn:schemas-microsoft-com:office:excel");
                xw.WriteAttributeString("xmlns:ss", "urn:schemas-microsoft-com:office:spreadsheet");

                // Styles
                xw.WriteStartElement("Styles");

                // Default style
                xw.WriteStartElement("Style");
                xw.WriteAttributeString("ss:ID", "Default");
                xw.WriteAttributeString("ss:Name", "Normal");
                xw.WriteStartElement("Alignment");
                xw.WriteAttributeString("ss:Vertical", "Center");
                xw.WriteEndElement(); // Alignment
                xw.WriteEndElement(); // Style

                // Header style
                xw.WriteStartElement("Style");
                xw.WriteAttributeString("ss:ID", "Header");
                xw.WriteStartElement("Font");
                xw.WriteAttributeString("ss:Bold", "1");
                xw.WriteEndElement(); // Font
                xw.WriteStartElement("Interior");
                xw.WriteAttributeString("ss:Color", "#D3D3D3");
                xw.WriteAttributeString("ss:Pattern", "Solid");
                xw.WriteEndElement(); // Interior
                xw.WriteEndElement(); // Style

                xw.WriteEndElement(); // Styles

                // Worksheet
                xw.WriteStartElement("Worksheet");
                xw.WriteAttributeString("ss:Name", "Customers");

                // Table
                xw.WriteStartElement("Table");

                // Header Row
                xw.WriteStartElement("Row");
                xw.WriteAttributeString("ss:StyleID", "Header");

                WriteExcelCell(xw, "Customer Name");
                WriteExcelCell(xw, "Address");
                WriteExcelCell(xw, "Business Start");
                WriteExcelCell(xw, "Customer Type");
                WriteExcelCell(xw, "Phone");
                WriteExcelCell(xw, "Email");
                WriteExcelCell(xw, "Credit Limit");

                xw.WriteEndElement(); // Row

                // Data Rows
                foreach (var customer in customers)
                {
                    xw.WriteStartElement("Row");

                    WriteExcelCell(xw, customer.CustomerName ?? "");
                    WriteExcelCell(xw, customer.Address ?? "");
                    WriteExcelCell(xw, customer.BusinessStart?.ToString("yyyy-MM-dd") ?? "");
                    WriteExcelCell(xw, customer.CustomerType?.ToString() ?? "");
                    WriteExcelCell(xw, customer.Phone ?? "");
                    WriteExcelCell(xw, customer.Email ?? "");
                    WriteExcelCell(xw, customer.CreditLimit?.ToString() ?? "");

                    xw.WriteEndElement(); // Row
                }

                xw.WriteEndElement(); // Table

                // WorksheetOptions
                xw.WriteStartElement("WorksheetOptions");
                xw.WriteAttributeString("xmlns", "urn:schemas-microsoft-com:office:excel");
                xw.WriteStartElement("PageSetup");
                xw.WriteStartElement("Layout");
                xw.WriteAttributeString("x:Orientation", "Landscape");
                xw.WriteEndElement(); // Layout
                xw.WriteEndElement(); // PageSetup
                xw.WriteEndElement(); // WorksheetOptions

                xw.WriteEndElement(); // Worksheet
                xw.WriteEndElement(); // Workbook
            }

            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            string fileName = $"Customer_Data_{DateTime.Now:yyyy-MM-dd}.xls";
            // Fix Excel download by specifying correct content type
            return File(bytes, "application/vnd.ms-excel", fileName);
        }

        private void WriteExcelCell(XmlTextWriter xw, string value)
        {
            xw.WriteStartElement("Cell");
            xw.WriteStartElement("Data");
            xw.WriteAttributeString("ss:Type", "String");
            xw.WriteString(value ?? "");
            xw.WriteEndElement(); // Data
            xw.WriteEndElement(); // Cell
        }
        #endregion

        #region Word Export (HTML-Based)
        [HttpPost]
        public IActionResult ExportToWord()
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            var customers = _db.Customers.ToList();

            // Build HTML content
            StringBuilder sb = new StringBuilder();

            // HTML styling
            sb.Append(@"
                <html xmlns:o='urn:schemas-microsoft-com:office:office' 
                      xmlns:w='urn:schemas-microsoft-com:office:word'
                      xmlns='http://www.w3.org/TR/REC-html40'>
                <head>
                <meta charset='utf-8'>
                <title>Customer Data</title>
                <style>
                    table {
                        border-collapse: collapse;
                        width: 100%;
                        margin: 20px 0;
                    }
                    th, td {
                        border: 1px solid #ddd;
                        padding: 8px;
                        text-align: left;
                    }
                    th {
                        background-color: #f2f2f2;
                        font-weight: bold;
                    }
                    h1 {
                        text-align: center;
                    }
                    .date {
                        text-align: right;
                        font-style: italic;
                        margin-bottom: 20px;
                    }
                </style>
                </head>
                <body>");

            // Document title
            sb.Append("<h1>Customer Data</h1>");
            sb.Append($"<p class='date'>Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");

            // Table start
            sb.Append("<table>");

            // Headers
            sb.Append("<tr>");
            sb.Append("<th>Customer Name</th>");
            sb.Append("<th>Address</th>");
            sb.Append("<th>Business Start</th>");
            sb.Append("<th>Customer Type</th>");
            sb.Append("<th>Phone</th>");
            sb.Append("<th>Email</th>");
            sb.Append("<th>Credit Limit</th>");
            sb.Append("</tr>");

            // Data rows
            foreach (var customer in customers)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{customer.CustomerName}</td>");
                sb.Append($"<td>{customer.Address}</td>");
                sb.Append($"<td>{customer.BusinessStart:yyyy-MM-dd}</td>");
                sb.Append($"<td>{customer.CustomerType}</td>");
                sb.Append($"<td>{customer.Phone}</td>");
                sb.Append($"<td>{customer.Email}</td>");
                sb.Append($"<td>{customer.CreditLimit}</td>");
                sb.Append("</tr>");
            }

            // Table end
            sb.Append("</table>");

            // Document end
            sb.Append("</body></html>");

            // Convert to bytes
            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            string fileName = $"Customer_Data_{DateTime.Now:yyyy-MM-dd}.doc";
            return File(bytes, "application/msword", fileName);
        }
        #endregion

        #region PDF Export (Dynamic HTML to PDF)
        [HttpPost]
        public IActionResult ExportToPdf()
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            var customers = _db.Customers.ToList();

            StringBuilder sb = new StringBuilder();

            // HTML styling for PDF optimized view
            sb.Append(@"
                <!DOCTYPE html>
                <html>
                <head>
                <meta charset='utf-8'>
                <title>Customer Data (PDF Export)</title>
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        font-size: 12pt;
                    }
                    table {
                        border-collapse: collapse;
                        width: 100%;
                        margin: 20px 0;
                        page-break-inside: auto;
                    }
                    tr {
                        page-break-inside: avoid;
                        page-break-after: auto;
                    }
                    th, td {
                        border: 1px solid #000;
                        padding: 8px;
                        text-align: left;
                    }
                    th {
                        background-color: #f2f2f2;
                        font-weight: bold;
                    }
                    h1 {
                        text-align: center;
                    }
                    .date {
                        text-align: right;
                        font-style: italic;
                        margin-bottom: 20px;
                    }
                    @media print {
                        body {
                            margin: 0;
                            padding: 15mm;
                        }
                    }
                    .no-print {
                        display: block;
                    }
                    @media print {
                        .no-print {
                            display: none;
                        }
                    }
                </style>
                <script>
                    window.onload = function() {
                        // Auto trigger print dialog and then download
                        window.print();
                        
                        // Create a fake download link to trigger download
                        setTimeout(function() {
                            var downloadBtn = document.getElementById('downloadPdf');
                            if (downloadBtn) {
                                downloadBtn.click();
                            }
                        }, 1000);
                    }
                </script>
                </head>
                <body>");

            // Add hidden download button that will be triggered by JavaScript
            sb.Append(@"
                <div class='no-print'>
                    <button onclick='window.print()'>Print Again</button>
                    <a id='downloadPdf' href='javascript:window.print()' style='display:none;'>Download PDF</a>
                </div>");

            // Document title
            sb.Append("<h1>Customer Data</h1>");
            sb.Append($"<p class='date'>Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");

            // Table start
            sb.Append("<table>");

            // Headers
            sb.Append("<tr>");
            sb.Append("<th>Customer Name</th>");
            sb.Append("<th>Address</th>");
            sb.Append("<th>Business Start</th>");
            sb.Append("<th>Customer Type</th>");
            sb.Append("<th>Phone</th>");
            sb.Append("<th>Email</th>");
            sb.Append("<th>Credit Limit</th>");
            sb.Append("</tr>");

            // Data rows
            foreach (var customer in customers)
            {
                sb.Append("<tr>");
                sb.Append($"<td>{customer.CustomerName}</td>");
                sb.Append($"<td>{customer.Address}</td>");
                sb.Append($"<td>{customer.BusinessStart:yyyy-MM-dd}</td>");
                sb.Append($"<td>{customer.CustomerType}</td>");
                sb.Append($"<td>{customer.Phone}</td>");
                sb.Append($"<td>{customer.Email}</td>");
                sb.Append($"<td>{customer.CreditLimit}</td>");
                sb.Append("</tr>");
            }

            // Table end
            sb.Append("</table>");

            // Document end
            sb.Append("</body></html>");

            // Return as HTML content with PDF print trigger
            return Content(sb.ToString(), "text/html");
        }
        #endregion

        [HttpPost]
        public IActionResult ExportJson()
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            var customers = _db.Customers.ToList();
            var customerData = customers.Select(c => new {
                CustomerName = c.CustomerName,
                Address = c.Address,
                BusinessStart = c.BusinessStart?.ToString("yyyy-MM-dd"),
                CustomerType = c.CustomerType?.ToString(),
                Phone = c.Phone,
                Email = c.Email,
                CreditLimit = c.CreditLimit
            }).ToList();

            string json = System.Text.Json.JsonSerializer.Serialize(customerData,
                new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true
                });

            byte[] bytes = Encoding.UTF8.GetBytes(json);
            string fileName = $"Customer_Data_{DateTime.Now:yyyy-MM-dd}.json";
            return File(bytes, "application/json", fileName);
        }
    }
}