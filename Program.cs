var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

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


app.Run();