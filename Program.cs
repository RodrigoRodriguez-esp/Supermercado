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

// ---------- "Base de datos" en memoria ----------
var productos = new List<Producto>
{
    new(1,  "P001", "Leche entera 1L",        "Lácteos",   4.50m,  40),
    new(2,  "P002", "Yogurt fresa 1L",        "Lácteos",   6.80m,  25),
    new(3,  "P003", "Queso fresco 250g",      "Lácteos",   9.90m,  15),
    new(4,  "P004", "Arroz extra 1kg",        "Abarrotes", 4.20m,  80),
    new(5,  "P005", "Fideos spaghetti 500g",  "Abarrotes", 3.10m,  60),
    new(6,  "P006", "Aceite vegetal 1L",      "Abarrotes", 10.50m, 35),
    new(7,  "P007", "Manzana (kg)",           "Frutas",    5.50m,  50),
    new(8,  "P008", "Plátano (kg)",           "Frutas",    3.20m,  70),
    new(9,  "P009", "Papa blanca (kg)",       "Verduras",  2.80m,  90),
    new(10, "P010", "Tomate (kg)",            "Verduras",  3.90m,  45),
    new(11, "P011", "Pollo entero (kg)",      "Carnes",    11.90m, 20),
    new(12, "P012", "Gaseosa 2L",             "Bebidas",   7.50m,  55),
    new(13, "P013", "Agua mineral 625ml",     "Bebidas",   1.80m,  100),
    new(14, "P014", "Detergente 1kg",         "Limpieza",  12.90m, 30),
};

var siguienteId = productos.Max(p => p.Id) + 1;
var candado = new object();

string? Validar(NuevoProducto p)
{
    if (string.IsNullOrWhiteSpace(p.Codigo)) return "El código es obligatorio.";
    if (string.IsNullOrWhiteSpace(p.Nombre)) return "El nombre es obligatorio.";
    if (string.IsNullOrWhiteSpace(p.Categoria)) return "La categoría es obligatoria.";
    if (p.Precio < 0) return "El precio no puede ser negativo.";
    if (p.Stock < 0) return "El stock no puede ser negativo.";
    return null;
}

// ---------- Rutas ----------
app.MapGet("/", () => "API Supermercado funcionando");

// Listar productos. Filtros opcionales: ?categoria=Lácteos&buscar=leche
app.MapGet("/api/productos", (string? categoria, string? buscar) =>
{
    lock (candado)
    {
        IEnumerable<Producto> resultado = productos;

        if (!string.IsNullOrWhiteSpace(categoria))
            resultado = resultado.Where(p =>
                p.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(buscar))
            resultado = resultado.Where(p =>
                p.Nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase));

        return Results.Ok(resultado.ToList());
    }
});

// Obtener un producto por id
app.MapGet("/api/productos/{id:int}", (int id) =>
{
    lock (candado)
    {
        var producto = productos.FirstOrDefault(p => p.Id == id);
        return producto is null
            ? Results.NotFound(new { mensaje = $"No existe el producto {id}." })
            : Results.Ok(producto);
    }
});

// Listar categorías disponibles
app.MapGet("/api/categorias", () =>
{
    lock (candado)
    {
        return Results.Ok(productos
            .Select(p => p.Categoria)
            .Distinct()
            .OrderBy(c => c)
            .ToList());
    }
});

// Crear producto
app.MapPost("/api/productos", (NuevoProducto datos) =>
{
    var error = Validar(datos);
    if (error is not null) return Results.BadRequest(new { mensaje = error });

    lock (candado)
    {
        if (productos.Any(p => p.Codigo.Equals(datos.Codigo, StringComparison.OrdinalIgnoreCase)))
            return Results.Conflict(new { mensaje = $"Ya existe un producto con el código {datos.Codigo}." });

        var nuevo = new Producto(siguienteId++, datos.Codigo, datos.Nombre,
                                 datos.Categoria, datos.Precio, datos.Stock);
        productos.Add(nuevo);
        return Results.Created($"/api/productos/{nuevo.Id}", nuevo);
    }
});

// Actualizar producto
app.MapPut("/api/productos/{id:int}", (int id, NuevoProducto datos) =>
{
    var error = Validar(datos);
    if (error is not null) return Results.BadRequest(new { mensaje = error });

    lock (candado)
    {
        var indice = productos.FindIndex(p => p.Id == id);
        if (indice < 0)
            return Results.NotFound(new { mensaje = $"No existe el producto {id}." });

        if (productos.Any(p => p.Id != id &&
                               p.Codigo.Equals(datos.Codigo, StringComparison.OrdinalIgnoreCase)))
            return Results.Conflict(new { mensaje = $"Ya existe otro producto con el código {datos.Codigo}." });

        var actualizado = new Producto(id, datos.Codigo, datos.Nombre,
                                       datos.Categoria, datos.Precio, datos.Stock);
        productos[indice] = actualizado;
        return Results.Ok(actualizado);
    }
});

// Eliminar producto
app.MapDelete("/api/productos/{id:int}", (int id) =>
{
    lock (candado)
    {
        var producto = productos.FirstOrDefault(p => p.Id == id);
        if (producto is null)
            return Results.NotFound(new { mensaje = $"No existe el producto {id}." });

        productos.Remove(producto);
        return Results.NoContent();
    }
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");

// ---------- Modelos (siempre al final del archivo) ----------
record Producto(int Id, string Codigo, string Nombre, string Categoria, decimal Precio, int Stock);

record NuevoProducto(string Codigo, string Nombre, string Categoria, decimal Precio, int Stock);
