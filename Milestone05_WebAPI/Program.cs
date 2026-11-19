using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Milestone05_WebAPI.Filters;
using Milestone05_WebAPI.Json;
using Milestone05_WebAPI.Repositories;
using Milestone05_WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

SqlMapper.AddTypeHandler(new DateTypeHandler());

string databaseFile = Path.Combine(builder.Environment.ContentRootPath, builder.Configuration["Database:File"]!);
builder.Services.AddSingleton(new SqliteConnectionFactory(databaseFile));
builder.Services.AddScoped<IUserRepository, SqliteUserRepository>();
builder.Services.AddScoped<IExpenseRepository, SqliteExpenseRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddSingleton<TokenService>();

builder.Services
    .AddControllers(options => options.Filters.Add<ServiceExceptionFilter>())
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        options.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
    });

builder.Services.AddProblemDetails();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var auth = context.HttpContext.RequestServices.GetRequiredService<AuthService>();
                string? id = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                string? name = context.Principal?.FindFirstValue(ClaimTypes.Name);

                if (!int.TryParse(id, out int userId) || auth.GetById(userId)?.Username != name)
                {
                    context.Fail("The user of this token does not exist.");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Expense Tracker API", Version = "v1" });

    options.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Example = new OpenApiString("19-11-2026")
    });

    options.MapType<DateOnly?>(() => new OpenApiSchema
    {
        Type = "string",
        Nullable = true,
        Example = new OpenApiString("19-11-2026")
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the token returned by POST /api/auth/login"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

var connectionFactory = app.Services.GetRequiredService<SqliteConnectionFactory>();
string schemaFile = Path.Combine(AppContext.BaseDirectory, "Database", "schema.sql");
new DatabaseInitializer(connectionFactory).Initialize(schemaFile);

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();

app.Run();
