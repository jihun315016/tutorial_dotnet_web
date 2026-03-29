using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectsManagement.Models;

namespace ProjectsManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        static List<Product> products = new List<Product>
            {
                new Product { Id = 1, Name = "Labtop", Description = "Labtop Product1", Pricd = 10.99m },
                new Product { Id = 2, Name = "Smartphone", Description = "Smartphone Product2", Pricd = 20.99m },
                new Product { Id = 3, Name = "Tablet", Description = "Tablet Product3", Pricd = 30.99m }
            };

        [HttpGet]
        public IActionResult GetProducts()
        {
            return Ok(products);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetProductById(int id)
        {
            var response = products.FirstOrDefault(p => p.Id == id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult UpdateProduct(int id, Product product)
        {
            var existingProduct = products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Pricd = product.Pricd;

            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            products.Remove(product);
            return NoContent();
        }
    }
}
