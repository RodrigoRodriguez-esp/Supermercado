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

// ==========================================
// BASE DE DATOS EN MEMORIA (RECURSOS)
// ==========================================

var categorias = new List<Categoria>
{
    new(1, "Lácteos", "Productos lácteos y derivados frescos", "https://placehold.co/100x100?text=Lacteos"),
    new(2, "Abarrotes", "Artículos de primera necesidad para el hogar", "https://placehold.co/100x100?text=Abarrotes"),
    new(3, "Frutas", "Frutas frescas de estación", "https://placehold.co/100x100?text=Frutas"),
    new(4, "Bebidas", "Gaseosas, jugos y aguas", "https://placehold.co/100x100?text=Bebidas"),
    new(5, "Carnes", "Cortes de ave, res y cerdo", "https://placehold.co/100x100?text=Carnes"),
    new(6, "Limpieza", "Productos de higiene y limpieza del hogar", "https://placehold.co/100x100?text=Limpieza")
};

var promociones = new List<Promocion>
{
    new(1, "Mega Descuento Lácteos", "10% de descuento en yogurts seleccionados", 10, true),
    new(2, "Limpieza Impecable", "15% de descuento en detergentes Bolívar", 15, true),
    new(3, "Ofertón de Aceites", "5% de descuento en aceites Primor", 5, true)
};

var sucursales = new List<Sucursal>
{
    new(1, "Sede Central", "Av. Principal 123", "08:00 - 22:00", "(01) 555-0199"),
    new(2, "Sede Norte", "Av. Las Palmeras 456", "08:30 - 21:30", "(01) 555-0144")
};

var productos = new List<Producto>
{
    new(1, "LAC001", "Leche entera 1L", "Lácteos", 4.50m, 40, "Gloria", 0, "https://placehold.co/200x200?text=Leche", "Leche entera en envase de 1 litro."),
    new(2, "LAC002", "Yogurt fresa 1L", "Lácteos", 6.80m, 8, "Laive", 10, "https://placehold.co/200x200?text=Yogurt", "Yogurt bebible sabor fresa."),
    new(3, "ABA001", "Arroz extra 1kg", "Abarrotes", 4.20m, 80, "Costeño", 0, "https://placehold.co/200x200?text=Arroz", "Arroz extra de grano largo."),
    new(4, "ABA002", "Aceite vegetal 1L", "Abarrotes", 10.50m, 35, "Primor", 5, "https://placehold.co/200x200?text=Aceite", "Aceite vegetal para cocina."),
    new(5, "FRU001", "Manzana (kg)", "Frutas", 5.50m, 50, "Fresco", 0, "https://placehold.co/200x200?text=Manzana", "Manzana roja por kilo."),
    new(6, "BEB001", "Gaseosa 2L", "Bebidas", 7.50m, 6, "Inca Kola", 0, "https://placehold.co/200x200?text=Gaseosa", "Gaseosa de 2 litros."),
    new(7, "CAR001", "Pollo entero (kg)", "Carnes", 11.90m, 20, "San Fernando", 0, "https://placehold.co/200x200?text=Pollo", "Pollo fresco por kilo."),
    new(8, "LIM001", "Detergente 1kg", "Limpieza", 12.90m, 30, "Bolívar", 15, "https://placehold.co/200x200?text=Detergente", "Detergente en polvo.")
};

var siguienteId = 9;

// ==========================================
// ENDPOINTS
// ==========================================

app.MapGet("/", () => "API Supermercado Funcionando Correctamente");

// --- RECURSO 1: PRODUCTOS ---
app.MapGet("/api/supermercado", (string? categoria, string? q) =>
{
    var resultado = productos.AsEnumerable();

    if (!string.IsNullOrEmpty(categoria))
        resultado = resultado.Where(p => p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));

    if (!string.IsNullOrEmpty(q))
        resultado = resultado.Where(p => p.Nombre.Contains(q, StringComparison.OrdinalIgnoreCase) || 
                                         p.Codigo.Contains(q, StringComparison.OrdinalIgnoreCase));

    return Results.Ok(resultado.ToList());
});

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
    if (indice < 0) return Results.NotFound(new { message = "Producto no encontrado." });

    var actualizado = datos with { Id = id };
    productos[indice] = actualizado;
    return Results.Ok(actualizado);
});

app.MapDelete("/api/supermercado/{id:int}", (int id) =>
{
    var producto = productos.FirstOrDefault(p => p.Id == id);
    if (producto is null) return Results.NotFound(new { message = "Producto no encontrado." });

    productos.Remove(producto);
    return Results.NoContent();
});

// --- RECURSO 2: CATEGORÍAS ---
app.MapGet("/api/categorias", () => Results.Ok(categorias));

// --- RECURSO 3: PROMOCIONES ---
app.MapGet("/api/promociones", () => Results.Ok(promociones));

// --- RECURSO 4: SUCURSALES ---
app.MapGet("/api/sucursales", () => Results.Ok(sucursales));


var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");

// ==========================================
// MODELOS / RECORDS
// ==========================================

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

record Categoria(int Id, string Nombre, string Descripcion, string Imagen);
record Promocion(int Id, string Titulo, string Descripcion, int PorcentajeDescuento, bool Activo);
record Sucursal(int Id, string Nombre, string Direccion, string Horario, string Telefono);
