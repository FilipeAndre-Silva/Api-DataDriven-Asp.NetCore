using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.ViewModels;

namespace Shop.Controllers
{
    [Route("v1/products")]
    public class ProductController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        
        public ProductController(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<List<Product>>> Get()
        {
            var products = await _context.Products.Include(x => x.Category)
                                                  .AsNoTracking()
                                                  .ToListAsync();
            if(!products.Any())
            {
                return NoContent();
            }
            return Ok(products);
        }

        [HttpGet]
        [Route("{productId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetById(int productId)
        {
            var product = await _context.Products.Include(x => x.Category)
                                                 .FirstOrDefaultAsync(x => x.Id == productId);
            if(product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> Post([FromBody]ProductViewModelCreate productModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _context.Categories.AsNoTracking()
                                                   .FirstOrDefaultAsync(x => x.Id == productModel.CategoryId);
            if(category == null)
            {
                return BadRequest(new { message = "Categoria informada não existe." });
            }
            try
            {
                var product = _mapper.Map<Product>(productModel);
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return Ok(product);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar este produto." });
            }
        }

        [HttpPut]
        [Route("{productId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> Put(int productId,
                                                     [FromBody]ProductViewModelUpdate productModel)
        {
            if (productId != productModel.Id)
            {
                return NotFound(new { message = "Produto não encontrado." });
            }
            
            var category = await _context.Categories.AsNoTracking()
                                                   .FirstOrDefaultAsync(x => x.Id == productModel.CategoryId);
            if(category == null)
            {
                return BadRequest(new { message = "Categoria informada não existe." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var product = _mapper.Map<Product>(productModel);
                _context.Entry<Product>(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(product);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Não foi possível atualizar este produto." });
            }
        }

        [HttpDelete]
        [Route("{productId:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> Delete(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);

            if (product == null)
            {
                return NotFound(new { message = "Produto não encontrado." });
            }

            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return Ok(product);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível remover o produto" });
            }
        }
    }
}