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
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = JwtConfigurations.Issuer,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = JwtConfigurations.Audience,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = JwtConfigurations.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddIdentity<User, IdentityRole<Guid>>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireUppercase = false;
    opt.User.RequireUniqueEmail = false;
    opt.Password.RequireDigit = false;
}) // Добавление identity к проекту
    .AddEntityFrameworkStores<ApplicationDbContext>() // указание контекста
    .AddSignInManager<SignInManager<User>>() // явное указание того, что менеджер авторизации должен работать с переопределенной моделью пользователя
    .AddUserManager<UserManager<User>>() // аналогично для менеджера юзеров
    .AddRoleManager<RoleManager<IdentityRole<Guid>>>(); // аналогично для менеджера ролей


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
