using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using GctlInfoSysTask.Models;
using System.Drawing;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using GctlInfoSysTask.Data;
using GctlInfoSysTask.Services;
using GctlInfoSysTask.ModelDto;

namespace GtclTaskProject.Controllers
{
    public class CustomerInformationController : Controller
    {
        //private readonly GctlinfoExamTest2025Context _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _context;
        private readonly PdfGeneratorService _pdfGeneratorService;

        public CustomerInformationController( IWebHostEnvironment webHostEnvironment, AppDbContext context, PdfGeneratorService pdfGeneratorService)
        {

            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _pdfGeneratorService = pdfGeneratorService;
        }

        public IActionResult Index()
        {
            var customer = _context.Customers.Include(d => d.DeliveryAddresses).Include(ct=>ct.CustomerType).ToList();
            return View(customer);
        }
        public IActionResult PreviewCustomer(int id)
        {

            var customer = _context.Customers.FirstOrDefault(c => c.AI_ID == id);
            if (customer == null)
            {
                return NotFound();
            }
            return PartialView("_CustomerPreview", customer);
        }       
        public IActionResult DownloadPdfA4(List<int> ids)
        {
            var customers = _context.Customers
                                    .Where(c => ids.Contains(c.AI_ID))
                                    .Include(da => da.DeliveryAddresses)
                                    .Include(ct => ct.CustomerType)
                                    .ToList();
            string html = GenerateA4Html(customers);
            var pdfBytes = _pdfGeneratorService.GgeneratePdfFromHtml(html);
            return File(pdfBytes, "application/pdf", "SelectedCustomer.pdf");
        }

        private string GenerateA4Html(List<Customer> customers)
        {
            string host = $"{Request.Scheme}://{Request.Host}";
            var html = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <title>Customer Information</title>
    <style>
        @page {
            size: A4;
            margin: 10mm;
        }

        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: 'Segoe UI', Arial, sans-serif;
            background-color: #fff;
            color: #333;
            margin: 0;
            padding: 0;
            width: 100%;
        }

        .admit-card {
            width: 210mm; /* A4 width */
            height: 297mm; /* A4 height */
            padding: 10mm;
            margin: 0 auto;
            background: white;
            position: relative;
            page-break-after: always;
        }

        .header {
            position: relative;
            padding-bottom: 15px;
            border-bottom: 3px solid #0056b3;
            margin-bottom: 20px;
            height: 120px;
            width: 100%;
        }

        .logo {
            position: absolute;
            top: 0;
            left: 0;
            width: 100px;
            height: 100px;
            display: flex;
            align-items: center;
            justify-content: center;
            background-color: #f9f9f9;
        }

        .card-title-section {
            text-align: center;
            margin-left: 110px;
            margin-right: 130px;
        }

        .card-title {
            font-size: 24px;
            font-weight: bold;
            color: #0056b3;
        }

        .card-subtitle {
            font-size: 16px;
            color: #666;
            margin: 5px 0;
        }

        .card-id {
            font-size: 14px;
            font-weight: bold;
        }

        .photo-container {
            position: absolute;
            top: 0;
            right: 0;
            width: 120px;
            height: 140px;
            border: 2px solid #0056b3;
            padding: 3px;
            background-color: #fff;
        }

        .photo-container img {
            width: 100%;
            height: 100%;
            object-fit: cover;
        }

        .section {
            margin-bottom: 20px;
            width: 100%;
        }

        .section-title {
            font-size: 18px;
            font-weight: bold;
            color: #0056b3;
            margin-bottom: 10px;
            padding-bottom: 5px;
            border-bottom: 1px solid #ccc;
        }

        .info-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 10px;
            width: 100%;
        }

        .info-item {
            margin-bottom: 5px;
        }

        .label {
            font-weight: bold;
            margin-right: 5px;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 10px;
            font-size: 14px;
        }

        th, td {
            border: 1px solid #000;
            padding: 8px;
            text-align: left;
        }

        thead tr {
            background-color: #eee;
        }

        tbody tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        .instructions {
            margin-top: 20px;
            border: 1px solid #ccc;
            padding: 10px;
            background-color: #f9f9f9;
        }

        .instructions-title {
            font-weight: bold;
            margin-bottom: 5px;
        }

        .instructions-list {
            padding-left: 20px;
        }

        .barcode {
            text-align: center;
            margin: 20px 0;
            font-family: 'Courier New', monospace;
            font-size: 16px;
            letter-spacing: 2px;
        }

        .footer {
            display: flex;
            justify-content: space-between;
            margin-top: 30px;
        }

        .signature {
            width: 45%;
        }

        .signature-line {
            border-top: 1px solid #000;
            padding-top: 5px;
            text-align: center;
            font-size: 12px;
        }
    </style>
</head>
<body>";

            // Process each customer
            for (int i = 0; i < customers.Count; i++)
            {
                var customer = customers[i];

                // Prepare image path if photo exists
                string imagePath = !string.IsNullOrEmpty(customer.Photo)
                    ? $"{host}/images/{customer.Photo}"
                    : $"{host}/images/placeholder.jpg"; // Use a placeholder if no photo exists

                html += $@"
<div class='admit-card'>
    <div class='header'>
        <div class='logo'>
            <!-- Logo placeholder -->
            <span>LOGO</span>
        </div>
        <div class='card-title-section'>
            <div class='card-title'>Customer Card</div>
            <div class='card-subtitle'>Customer Information Document</div>
            <div class='card-id'>ID: {customer.CustomerId}</div>
        </div>
        <div class='photo-container'>
            <img src='{imagePath}' alt='{customer.CustomerName}' />
        </div>
    </div>

    <div class='section'>
        <div class='section-title'>Personal Information</div>
        <div class='info-grid'>
            <div class='info-item'>
                <span class='label'>Customer ID:</span>
                <span class='value'>{customer.CustomerId}</span>
            </div>
            <div class='info-item'>
                <span class='label'>Full Name:</span>
                <span class='value'>{customer.CustomerName}</span>
            </div>
            <div class='info-item'>
                <span class='label'>Address:</span>
                <span class='value'>{customer.Address}</span>
            </div>
            <div class='info-item'>
                <span class='label'>Business Start:</span>
                <span class='value'>{customer.BusinessStart?.ToString("dd MMM yyyy")}</span>
            </div>
            <div class='info-item'>
                <span class='label'>Customer Type :</span>
                <span class='value'>{(customer.CustomerType != null ? customer.CustomerType.CustomerTypeName : customer.CustomerTypeId.ToString())}</span>
            </div>
            <div class='info-item'>
                <span class='label'>Credit Limit:</span>
                <span class='value'>{customer.CreditLimit}</span>
            </div>
        </div>
    </div>

    <div class='section'>
        <div class='section-title'>Contact Information</div>
        <div class='info-grid'>
            <div class='info-item'>
                <span class='label'>Phone:</span>
                <span class='value'>{customer.Phone}</span>
            </div>
            <div class='info-item'>
                <span class='label'>Email:</span>
                <span class='value'>{customer.Email}</span>
            </div>
        </div>
    </div>";

                if (customer.DeliveryAddresses != null && customer.DeliveryAddresses.Any())
                {
                    html += @"
<div class='section'>
    <div class='section-title'>Delivery Addresses</div>
    <table>
        <thead>
            <tr>
                <th>Delivery Address</th>
                <th>Contact Person</th>
                <th>Phone</th>
            </tr>
        </thead>
        <tbody>";

                    foreach (var address in customer.DeliveryAddresses)
                    {
                        html += @"<tr>";
                        html += $"<td>{address.DeliveryAddressLine}</td>";
                        html += $"<td>{address.ContactPerson}</td>";
                        html += $"<td>{address.Phone}</td>";
                        html += @"</tr>";
                    }

                    html += @"
        </tbody>
    </table>
</div>";
                }

                html += @"
<div class='instructions'>
    <div class='instructions-title'>Important Instructions:</div>
    <ol class='instructions-list'>
        <li>This card must be presented during all business transactions.</li>
        <li>This card is not transferable and is valid only for the named customer.</li>
        <li>In case of any discrepancy in the information, please contact our office immediately.</li>
        <li>Please keep this card safe and secure.</li>
    </ol>
</div>

<div class='barcode'>
    **** " + customer.CustomerId + @" ****
</div>

<div class='footer'>
    <div class='signature'>
        <div class='signature-line'>Customer's Signature</div>
    </div>
    <div class='signature'>
        <div class='signature-line'>Authorized Signature</div>
    </div>
</div>
</div>";

                // Only add page break if it's not the last customer
                // We don't need to add a page break after the last customer
            }

            html += "</body></html>";
            return html;
        }
                public IActionResult DownloadSelectedPdf(List<int> ids)
        {
            var customer = _context.Customers.Where(c => ids.Contains(c.AI_ID)).Include(d=>d.DeliveryAddresses).Include(ct=>ct.CustomerType).ToList();
            string html = GenerateHtml(customer);
            var pdfBytes = _pdfGeneratorService.GgeneratePdfFromHtml(html);
            return File(pdfBytes, "application/pdf", "SelectedCustomer.pdf");
        }

        



        private string GenerateHtml(List<Customer> customers)
        {
            string host = $"{Request.Scheme}://{Request.Host}";
            int serial = 1;

            var html = @"
<html>
<head>
    <style>
        body {
            font-family: Arial, sans-serif;
            padding: 20px;
            background-color: #fafafa;
        }

        h2 {
            text-align: center;
            color: #333;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
            background-color: white;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }

        th, td {
            border: 1px solid #ddd;
            padding: 10px;
            text-align: left;
        }

        th {
            background-color: #f5f5f5;
            color: #333;
        }

        tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        tr:hover {
            background-color: #f1f1f1;
        }

        img {
            display: block;
            margin: auto;
            border-radius: 5px;
            border: 1px solid #ccc;
        }

        hr {
            background-color: red;
            height: 5px;
            width: 500px;
            border: none;
            margin: 0 auto 20px auto;
        }

        .inner-table {
            width: 100%;
            border-collapse: collapse;
        }

        .inner-table th, .inner-table td {
            border: 1px solid #bbb;
            padding: 5px;
            font-size: 13px;
        }

        .inner-table th {
            background-color: #f0f0f0;
        }

        .no-delivery {
            color: #999;
            font-style: italic;
        }
    </style>
</head>
<body>
    <h2>Customer Report</h2>
    <hr/>
    <table>
        <thead>
            <tr>
                <th>Sl No.</th>
                <th>Customer ID</th>
                <th>Customer Name</th>
                <th>Customer Address</th>
                <th>Customer Type</th>
                <th>Business Start</th>
                <th>Phone</th>
                <th>Email</th>
                <th>Credit Limit</th>
                <th>Photo</th>
                <th>Delivery Address Info</th>
            </tr>
        </thead>
        <tbody>";

            foreach (var customer in customers)
            {
                string imagePath = $"{host}/images/{customer.Photo}";
                string deliveryInfo = "";

                if (customer.DeliveryAddresses != null && customer.DeliveryAddresses.Any())
                {
                    deliveryInfo += @"
            <table class='inner-table'>
                <thead>
                    <tr>
                        <th>Delivery Address</th>
                        <th>Contact Person</th>
                        <th>Phone</th>
                    </tr>
                </thead>
                <tbody>";

                    foreach (var address in customer.DeliveryAddresses)
                    {
                        deliveryInfo += $@"
                    <tr>
                        <td>{address.DeliveryAddressLine}</td>
                        <td>{address.ContactPerson}</td>
                        <td>{address.Phone}</td>
                    </tr>";
                    }

                    deliveryInfo += @"
                </tbody>
            </table>";
                }
                else
                {
                    deliveryInfo = "<div class='no-delivery'>No delivery address available</div>";
                }

                html += $@"
            <tr>
                <td>{serial++}</td>
                <td>{customer.CustomerId}</td>
                <td>{customer.CustomerName}</td>
                <td>{customer.Address}</td>
                <td>{customer.CustomerType?.CustomerTypeName}</td>
                <td>{customer.BusinessStart:yyyy-MM-dd}</td>
                <td>{customer.Phone}</td>
                <td>{customer.Email}</td>
                <td>{customer.CreditLimit}</td>
                <td><img src='{imagePath}' width='50' height='50'/></td>
                <td>{deliveryInfo}</td>
            </tr>";
            }

            html += @"
        </tbody>
    </table>
</body>
</html>";

            return html;
        }




    }
}