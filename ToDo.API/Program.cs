using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ToDo.API.Extensions;
using ToDo.API.Middlewares;
using ToDo.Application.Abstractions;
using ToDo.Application.Profiles;
using ToDo.Application.Services.Attachments;
using ToDo.Application.Services.Categories;
using ToDo.Application.Services.Comments;
using ToDo.Application.Services.ToDo;
using ToDo.Application.Services.Users;
using ToDo.Infrastructure;
using ToDo.Persistence;
using ToDo.Persistence.Concrete;

var builder = WebApplication.CreateBuilder(args);

//SERILOG YAPILANDIRMASI
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning() //sadece warning, error ve critical seviyelerini yazar dosyaya
    .WriteTo.Console() //terminal ekranına bas
    .WriteTo.File("Logs/todo-log-.txt", rollingInterval: RollingInterval.Day) //Günlüx .txt dosyası oluşturma
    .CreateLogger();

builder.Host.UseSerilog(); //projedeki varsayılan logger yerine serilog'u mühürleme
builder.Services.AddCustomControllers();
builder.Services.ConfigureOptions<ConfigureApiBehaviorOptions>();
builder.Services.AddValidatorsFromAssemblyContaining
    <ToDo.Application.Validators.ToDoItemsSaveDtoValidator>();
//builder.Services.AddValidatorsFromAssemblyContaining
//    <ToDoApp.Validators.CategorySaveDtoValidator>();

builder.Services.AddOpenApi();

// AutoMapper 16.1+ Modern Dependency Injection Standardı
builder.Services.AddAutoMapper(cfg =>
{
    cfg.DisableConstructorMapping();
}, typeof(MappingProfile).Assembly);

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration
    .GetConnectionString("DefaultConnection")));

builder.Services.AddSwaggerGen();

builder.Services.AddScoped(typeof(IGenericRepository<>),
    typeof(GenericRepository<>));

builder.Services.AddScoped<IToDoRepository, ToDoRepository>();

builder.Services.AddScoped<IToDoService, ToDoService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IStorageService, LocalStorageService>();

//authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

//.Net Core'un kendi problemdetails servisini projeye dahil ettim
builder.Services.AddProblemDetails();

//swagger'da authorize ayarı yapabilmek için yaptığımız ekleme
builder.Services.AddSwaggerConfiguration();

// HTTP isteğinin (HttpContext) bilgilerine, Controller dışında da erişebilmek için kullanıyoruz.
builder.Services.AddHttpContextAccessor();

// Kendi yazdığımız GlobalExceptionHandler sınıfını sisteme kaydettim.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

//UnitofWork olayını tanımlama (saveleme olayını tek bir yere toplama)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();