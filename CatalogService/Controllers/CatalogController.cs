using CatalogService.Data;
using CatalogService.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _context;
        private readonly RabbitMqProducer _rabbitMqProducer;

        public CatalogController(CatalogContext context)
        {
            _context = context;
            _rabbitMqProducer = new RabbitMqProducer();
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();

            // Send message to RabbitMQ
            _rabbitMqProducer.SendMessage($"Product added: {product.Name}");

            return Ok(product);
        }
    }
}
