using Microsoft.AspNetCore.Mvc; // IActionResult
using Microsoft.AspNetCore.OData.Query; // [EnableQuery]
using Microsoft.AspNetCore.OData.Routing.Controllers; // ODataController
using Packt.Shared; // NorthwindContext

namespace NorthwindOData.Controllers
{
    public class SuppliersController : ODataController
    {
        private readonly Northwind db;

        public SuppliersController(Northwind db)
        {
            this.db = db;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(db.Suppliers);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(db.Suppliers.Find(key));
        }
    }
}