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
    public class SectionController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ISectionsService _sectionsService;

        public SectionController(
            UserManager<User> userManager,
            ApplicationDbContext context,
            ISectionsService sectionsService)
        {
            _userManager = userManager;
            _context = context;
            _sectionsService = sectionsService;
        }


        [HttpPost]
        [Authorize(Policy ="Admin", AuthenticationSchemes = "Bearer")]
        [Route("sections")]

        public async Task<IActionResult> AddSection([FromBody] AddSectionModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {

                _sectionsService.AddSection(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,ex);
            }
        }

        [HttpGet]
        [Route("sections")]

        public IActionResult GetAllSection()
        {
            try
            {
                var collect_section = _context.ForumSections.Select(x => new { x.Name,x.Description}).ToList();
                return Ok(collect_section);
            }catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPut]
        [Route("sections/{id}")]
        public async Task<IActionResult> PutSection(int id,[FromBody] AddSectionModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var section = _sectionsService.GetSection(id);
                if (section == null)
                    return StatusCode(StatusCodes.Status404NotFound, "Section Not Found");

                _sectionsService.EditSection(section, model);
                await _context.SaveChangesAsync();
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpDelete]
        [Route("sections/{id}")]

        public async Task<IActionResult> DeleteSection(int id)
        {
            try
            {
                var section = _sectionsService.GetSection(id);
                if (section == null)
                    return StatusCode(StatusCodes.Status404NotFound, "Section Not Found");
                _context.Remove(section);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("sections/{id}/topics")]

        public IActionResult GetTopicsForSection(int id)
        {
            try
            {
                var section = _sectionsService.GetSection(id);
                if (section == null)
                    return StatusCode(StatusCodes.Status404NotFound, "Section Not Found");
                var topic = _context.Topics.Where(x => x.ForumSectionId == id).Select(x => new {x.Id, x.Name ,x.Description,x.Created}).ToList();
                if (topic == null)
                    return StatusCode(StatusCodes.Status404NotFound, "Section Not Found");

                return Ok(topic);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("sections/{id}/topics")]
        public async Task<IActionResult> AddTopic(int id, AddTopicModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var section = _context.ForumSections.FirstOrDefault(x => x.Id == id);
                if (section == null)
                    return StatusCode(StatusCodes.Status404NotFound, "Section Not Found");
                var topic = new Topic
                {
                    Name = model.Name,
                    Description = model.Description,
                    ForumSection = section,
                    User = user
                };
                section.Topics.Add(topic);
                _context.ForumSections.Update(section);
                await _context.Topics.AddAsync(topic);
                user.Topics.Add(topic);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return Ok();
                
            }catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.ToString());
            }
        }
    }
}
