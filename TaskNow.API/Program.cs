using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using TaskNow.API.Hubs;
using TaskNow.API.Mapper;
using TaskNow.API.Middlewares;
using TaskNow.BLL.Entities;
using TaskNow.BLL.Entities.Interfaces;
using TaskNow.BLL.Utils;
using TaskNow.BLL.Utils.Interfaces;
using TaskNow.DAL.Entities;
using TaskNow.DAL.Entities.Interfaces;
using TaskNow.DAO;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Path.GetTempPath(), "TaskNow-DataProtection-Keys")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskNow API",
        Version = "v1",
        Description = "API base do TaskNow para quadros, listas, autenticacao e proximas fases do kanban."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe somente o token JWT gerado pelo endpoint /api/auth/login ou /api/auth/registrar."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document, null),
            []
        }
    });
});
builder.Services.AddDbContext<TaskNowDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<TaskNowDbContext>()
    .AddDefaultTokenProviders();

var jwt = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSignalR();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUsuarioContexto, UsuarioContexto>();

builder.Services.AddScoped<IQuadroDAL, QuadroDAL>();
builder.Services.AddScoped<IListaDAL, ListaDAL>();
builder.Services.AddScoped<ICartaoDAL, CartaoDAL>();
builder.Services.AddScoped<IEtiquetaDAL, EtiquetaDAL>();
builder.Services.AddScoped<IComentarioDAL, ComentarioDAL>();
builder.Services.AddScoped<IAtividadeCartaoDAL, AtividadeCartaoDAL>();

builder.Services.AddScoped<IQuadroBLL, QuadroBLL>();
builder.Services.AddScoped<IListaBLL, ListaBLL>();
builder.Services.AddScoped<ICartaoBLL, CartaoBLL>();
builder.Services.AddScoped<IEtiquetaBLL, EtiquetaBLL>();
builder.Services.AddScoped<IComentarioBLL, ComentarioBLL>();
builder.Services.AddScoped<IAtividadeCartaoBLL, AtividadeCartaoBLL>();




var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Portal", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TaskNowDbContext>();
    db.Database.EnsureCreated();
}
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskNow API v1");
        options.RoutePrefix = "swagger";
    });
}
app.UseCors("Portal");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<QuadroHub>("/hubs/quadro");

app.Run();
