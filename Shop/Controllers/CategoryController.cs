using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : Controller
    {
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext context)
        {
            var categories = await context.Categories.AsNoTracking()
                                                     .ToListAsync();
            if(!categories.Any())
            {
                return NoContent();
            }
            return Ok(categories);
        }

        [HttpGet]
        [Route("{categoryId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Category>> GetById([FromServices] DataContext context,
                                                           int categoryId)
        {
            var category = await context.Categories.AsNoTracking()
                                                   .FirstOrDefaultAsync(x => x.Id == categoryId);
            if(category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Category>> Post([FromServices] DataContext context,
                                                       [FromBody]Category categoryModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(categoryModel);
                await context.SaveChangesAsync();
                return Ok(categoryModel);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar esta categoria." });
            }
        }

        [HttpPut]
        [Route("{categoryId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Category>> Put([FromServices] DataContext context,
                                                       int categoryId,
                                                       [FromBody]Category model)
        {
            if (categoryId != model.Id)
                return NotFound(new { message = "Categoria não encontrada" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Não foi possível atualizar esta categoria" });
            }
        }

        [HttpDelete]
        [Route("{categoryId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Category>> Delete([FromServices] DataContext context,
                                                          int categoryId)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);

            if (category == null)
            {
                return NotFound(new { message = "Categoria não encontrada" });
            }

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(category);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível remover a categoria" });
            }
        }
    }
}