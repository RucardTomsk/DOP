using APIDOP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using APIDOP.Models.DB;
using Microsoft.AspNetCore.Identity;
using APIDOP.Services;
using APIDOP;
using System.Security.Claims;
using APIDOP.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthenticateService, AuthenticateService>();
builder.Services.AddScoped<ISectionsService, SectionsService>();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(connection));
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim(ApplicationRoleNames.Administrator));
    options.AddPolicy("User", policy => policy.RequireClaim(ApplicationRoleNames.User));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // ���������, ����� �� �������������� �������� ��� ��������� ������
            ValidateIssuer = true,
            // ������, �������������� ��������
            ValidIssuer = JwtConfigurations.Issuer,
            // ����� �� �������������� ����������� ������
            ValidateAudience = true,
            // ��������� ����������� ������
            ValidAudience = JwtConfigurations.Audience,
            // ����� �� �������������� ����� �������������
            ValidateLifetime = true,
            // ��������� ����� ������������
            IssuerSigningKey = JwtConfigurations.GetSymmetricSecurityKey(),
            // ��������� ����� ������������
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddIdentity<User, IdentityRole<Guid>>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.User.RequireUniqueEmail = false;
    opt.Password.RequireDigit = false;
}) // ���������� identity � �������
    .AddEntityFrameworkStores<ApplicationDbContext>() // �������� ���������
    .AddSignInManager<SignInManager<User>>() // ����� �������� ����, ��� �������� ����������� ������ �������� � ���������������� ������� ������������
    .AddUserManager<UserManager<User>>() // ���������� ��� ��������� ������
    .AddRoleManager<RoleManager<IdentityRole<Guid>>>(); // ���������� ��� ��������� �����


var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

using var serviceScope = app.Services.CreateScope();
var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
// auto migration
context?.Database.Migrate();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
