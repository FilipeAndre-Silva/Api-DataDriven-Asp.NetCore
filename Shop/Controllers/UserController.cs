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
using Shop.Services;
using Shop.ViewModels.User;

namespace Shop.Controllers
{
    [Route("v1/users")]
    public class UserController : Controller
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        
        public UserController(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<List<User>>> Get()
        {
            var users = await _context.Users.AsNoTracking()
                                            .ToListAsync();
            if(!users.Any())
            {
                return NoContent();
            }
            return Ok(users);
        }

        [HttpGet]
        [Route("{userId:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetById(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if(user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> Post([FromBody]UserViewModelCreate userViewModelCreate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = _mapper.Map<User>(userViewModelCreate);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok(userViewModelCreate);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar este usuário." });
            }
        }

        [HttpPut]
        [Route("{userId:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> Put(int userId,
                                                     [FromBody]UserViewModelUpdate userViewModelUpdate)
        {
            if (userId != userViewModelUpdate.Id)
            {
                return NotFound(new { message = "Usuário não encontrado." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = await _context.Users.AsNoTracking()
                                                   .FirstOrDefaultAsync(x => x.Id == userId);
            if(user == null)
            {
                return BadRequest(new { message = "Usuário informada não existe." });
            }

            try
            {
                var userUpdate = _mapper.Map<User>(userViewModelUpdate);
                _context.Entry<User>(userUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(userUpdate);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Não foi possível atualizar este usuário." });
            }
        }

        [HttpDelete]
        [Route("{userId:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> Delete(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado." });
            }

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível remover o usuário." });
            }
        }
    
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody]UserViewModelAutentication userViewModelAutentication)
        {
            var userViewModel = _mapper.Map<User>(userViewModelAutentication);
            var user = await _context.Users.AsNoTracking()
                                           .Where(x => x.Username == userViewModel.Username && 
                                                       x.Password == userViewModel.Password)
                                           .FirstOrDefaultAsync();
            if (userViewModel == null)
            {
                return NotFound(new { message = "Usuário ou senha inválidos" });
            }

            var token = TokenService.GenerateToken(user);
            userViewModel.Password = "*****";

            return new
            {
                user = userViewModel,
                token = token
            };
        }
    }
}