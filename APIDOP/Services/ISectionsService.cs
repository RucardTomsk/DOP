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

namespace APIDOP.Services
{
    public interface ISectionsService
    {
        void AddSection(AddSectionModel model);
        ForumSection GetSection(int id);
        void EditSection(ForumSection section, AddSectionModel model);
    }
    public class SectionsService: ISectionsService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public SectionsService(UserManager<User> usersService,
            ApplicationDbContext context)
        {
            _userManager = usersService;
            _context = context;
        }

        public async void AddSection(AddSectionModel model)
        {
            var section = new ForumSection { Name = model.Name, Description = model.Description };
            await _context.ForumSections.AddAsync(section);
            _context.SaveChanges();
        }

        public ForumSection GetSection(int id)
        {
            return _context.ForumSections.FirstOrDefault(x => x.Id == id);
        }

        public void EditSection(ForumSection section, AddSectionModel model)
        {
            section.Name = model.Name;
            section.Description = model.Description;
            _context.ForumSections.Update(section);
            _context.SaveChanges();
        }
    }
}
