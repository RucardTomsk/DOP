using APIDOP.Models.DTO;
using APIDOP.Models.DB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using APIDOP.Enums;
using APIDOP.Services;
using APIDOP.Models;

namespace APIDOP.Controllers
{
    [Route("api")]
    [ApiController]
    public class TopicController: ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public TopicController(
            UserManager<User> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpDelete]
        [Route("topics/{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            try
            {
                var topic = _context.Topics.FirstOrDefault(x => x.Id == id);
                if (topic == null)
                    return StatusCode(StatusCodes.Status404NotFound, "Topic Not Found");
                _context.Topics.Remove(topic);
                await _context.SaveChangesAsync();
                return Ok();

            }catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }

        [HttpPut]
        [Route("topics/{id}")]
        public async Task<IActionResult> EditTopic(int id, EditTopicModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var topic = _context.Topics.FirstOrDefault(x => x.Id == id);
                if (topic == null)
                    return StatusCode(StatusCodes.Status404NotFound, "Topic Not Found");
                topic.Name = model.Name;
                topic.Description = model.Description;
                _context.Topics.Update(topic);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }

        [HttpGet]
        [Route("topics/{id}/messages")]
        public async Task<IActionResult> GetMessagesForTopic(int id)
        {
            try
            {
                var topic = _context.Topics.FirstOrDefault(x => x.Id == id);
                if (topic == null)
                    return StatusCode(StatusCodes.Status404NotFound, "Topic Not Found");

                var massage = _context.Messages.Where(x => x.TopicId == id).Select(x => new { x.Id, x.Text, x.Attachments, x.Created, x.Modified }).ToList();
                return Ok(massage);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("topics/{id}/messages")]
        public async Task<IActionResult> AddMessage(int id, AddMessageModelcs model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var topic = _context.Topics.FirstOrDefault(x => x.Id == id);
                if (topic == null)
                    return StatusCode(StatusCodes.Status404NotFound, "Topic Not Found");
                var messege = new Message
                {
                    Text = model.Text,

                };
                topic.Messages.Add(messege);
                _context.Topics.Update(topic);
                await _context.Messages.AddAsync(messege);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }
    }
}
