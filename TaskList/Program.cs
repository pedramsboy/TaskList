using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using TaskList.Application.Classes;
using TaskList.Application.Interfaces;
using TaskList.Data;
using TaskList.Data.Repositories;
using TaskList.Domain;
using TaskList.Domain.RepositoryInerfaces;
using TaskList.Repositories.Classes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

// Add repositories
builder.Services.AddScoped<ITaskListRepository, TaskListRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

// Add services
builder.Services.AddScoped<ITaskListApplicationService, TaskListApplicationService>();
builder.Services.AddScoped<ITaskApplicationService, TaskApplicationService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add controllers
builder.Services.AddControllers();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Configure static files
var uploadsFolder = Path.Combine(builder.Environment.ContentRootPath, "Uploads");
if (!Directory.Exists(uploadsFolder))
{
    Directory.CreateDirectory(uploadsFolder);
}

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".webp"] = "image/webp";

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsFolder),
    RequestPath = "/uploads",
    ContentTypeProvider = provider,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append(
            "Cache-Control", "public,max-age=604800"); // 1 week cache
    }
});

app.Run();