var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

var productos = new List<Producto>
{
    new(1, "LAC001", "Leche entera 1L",   "Lácteos",   4.50m,  40, "Gloria",        0,  "https://corporacionliderperu.com/50720-large_default/gloria-leche-tarro-azul-gde-x-390-gr.jpg",      "Leche entera en envase de 1 litro."),
    new(2, "LAC002", "Yogurt fresa 1L",   "Lácteos",   6.80m,  8,  "Laive",         10, "https://plazavea.vteximg.com.br/arquivos/ids/34361308-418-418/20326319.jpg",     "Yogurt bebible sabor fresa."),
    new(3, "ABA001", "Arroz extra 1kg",   "Abarrotes", 4.20m,  80, "Costeño",       0,  "https://placehold.co/80x100?text=Arroz",      "Arroz extra de grano largo."),
    new(4, "ABA002", "Aceite vegetal 1L", "Abarrotes", 10.50m, 35, "Primor",        5,  "https://placehold.co/80x100?text=Aceite",     "Aceite vegetal para cocina."),
    new(5, "FRU001", "Manzana (kg)",      "Frutas",    5.50m,  50, "Fresco",        0,  "https://placehold.co/80x100?text=Manzana",    "Manzana roja por kilo."),
    new(6, "BEB001", "Gaseosa 2L",        "Bebidas",   7.50m,  6,  "Inca Kola",     0,  "https://placehold.co/80x100?text=Gaseosa",    "Gaseosa de 2 litros."),
    new(7, "CAR001", "Pollo entero (kg)", "Carnes",    11.90m, 20, "San Fernando",  0,  "https://placehold.co/80x100?text=Pollo",      "Pollo fresco por kilo."),
    new(8, "LIM001", "Detergente 1kg",    "Limpieza",  12.90m, 30, "Bolívar",       15, "https://placehold.co/80x100?text=Detergente", "Detergente en polvo."),
};

var siguienteId = 9;

app.MapGet("/", () => "API Supermercado funcionando");

app.MapGet("/api/supermercado", () => productos);

app.MapGet("/api/supermercado/{id:int}", (int id) =>
{
    var producto = productos.FirstOrDefault(p => p.Id == id);

    return producto is null
        ? Results.NotFound(new { message = "Producto no encontrado." })
        : Results.Ok(producto);
});

app.MapPost("/api/supermercado", (Producto datos) =>
{
    var nuevo = datos with { Id = siguienteId++ };
    productos.Add(nuevo);

    return Results.Created($"/api/supermercado/{nuevo.Id}", nuevo);
});

app.MapPut("/api/supermercado/{id:int}", (int id, Producto datos) =>
{
    var indice = productos.FindIndex(p => p.Id == id);

    if (indice < 0)
        return Results.NotFound(new { message = "Producto no encontrado." });

    var actualizado = datos with { Id = id };
    productos[indice] = actualizado;

    return Results.Ok(actualizado);
});

app.MapDelete("/api/supermercado/{id:int}", (int id) =>
{
    var producto = productos.FirstOrDefault(p => p.Id == id);

    if (producto is null)
        return Results.NotFound(new { message = "Producto no encontrado." });

    productos.Remove(producto);

    return Results.NoContent();
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");

record Producto(
    int Id,
    string Codigo,
    string Nombre,
    string Categoria,
    decimal Precio,
    int Stock,
    string Marca,
    int Descuento,
    string Imagen,
    string Descripcion
);
