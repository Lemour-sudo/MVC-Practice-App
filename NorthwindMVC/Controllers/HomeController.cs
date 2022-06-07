using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NorthwindMVC.Models;
using Packt.Shared;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace NorthwindMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory clientFactory;

        private Northwind db;

        public HomeController(ILogger<HomeController> logger, 
            Northwind injectedContext, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            db = injectedContext;
            clientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeIndexViewModel
            {
                VisitorCount = (new Random()).Next(1, 1001),
                Categories = await db.Categories.ToListAsync(),
                Products = await db.Products.ToListAsync()
            };
            
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> ProductDetail(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound("You must pass a product ID in the route, for example, /Home/ProductDetail/3");
            }

            Product model = await db.Products.SingleOrDefaultAsync(p => p.ProductID == id);

            if (model == null)
            {
                return NotFound($"Product with ID of {id} not found.");
            }

            return View(model);
        }

        public async Task<IActionResult> Category(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound("You must pass a category ID in the route, for example, /Home/Category/3");
            }

            Category model = await db.Categories.SingleOrDefaultAsync(c => c.CategoryID == id);

            if (model == null)
            {
                return NotFound($"Category {id} not found.");
            }

            return View(model);
        }

        public IActionResult ModelBinding()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ModelBinding(Thing thing)
        {
            var checkedModel = new HomeModelBindingViewModel
            {
                Thing = thing,
                HasErrors = !ModelState.IsValid,
                ValidationErrors = ModelState.Values
                    .SelectMany(state => state.Errors)
                    .Select(error => error.ErrorMessage)
            };

            return View(checkedModel);
        }

        public IActionResult ProductsThatCostMoreThan(decimal? price)
        {
            if (!price.HasValue)
            {
                return NotFound("You must pass a product price in the query " +
                    "string, for example, /Home/ProductsThatCostMoreThan?price=50");
            }

            IEnumerable<Product> model = db.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsEnumerable()
                .Where(p => p.UnitPrice >= price);

            if (model.Count() == 0)
            {
                return NotFound($"No products cost more than {price:C}.");
            }

            ViewData["MinPrice"] = price.Value.ToString("C");

            return View(model);
        }

        public async Task<IActionResult> Customers(string country)
        {
            string uri;

            if (string.IsNullOrEmpty(country))
            {
                ViewData["Title"] = "All Customers Worldwide";
                uri = "api/customers/";
            }
            else
            {
                ViewData["Title"] = $"Customers in {country}";
                uri = $"api/customers/?country={country}";
            }

            var client = clientFactory.CreateClient(name: "NorthwindService");

            HttpRequestMessage request = new HttpRequestMessage(method: HttpMethod.Get, requestUri: uri);

            HttpResponseMessage response = await client.SendAsync(request);

            string jsonString = await response.Content.ReadAsStringAsync();

            IEnumerable<Customer> model = JsonConvert.DeserializeObject<IEnumerable<Customer>>(jsonString);

            return View(model);
        }

        public async Task<IActionResult> Services()
        {
            try
            {
                var client = clientFactory.CreateClient(name: "NorthwindOData");

                HttpRequestMessage request = new(
                    method: HttpMethod.Get,
                    requestUri: "catalog/products/?$filter=startswith(ProductName, 'Cha')&$select=ProductId,ProductName,UnitPrice"
                );

                HttpResponseMessage response = await client.SendAsync(request);

                ViewData["productsCha"] = (await response.Content.ReadFromJsonAsync<ODataProducts>())?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"NorthwindOData service exception: {ex.Message}");
            }

            return View();
        }
    }
}
