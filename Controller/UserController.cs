using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PicPayClone.Data;
using PicPayClone.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using BCrypt.Net;

namespace PicPayClone.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (user == null)
                return BadRequest();
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.CPF == user.CPF || u.Email == user.Email);

            if (existingUser != null)
            {
                return StatusCode(409, "CPF ou Email já Registrado.");
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }
    }
}
