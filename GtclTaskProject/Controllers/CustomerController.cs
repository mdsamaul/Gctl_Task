using GtclTaskProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Dml.Chart;

namespace GtclTaskProject.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IWebHostEnvironment _en;

        public CustomerController(IWebHostEnvironment en)
        {
            _en = en;
        }

        public async Task<IActionResult> Index()
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();

           
            var customers = from c in _db.Customers
                            join ct in _db.CustomerTypes on c.CustomerType equals ct.Id
                            select new CustomerViewModel
                            {
                                CustomerId = c.CustomerId,
                                CustomerName = c.CustomerName,
                                Address = c.Address,
                                BusinessStart = c.BusinessStart,
                                CustomerType = c.CustomerType,
                                CustomerTypeName = ct.CustomerType1, 
                                Phone = c.Phone,
                                Email = c.Email,
                                CreditLimit = c.CreditLimit,
                                Photo = c.Photo,                                
                            };

            return View(customers.ToList());




            //return View(cs.ToList());
        }

        public IActionResult Create()
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            var viewModel = new CustomerViewModel();
            ViewBag.CustomerType = new SelectList(_db.CustomerTypes, "Id", "CustomerType1");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel viewModel)
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();

            bool exists = await _db.Customers.AnyAsync(c =>
                c.CustomerName == viewModel.CustomerName &&
                c.Address == viewModel.Address);

            if (exists)
            {
                ModelState.AddModelError("", "A customer with this name and address already exists.");
                ViewBag.CustomerType = new SelectList(_db.CustomerTypes, "Id", "CustomerType1");
                return View(viewModel);
            }

            if (ModelState.IsValid)
            {
                var customer = new Customer
                {
                    CustomerName = viewModel.CustomerName,
                    Address = viewModel.Address,
                    BusinessStart = viewModel.BusinessStart,
                    CustomerType = viewModel.CustomerType,
                    Phone = viewModel.Phone,
                    Email = viewModel.Email,
                    CreditLimit = viewModel.CreditLimit,
                };

                if (viewModel.PhotoFile != null)
                {
                    var file = DateTime.Now.Ticks.ToString() + Path.GetExtension(viewModel.PhotoFile.FileName);
                    var fileName = _en.WebRootPath + "/Images/" + file;
                    using (var stream = System.IO.File.Create(fileName))
                    {
                        viewModel.PhotoFile.CopyTo(stream);
                    }
                    customer.Photo = "/Images/" + file;
                }

                await _db.Customers.AddAsync(customer);
                await _db.SaveChangesAsync();

                // Add Delivery Addresses
                if (viewModel.DeliveryAddresses != null && viewModel.DeliveryAddresses.Any())
                {
                    foreach (var address in viewModel.DeliveryAddresses)
                    {
                        var deliveryAddress = new DeliveryAddress
                        {
                            DeliveryAddressLine = address.DeliveryAddressLine,
                            ContactPerson = address.ContactPerson,
                            Phone = address.Phone,
                            CustomerId = customer.CustomerId
                        };

                        await _db.DeliveryAddresses.AddAsync(deliveryAddress);
                    }
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewBag.CustomerType = new SelectList(_db.CustomerTypes, "Id", "CustomerType1");
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            ViewBag.CustomerType = new SelectList(_db.CustomerTypes, "Id", "CustomerType1");
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _db.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            // Fetch delivery addresses based on the customer ID
            var deliveryAddresses = await _db.DeliveryAddresses.Where(c => c.CustomerId == customer.CustomerId).ToListAsync();
            var customerType = await _db.CustomerTypes.FindAsync(customer.CustomerType);
            // Create the ViewModel and pass the data, including DeliveryAddresses
            var viewModel = new CustomerViewModel
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,
                Address = customer.Address,
                BusinessStart = customer.BusinessStart,
                CustomerTypeName = customerType.CustomerType1,
                Phone = customer.Phone,
                Email = customer.Email,
                CreditLimit = customer.CreditLimit,
                Photo = customer.Photo,
                DeliveryAddresses = deliveryAddresses.Select(d => new DeliveryAddressViewModel
                {
                    DeliveryAddressId = d.DeliveryAddressId,
                    DeliveryAddressLine = d.DeliveryAddressLine,
                    ContactPerson = d.ContactPerson,
                    Phone = d.Phone,
                    CustomerId = d.CustomerId
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerViewModel viewModel)
        {
            if (id != viewModel.CustomerId)
            {
                return NotFound();
            }

            using (var _db = new GctlinfoExamTest2025Context())
            {
                // Check if a customer with the same name and address exists
                bool exists = await _db.Customers.AnyAsync(c =>
                    c.CustomerId != id &&
                    c.CustomerName == viewModel.CustomerName &&
                    c.Address == viewModel.Address);

                if (exists)
                {
                    ModelState.AddModelError("", "A customer with this name and address already exists.");
                    return View(viewModel);
                }

                var customer = await _db.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                // Update customer details
                customer.CustomerName = viewModel.CustomerName;
                customer.Address = viewModel.Address;
                customer.BusinessStart = viewModel.BusinessStart;
                customer.CustomerType = viewModel.CustomerType;
                customer.Phone = viewModel.Phone;
                customer.Email = viewModel.Email;
                customer.CreditLimit = viewModel.CreditLimit;

                // Handle photo upload
                if (viewModel.PhotoFile != null)
                {
                    var file = DateTime.Now.Ticks.ToString() + Path.GetExtension(viewModel.PhotoFile.FileName);
                    var fileName = Path.Combine(_en.WebRootPath, "Images", file);
                    using (var stream = System.IO.File.Create(fileName))
                    {
                        await viewModel.PhotoFile.CopyToAsync(stream);
                    }
                    customer.Photo = "/Images/" + file;
                }
                else if (viewModel.RemoveExistingPhoto)
                {
                    customer.Photo = null;
                }

                _db.Entry(customer).State = EntityState.Modified;

                // Get existing delivery addresses for this customer
                var existingAddresses = await _db.DeliveryAddresses
                    .Where(d => d.CustomerId == customer.CustomerId)
                    .ToListAsync();

                // Remove all existing delivery addresses
                _db.DeliveryAddresses.RemoveRange(existingAddresses);
                await _db.SaveChangesAsync();

                // Add all delivery addresses from the viewModel (both updated existing ones and new ones)
                if (viewModel.DeliveryAddresses != null && viewModel.DeliveryAddresses.Any())
                {
                    foreach (var address in viewModel.DeliveryAddresses)
                    {
                        // Create a new delivery address entry
                        var deliveryAddress = new DeliveryAddress
                        {
                            DeliveryAddressLine = address.DeliveryAddressLine,
                            ContactPerson = address.ContactPerson,
                            Phone = address.Phone,
                            CustomerId = customer.CustomerId
                        };

                        await _db.DeliveryAddresses.AddAsync(deliveryAddress);
                    }
                }

                // Save all changes
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _db.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            var customer = _db.Customers.Find(id);
            var delivery = _db.DeliveryAddresses.Where(c => c.CustomerId == customer.CustomerId).ToList();
            _db.DeliveryAddresses.RemoveRange(delivery);
            _db.Customers.Remove(customer);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _db.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        public IActionResult AddDeliveryAddress(int? id)
        {
            return PartialView("_addDeliveryAddress");
        }
        public IActionResult GetCustomerTypePartial()
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();
            var customerTypes = _db.CustomerTypes.ToList(); // List<CustomerType> তে data নেওয়া
            return PartialView("_GetCustomerTypePartial", customerTypes); // PartialView এ পাঠানো
        }

        [HttpPost]
        public IActionResult AddCustomerType(string CustomerTypeName)
        {
            GctlinfoExamTest2025Context _db = new GctlinfoExamTest2025Context();

            if (!string.IsNullOrEmpty(CustomerTypeName))
            {
                var exCustomer = _db.CustomerTypes.Any(c => c.CustomerType1 == CustomerTypeName);
                if (exCustomer)
                {
                    return Json(new { success = false, message = "Customer type already exists" });
                }

                var customerType = new CustomerType { CustomerType1 = CustomerTypeName };
                _db.CustomerTypes.Add(customerType);
                _db.SaveChanges();
                return Json(new { success = true, customerType = customerType });
            }

            return Json(new { success = false, message = "Customer type is required" });
        }




    }
}