using Microsoft.EntityFrameworkCore;
using HalogenPreTestAPI.Data;
using Microsoft.Extensions.DependencyInjection;


var policyHalogenPreTest = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<FileDBContext>(options =>
    options.UseInMemoryDatabase("HalogenGroupDB"));
// options.UseSqlServer(builder.Configuration.GetConnectionString("FileDBContext") ?? throw new InvalidOperationException("Connection string 'FileDBContext' not found.")));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policyHalogenPreTest,
                        builder =>
                        {
                            builder
                                // .WithOrigins("http://localhost:3000")
                                .AllowAnyOrigin()
                                .AllowAnyMethod()
                                // .WithMethod("GET")
                                .AllowAnyHeader();
                        });
});

// builder.Services.AddDbContext<FilDBContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("FilDBContext") ?? throw new InvalidOperationException("Connection string 'FilDBContext' not found.")));

// Add services to the container.
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddDbContext<FileDBContext>(opt =>
//     opt.UseInMemoryDatabase("FileDBList"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors(policyHalogenPreTest);

app.UseAuthorization();

app.UseEndpoints(endPoints =>
{
    endPoints.MapControllers()
        .RequireCors(policyHalogenPreTest);
    // endPoints.MapControllerRoute("/sendfile")
    //     .RequireCors(policyHalogenPreTest);
});

app.MapControllers();

app.Run();
