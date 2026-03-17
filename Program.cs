

using Hospital_API.Data;
using Hospital_API.Repositories;
using Hospital_API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<AppDbContext>();
builder.Services.AddScoped<MedicoRepository>();
builder.Services.AddScoped<MedicoService>();
builder.Services.AddScoped<PacienteRepository>();
builder.Services.AddScoped<PacienteService>();
builder.Services.AddScoped<CitasRepository>();
builder.Services.AddScoped<CitasService>();
builder.Services.AddScoped<AgendaRepository>();
builder.Services.AddScoped<AgendaService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.UseAuthorization();

app.MapControllers();

app.Run();
