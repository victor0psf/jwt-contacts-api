using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gerenciador_contatos.Data;
using gerenciador_contatos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gerenciador_contatos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListContactsController : ControllerBase
    {
        private readonly ILogger<ListContactsController> _logger;
        private readonly AppDbContext _context;

        public ListContactsController(AppDbContext context, ILogger<ListContactsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ListContactsModel>> GetListContact(Guid id)
        {
            try
            {
                var contact = await _context.ListContacts.FindAsync(id);
                if (contact == null)
                    return NotFound("Contact not found");

                return Ok(contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contact with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListContactsModel>>> GetAllContacts()
        {
            try
            {
                var contact = await _context.ListContacts.ToListAsync();
                return Ok(contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contacts");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddListContact([FromBody] ListContactsModel contactsModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage));
            try
            {
                var contact = new ListContactsModel
                {
                    Name = contactsModel.Name,
                    Telephone = contactsModel.Telephone,
                    Email = contactsModel.Email
                };
                _context.ListContacts.Add(contact);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetListContact), new { id = contact.Id }, contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding contact");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}