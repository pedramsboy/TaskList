using AutoMapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using TaskList.Application.Classes;
using TaskList.Application.Interfaces;
using TaskList.Data;
using TaskList.Data.Repositories;
using TaskList.Domain;
using TaskList.Domain.RepositoryInerfaces;
using TaskList.Repositories.Classes;
using TaskList.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repositories
builder.Services.AddScoped<ITaskListRepository, TaskListRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

// Register services with their dependencies
builder.Services.AddScoped<ITaskListService>(provider =>
    new TaskListService(
        provider.GetRequiredService<ITaskListRepository>(),
        provider.GetRequiredService<IMapper>(),
        provider.GetRequiredService<IFileStorageService>(),
        provider.GetRequiredService<ILogger<TaskListService>>()));

builder.Services.AddScoped<ITaskService>(provider =>
    new TaskService(
        provider.GetRequiredService<ITaskRepository>(),
        provider.GetRequiredService<ITaskListRepository>(),
        provider.GetRequiredService<IMapper>()));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

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

// Add global exception handling
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            context.Response.StatusCode = contextFeature.Error switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                InvalidOperationException => StatusCodes.Status409Conflict,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            await context.Response.WriteAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = contextFeature.Error.Message
            }.ToString());
        }
    });
});

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