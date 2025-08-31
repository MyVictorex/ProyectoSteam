


using WEB_API_JUEGOS.Data;
using WEB_API_JUEGOS.Data.Contrato;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


builder.Services.AddScoped<ICompra, RepositorioCompra>();
builder.Services.AddScoped<IJuego, RepositorioJuego>();
builder.Services.AddScoped<IUsuario, RepositorioUsuario>();
builder.Services.AddScoped<ICategoria, RepositorioCategoria>();

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
