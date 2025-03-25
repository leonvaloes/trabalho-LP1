using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Lê as configurações do appsettings.json
    .WriteTo.Console() // Exibe logs no console
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 1) // Retém logs por 1 dia
    .CreateLogger();

builder.Host.UseSerilog(); // Define Serilog como o logger da aplicação

// Adicionando serviços ao container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware de logging do Serilog (deve vir antes das rotas)
app.UseSerilogRequestLogging();

// Configuração do pipeline de requisições
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    try
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();

        Log.Information("Endpoint /weatherforecast chamado com sucesso."); // Log de sucesso

        return Results.Ok(forecast);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Erro ao processar a requisição no endpoint /weatherforecast"); // Log de erro
        return Results.Problem("Ocorreu um erro interno.");
    }
})
.WithName("GetWeatherForecast")
.WithOpenApi(); // Habilita a documentação Swagger para esse endpoint

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
