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
using Shop.ViewModels.Category;

namespace Shop.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        
        public CategoryController(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }
        
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<List<Category>>> Get()
        {
            var categories = await _context.Categories.AsNoTracking()
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
        public async Task<ActionResult<Category>> GetById(int categoryId)
        {
            var category = await _context.Categories.AsNoTracking()
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
        public async Task<ActionResult<Category>> Post([FromBody]CategoryViewModelCreate categoryViewModelCreate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var category = _mapper.Map<Category>(categoryViewModelCreate);
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return Ok(categoryViewModelCreate);
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
        public async Task<ActionResult<Category>> Put(int categoryId,
                                                      [FromBody]CategoryViewModelUpdate categoryViewModelUpdate)
        {
            if (categoryId != categoryViewModelUpdate.Id)
            {
                return NotFound(new { message = "Categoria não encontrada." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _context.Categories.AsNoTracking()
                                                   .FirstOrDefaultAsync(x => x.Id == categoryId);
            if(category == null)
            {
                return BadRequest(new { message = "Categoria informada não existe." });
            }

            try
            {
                var categoryUpdate = _mapper.Map<Category>(categoryViewModelUpdate);
                _context.Entry<Category>(categoryUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(categoryUpdate);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Não foi possível atualizar esta categoria." });
            }
        }

        [HttpDelete]
        [Route("{categoryId:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Category>> Delete(int categoryId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);

            if (category == null)
            {
                return NotFound(new { message = "Categoria não encontrada." });
            }

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return Ok(category);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível remover a categoria." });
            }
        }
    }
}