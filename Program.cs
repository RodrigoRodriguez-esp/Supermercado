var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy( policity =>
    {
        policity
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
    }
);

var app = builder.Build();

app.UseCors();
                             
app.MapGet("/",() =>
{
    return "API Supermercado funcionando";
});

app.MapGet("/api/supermercado",() =>
{
    return Results.Ok(new[]
    {
        new{
            id=1,
            codigo="P001",
            nombre="Televisor",
        },
        new{
            id=2,
            codigo="P002",
            nombre="Laptop",
        }
    });
});

var port = Environment.GetEnviromentVariable("Port")??"10000";
app.Run($"http://0.0.0.0:(port)");
