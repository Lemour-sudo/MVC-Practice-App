using Microsoft.AspNetCore.Mvc; // IActionResult
using Microsoft.AspNetCore.OData.Query; // [EnableQuery]
using Microsoft.AspNetCore.OData.Routing.Controllers; // ODataController
using Packt.Shared; // NorthwindContext

namespace NorthwindOData.Controllers
{
    public class CategoriesController : ODataController
    {
        private readonly Northwind db;

        public CategoriesController(Northwind db)
        {
            this.db = db;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(db.Categories);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(db.Categories.Find(key));
        }
    }
}