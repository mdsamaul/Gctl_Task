using GctlInfoSysTask.Data;
using GctlInfoSysTask.ModelDto;
using GctlInfoSysTask.Models;
using GctlInfoSysTask.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace GctlInfoSysTask.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly PdfGeneratorService _pdfGeneratorService;

        public CustomerController(AppDbContext context, IWebHostEnvironment hostEnvironment, PdfGeneratorService pdfGeneratorService)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _pdfGeneratorService = pdfGeneratorService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["FormAction"] = "Create";
            // Fetch customers from the database
            var customers = await _context.Customers!
                .Include(d => d.DeliveryAddresses).Include(ct=>ct.CustomerType)
                .ToListAsync();
            ViewBag.CustomerType = new SelectList(_context.CustomerTypes, "CustomerTypeId", "CustomerTypeName");
            //ViewBag.CustomerId = "CUS-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            var nextCustomerId = 1;
            var lastCustomer = _context.Customers.OrderByDescending(a=>a.AI_ID).FirstOrDefault();
            if(lastCustomer != null)
            {
                string lastId = lastCustomer.CustomerId.Substring(4);
                nextCustomerId = int.Parse(lastId) + 1;
            }

            string newCustomerId = "GCTL" + nextCustomerId.ToString().PadLeft(8, '0');
            ViewBag.CustomerId = newCustomerId;
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            // Convert Customer list to CustomerDto list
            var customerDtos = customers.Select(c => new CustomerDto
            {
                AI_ID = c.AI_ID,
                CustomerId = c.CustomerId,
                CustomerName = c.CustomerName,
                Address = c.Address,
                BusinessStart = c.BusinessStart,
                CustomerTypeId = c.CustomerTypeId,
                Phone = c.Phone,
                Email = c.Email,
                CreditLimit = c.CreditLimit,
                Photo = c.Photo,
                DeliveryAddresses = c.DeliveryAddresses // Include related delivery addresses if needed
            }).ToList();

            return View(customers);  // Pass CustomerDto to the view
        }
       

        public IActionResult Create()
        {
            ViewData["FormAction"] = "Create";
            //ViewBag.CustomerId = "CUS-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            ViewBag.CustomerType = new SelectList(_context.CustomerTypes, "CustomerTypeId", "CustomerTypeName");
            return View(new CustomerDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerDto customerDto)
        {
            //if (ModelState.IsValid)
            //{
            //if(customerDto.CustomerName == null)
            //{
            //    return Json()
            //}
            ViewBag.CustomerType = new SelectList(_context.CustomerTypes, "CustomerTypeId", "CustomerTypeName");
            if (string.IsNullOrWhiteSpace(customerDto.CustomerName))
                {
                    ModelState.AddModelError("CustomerName", "Please Enter Customer Name");
                    ViewData["FormAction"] = "Create";
                    return View("_CustomerForm", customerDto);
                }
                if (string.IsNullOrWhiteSpace(customerDto.Address))
                {
                    ModelState.AddModelError("Address", "Please Enter Address");
                    ViewData["FormAction"] = "Create";
                    return View("_CustomerForm", customerDto);
                }
                
            var exists = await _context.Customers.AnyAsync(c => c.CustomerName == customerDto.CustomerName && c.Address == customerDto.Address);
            if (exists)
            {
                ModelState.AddModelError("ExistsCustomer","Customer Alredy Exsist");
                ViewData["FormAction"] = "Create";
                return View("_CustomerForm", customerDto);
            }
                // Handle photo upload
                if (customerDto.PhotoFile != null && customerDto.PhotoFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "Images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + customerDto.PhotoFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await customerDto.PhotoFile.CopyToAsync(fileStream);
                    }

                    customerDto.Photo = uniqueFileName;
                }

                // Map DTO to actual model
                var customer = new Customer
                {
                    CustomerId = customerDto.CustomerId,
                    CustomerName = customerDto.CustomerName,
                    Address = customerDto.Address,
                    BusinessStart = customerDto.BusinessStart,
                    CustomerTypeId = customerDto.CustomerTypeId,
                    Phone = customerDto.Phone,
                    Email = customerDto.Email,
                    CreditLimit = customerDto.CreditLimit,
                    Photo = customerDto.Photo,
                    DeliveryAddresses = customerDto.DeliveryAddresses?.Select(a => new DeliveryAddress
                    {
                        DeliveryAddressLine = a.DeliveryAddressLine,
                        ContactPerson = a.ContactPerson,
                        Phone = a.Phone
                    }).ToList()
                };

                // Save to DB
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            //TempData["SuccessMessage"] = "Customer created successfully!";
            // Set success message with icon
            //TempData["SuccessMessage"] = "<i class='bi bi-check-circle'></i> Customer created successfully!";
            TempData["SuccessMessage"] = "Customer created successfully!";
            return RedirectToAction(nameof(Index));
            //}

            //ViewData["FormAction"] = "Create";
            //ViewBag.CustomerType = new SelectList(_context.CustomerTypes, "CustomerTypeId", "CustomerTypeName");
            //return View("_CustomerForm", customerDto);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.DeliveryAddresses)
                .FirstOrDefaultAsync(c => c.AI_ID == id);
            var dAddress = _context.DeliveryAddresses.Where(c=>c.CustomerId==customer.AI_ID).ToList();
            if (customer == null)
            {
                return NotFound();
            }
            ViewBag.CustomerType = new SelectList(_context.CustomerTypes, "CustomerTypeId", "CustomerTypeName");
            // Map model to DTO
            var customerDto = new CustomerDto
            {
                AI_ID = customer.AI_ID,
                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,
                Address = customer.Address,
                BusinessStart = customer.BusinessStart,
                CustomerTypeId = customer.CustomerTypeId,
                Phone = customer.Phone,
                Email = customer.Email,
                CreditLimit = customer.CreditLimit,
                Photo = customer.Photo,
                DeliveryAddresses = dAddress
            };

            ViewData["FormAction"] = "Edit";
            ViewBag.CustomerType = new SelectList(_context.CustomerTypes, "CustomerTypeId", "CustomerTypeName", customer.CustomerTypeId);
            return View("_CustomerForm", customerDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerDto customerDto, int[] dAID)
        {
            //if (ModelState.IsValid)
            //{
            //    try
            //    {


                    // Get existing customer
                    var customer = await _context.Customers
                        .Include(c => c.DeliveryAddresses)
                        .FirstOrDefaultAsync(c => c.AI_ID == customerDto.AI_ID);

                    if (customer == null)
                    {
                        return NotFound();
                    }


            if (string.IsNullOrWhiteSpace(customerDto.CustomerName))
            {
                ModelState.AddModelError("CustomerName", "Please Enter Customer Name");
                ViewData["FormAction"] = "Edit";
                return View("_CustomerForm", customerDto);
            }
            if (string.IsNullOrWhiteSpace(customerDto.Address))
            {
                ModelState.AddModelError("Address", "Please Enter Address");
                ViewData["FormAction"] = "Edit";
                return View("_CustomerForm", customerDto);
            }
            //var exists = await _context.Customers.AnyAsync(c => c.CustomerName == customerDto.CustomerName && c.Address == customerDto.Address);
            //if (exists)
            //{
            //    ModelState.AddModelError("ExistsCustomer", "Customer Alredy Exsist");
            //    ViewData["FormAction"] = "Edit";
            //    return View("_CustomerForm", customerDto);
            //}
            // Check for duplicate customers excluding the current one
            var exists = await _context.Customers
                .Where(c => c.AI_ID != customerDto.AI_ID) // Exclude the current customer from duplicate check
                .AnyAsync(c => c.CustomerName == customerDto.CustomerName && c.Address == customerDto.Address);

            if (exists)
            {
                ModelState.AddModelError("ExistsCustomer", "Customer already exists.");
                ViewData["FormAction"] = "Edit";
                return View("_CustomerForm", customerDto);
            }



            // Handle photo upload
            if (customerDto.PhotoFile != null && customerDto.PhotoFile.Length > 0)
                    {
                        // Delete old photo if exists
                        if (!string.IsNullOrEmpty(customer.Photo))
                        {
                            var oldPhotoPath = Path.Combine(_hostEnvironment.WebRootPath, "Images", customer.Photo);
                            if (System.IO.File.Exists(oldPhotoPath))
                            {
                                System.IO.File.Delete(oldPhotoPath);
                            }
                        }

                        // Save new photo
                        string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "Images");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + customerDto.PhotoFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await customerDto.PhotoFile.CopyToAsync(fileStream);
                        }

                        customer.Photo = uniqueFileName;
                    }

                    // Update customer properties
                    customer.CustomerName = customerDto.CustomerName;
                    customer.Address = customerDto.Address;
                    customer.BusinessStart = customerDto.BusinessStart;
                    customer.CustomerTypeId = customerDto.CustomerTypeId;
                    customer.Phone = customerDto.Phone;
                    customer.Email = customerDto.Email;
                    customer.CreditLimit = customerDto.CreditLimit;

                    // Handle delivery addresses
                    // First, remove addresses that are no longer in the DTO
                    if (customerDto.DeliveryAddresses != null)
                    {
                        // Remove existing addresses not in the updated list
                        var existingAddressIds = customer.DeliveryAddresses.Select(d => d.DeliveryAddressId).ToList();
                        var updatedAddressIds = customerDto.DeliveryAddresses
                            .Where(d => d.DeliveryAddressId > 0)
                            .Select(d => d.DeliveryAddressId)
                            .ToList();

                        var addressesToRemove = customer.DeliveryAddresses
                            .Where(d => !updatedAddressIds.Contains(d.DeliveryAddressId))
                            .ToList();

                        foreach (var address in addressesToRemove)
                        {
                            _context.DeliveryAddresses.Remove(address);
                        }

                        // Update existing addresses and add new ones
                        foreach (var addressDto in customerDto.DeliveryAddresses)
                        {
                            if (addressDto.DeliveryAddressId > 0)
                            {
                                // Update existing address
                                var existingAddress = customer.DeliveryAddresses
                                    .FirstOrDefault(d => d.DeliveryAddressId == addressDto.DeliveryAddressId);

                                if (existingAddress != null)
                                {
                                    existingAddress.DeliveryAddressLine = addressDto.DeliveryAddressLine;
                                    existingAddress.ContactPerson = addressDto.ContactPerson;
                                    existingAddress.Phone = addressDto.Phone;
                                }
                            }
                            else
                            {
                                // Add new address
                                customer.DeliveryAddresses.Add(new DeliveryAddress
                                {
                                    DeliveryAddressLine = addressDto.DeliveryAddressLine,
                                    ContactPerson = addressDto.ContactPerson,
                                    Phone = addressDto.Phone,
                                    CustomerId = customer.AI_ID
                                });
                            }
                        }
                    }

                    // Save changes
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                //}
                //catch (DbUpdateConcurrencyException)
                //{
                //    return NotFound();
                //    //if (!CustomerExists(customerDto.AI_ID))
                //    //{
                //    //    return NotFound();
                //    //}
                //    //else
                //    //{
                //    //    throw;
                //    //}
                //}
                return RedirectToAction(nameof(Index));
            //}

            //ViewData["FormAction"] = "Edit";
            //ViewBag.CustomerType = new SelectList(_context.CustomerTypes, "CustomerTypeId", "CustomerTypeName", customerDto.CustomerTypeId);
            //return View("_CustomerForm", customerDto);
        }
        public IActionResult GetCustomerTypePartial()
        {
         
            var customerTypes = _context.CustomerTypes.ToList();
            //int customerTypeId = _db.CustomerTypes.Any() ? _db.CustomerTypes.Max(c => c.Id) : 0;
            int nextId = (customerTypes.Any() ? customerTypes.Max(ct => ct.CustomerTypeId) : 0) + 1;

            ViewBag.nextCusTypeId = nextId;
            return PartialView("_GetCustomerType", customerTypes);
        }


        [HttpPost]
        public JsonResult DeleteSelectedCustomerTypes(List<int> ids)
        {
            if (ids != null && ids.Any())
            {
                var itemsToDelete = _context.CustomerTypes.Where(ct => ids.Contains(ct.CustomerTypeId)).ToList();
                _context.CustomerTypes.RemoveRange(itemsToDelete);
                _context.SaveChanges();

                return Json(new { success = true });
            }

            return Json(new { success = false, message = "No IDs provided." });
        }
       
        [HttpPost]
        public JsonResult AddCustomerType(string CustomerTypeName)
        {

            if (_context.CustomerTypes.Any(c => c.CustomerTypeName == CustomerTypeName))
            {
                return Json(new { success = false, message = "Customer Type already exists." });
            }
            var newType = new CustomerType
            {
                CustomerTypeName = CustomerTypeName
            };
            _context.CustomerTypes.Add(newType);
            _context.SaveChanges();
            var nextId = _context.CustomerTypes.Max(c => c.CustomerTypeId) + 1;
            return Json(new
            {
                success = true,
                customerType = new
                {
                    customerTypeId = newType.CustomerTypeId,
                    customerType1 = newType.CustomerTypeName
                },
                nextCusTypeId = nextId
            });
        }
        [HttpPost]
        public JsonResult UpdateCustomerType(int Id, string CustomerTypeName)
        {
            var type = _context.CustomerTypes.Find(Id);
            if (type == null)
            {
                return Json(new { success = false, message = "Customer Type not found." });
            }
            if (_context.CustomerTypes.Any(c => c.CustomerTypeName == CustomerTypeName))
            {
                return Json(new { success = false, message = "Customer Type is already exists" });
            }
            type.CustomerTypeName = CustomerTypeName;
            _context.SaveChanges();

            return Json(new { success = true });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }

            var customers = await _context.Customers.ToListAsync();
            return Json(new { success = true, customers = customers });
        }



        [HttpPost]
        public JsonResult deleteCustomerSelected(List<String> ids)
        {
            try
            {
                foreach(var id in ids)
                {
                    int nId = int.Parse(id);
                    var customer = _context.Customers.Find(nId);
                    
                    if(customer != null)
                    {
                        _context.Customers.Remove(customer);
                    }
                }
                _context.SaveChanges();
                return Json(new {success=true});
            }
            catch
            {
                return Json(new { success=false});
            }
        }

        public async Task<IActionResult> GetCustomerTable()
        {
            var customers = await _context.Customers!
                .Include(d => d.DeliveryAddresses)
                .ToListAsync();         

            return PartialView("_CustomerTablePartial", customers);
        }
      

        public IActionResult DownloadPdf()
        {
            var customers = _context.Customers.ToList();
            string htmlContent = GenerateHtml(customers);

            var pdfBytes = _pdfGeneratorService.GgeneratePdfFromHtml(htmlContent);

            return File(pdfBytes, "application/pdf", "CustomerList.pdf");
        }


        private string GenerateHtml(List<Customer> customers)
        {
            string host = $"{Request.Scheme}://{Request.Host}";

            var html = @"
                <html>
                <head>
                    <style>
                        table { width: 100%; border-collapse: collapse; }
                        th, td { border: 1px solid #ddd; padding: 8px; }
                        th { background-color: #f2f2f2; text-align: left; }
                        img { display: block; margin: auto; }
                         hr{background-color: red; height:50px; width:500px;}
                    </style>
                </head>
                <body>
                    <h2>Customer Report</h2>
        <hr/>
        <hr/>
        <hr/>
                    <table>
                        <thead>
                            <tr>
                                <th>Id</th>
                                <th>Name</th>
                                <th>Email</th>
                                <th>Address</th>
                                <th>Image</th>
                            </tr>
                        </thead>
                        <tbody>";

            foreach (var customer in customers)
            {
                string imagePath = $"{host}/images/{customer.Photo}";
                html += $@"
                    <tr>
                        <td>{customer.CustomerId}</td>
                        <td>{customer.CustomerName}</td>
                        <td>{customer.Email}</td>
                        <td>{customer.Address}</td>
                        <td><img src='{imagePath}' width='50' height='50'/></td>
                    </tr>";
            }

            html += @"
                        </tbody>
                    </table>
                </body>
                </html>";

            return html;
        }
      
        public IActionResult Invoice()
        {
            var customers = _context.Customers
                .Include(c => c.DeliveryAddresses)
                .Include(c => c.CustomerType)
                .ToList();

            var customerDtos = customers.Select(c => new CustomerDto
            {
                AI_ID = c.AI_ID,
                CustomerId = c.CustomerId,
                CustomerName = c.CustomerName,
                Address = c.Address,
                BusinessStart = c.BusinessStart,
                CustomerType = c.CustomerType,
                Phone = c.Phone,
                Email = c.Email,
                CreditLimit = c.CreditLimit,
                Photo = c.Photo,
                DeliveryAddressDtos = c.DeliveryAddresses?
                    .Where(d => d != null &&
                           (!string.IsNullOrWhiteSpace(d.DeliveryAddressLine) ||
                            !string.IsNullOrWhiteSpace(d.ContactPerson) ||
                            !string.IsNullOrWhiteSpace(d.Phone)))
                    .Select(d => new DeliveryAddressDto
                    {
                        DeliveryAddressLine = d.DeliveryAddressLine ?? "",
                        ContactPerson = d.ContactPerson ?? "",
                        Phone = d.Phone ?? ""
                    }).ToList()
            }).ToList();

            return View(customerDtos);
        }

        public IActionResult GetCustomers()
        {
            try
            {
                var customers = _context.Customers
                    .Include(c => c.DeliveryAddresses)
                    .Include(c => c.CustomerType)
                    .ToList();

                var customerDtos = customers.Select(c => new CustomerDto
                {
                    AI_ID = c.AI_ID,
                    CustomerId = c.CustomerId,
                    CustomerName = c.CustomerName,
                    Address = c.Address,
                    BusinessStart = c.BusinessStart,
                    CustomerType = c.CustomerType != null ? new CustomerType
                    {
                        CustomerTypeId = c.CustomerType.CustomerTypeId,
                        CustomerTypeName = c.CustomerType.CustomerTypeName
                    } : null,
                    Phone = c.Phone,
                    Email = c.Email,
                    CreditLimit = c.CreditLimit,
                    Photo = c.Photo,
                    DeliveryAddressDtos = c.DeliveryAddresses?
                        .Where(d => d != null &&
                                (!string.IsNullOrWhiteSpace(d.DeliveryAddressLine) ||
                                 !string.IsNullOrWhiteSpace(d.ContactPerson) ||
                                 !string.IsNullOrWhiteSpace(d.Phone)))
                        .Select(d => new DeliveryAddressDto
                        {
                            DeliveryAddressLine = d.DeliveryAddressLine ?? "",
                            ContactPerson = d.ContactPerson ?? "",
                            Phone = d.Phone ?? ""
                        }).ToList()
                }).ToList();

                return Json(customerDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        public IActionResult DownloadInvoice()
        {
            try
            {
                // Create a Word document
                WordDocument document = new WordDocument();

                // Add a section to the document
                IWSection section = document.AddSection();
                IWParagraph paragraph = section.AddParagraph();
                paragraph.AppendText("Customer List Report");

                // Add a table with 1 row and 8 columns (headers only)
                IWTable table = section.AddTable();
                table.ResetCells(1, 8);  // 1 row, 8 columns

                // Set the header row
                table[0, 0].Paragraphs[0].AppendText("S/L");
                table[0, 1].Paragraphs[0].AppendText("Photo");
                table[0, 2].Paragraphs[0].AppendText("Customer ID");
                table[0, 3].Paragraphs[0].AppendText("Name");
                table[0, 4].Paragraphs[0].AppendText("Phone");
                table[0, 5].Paragraphs[0].AppendText("Email");
                table[0, 6].Paragraphs[0].AppendText("Customer Type");
                table[0, 7].Paragraphs[0].AppendText("Delivery Addresses");

                // Save the document to a memory stream and return as a file
                using (MemoryStream stream = new MemoryStream())
                {
                    document.Save(stream, FormatType.Docx);
                    stream.Position = 0;
                    return File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "CustomerList.docx");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }



    }
}
