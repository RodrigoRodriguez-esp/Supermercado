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

// ==================== PRODUCTOS ====================
var productos = new List<Producto>
{
    new(1, "LAC001", "Leche entera 1L",   "Lácteos",   4.50m,  40, "Gloria",        0,  "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRLDPnlDEQz5U98aFqk5Ys2hnRaT9Uj7X9fRm-qEL5c9mbM1jn9Y0z97hI&s=10",      "Leche entera en envase de 1 litro."),
    new(2, "LAC002", "Yogurt fresa 1L",   "Lácteos",   6.80m,  8,  "Laive",         10, "https://vegaperu.vtexassets.com/arquivos/ids/159174/7750151005548.jpg?v=637660223633330000",     "Yogurt bebible sabor fresa."),
    new(3, "ABA001", "Arroz extra 1kg",   "Abarrotes", 4.20m,  80, "Costeño",       0,  "https://plazavea.vteximg.com.br/arquivos/ids/27552446-512-512/433778.jpg,      "Arroz extra de grano largo."),
    new(4, "ABA002", "Aceite vegetal 1L", "Abarrotes", 10.50m, 35, "Primor",        5,  "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQwB_OrMatGEJ1bAnLAnkN6luP1IMCyg3otjjUgL3I6yPYs1xT_T5GWZaxk&s=10",     "Aceite vegetal para cocina."),
    new(5, "FRU001", "Manzana (kg)",      "Frutas",    5.50m,  50, "Fresco",        0,  "https://www.shutterstock.com/shutterstock/photos/2088601369/display_1500/stock-photo-a-lot-of-apples-in-a-cardboard-box-image-of-fruit-delivery-2088601369.jpg",    "Manzana roja por kilo."),
    new(6, "BEB001", "Gaseosa 2L",        "Bebidas",   7.50m,  6,  "Inca Kola",     0,  "https://www.popeyes.com.pe/media/catalog/product/2/1/2146464550.png?optimize=medium&bg-color=255,255,255&fit=bounds&height=700&width=700&canvas=700:700&format=jpeg",    "Gaseosa de 2 litros."),
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

// ==================== CATEGORÍAS ====================
var categorias = new List<Categoria>
{
    new(1, "Lácteos", "Productos derivados de la leche"),
    new(2, "Abarrotes", "Productos de despensa básicos"),
    new(3, "Frutas", "Frutas frescas por kilo"),
    new(4, "Bebidas", "Gaseosas, jugos y aguas"),
    new(5, "Carnes", "Carnes frescas y embutidos"),
    new(6, "Limpieza", "Artículos de limpieza del hogar")
};

app.MapGet("/api/categorias", () => categorias);
app.MapGet("/api/categorias/{id:int}", (int id) => {
    var cat = categorias.FirstOrDefault(c => c.Id == id);
    return cat is null ? Results.NotFound(new { message = "Categoría no encontrada" }) : Results.Ok(cat);
});

// ==================== CLIENTES ====================
var clientes = new List<Cliente>
{
    new(1, "Juan Pérez", "12345678", "juan@email.com", "987654321"),
    new(2, "María López", "87654321", "maria@email.com", "912345678")
};
var siguienteClienteId = 3;

app.MapGet("/api/clientes", () => clientes);
app.MapGet("/api/clientes/{id:int}", (int id) => {
    var cli = clientes.FirstOrDefault(c => c.Id == id);
    return cli is null ? Results.NotFound(new { message = "Cliente no encontrado" }) : Results.Ok(cli);
});
app.MapPost("/api/clientes", (Cliente datos) => {
    var nuevo = datos with { Id = siguienteClienteId++ };
    clientes.Add(nuevo);
    return Results.Created($"/api/clientes/{nuevo.Id}", nuevo);
});

// ==================== PEDIDOS ====================
var pedidos = new List<Pedido>();
var siguientePedidoId = 1;

app.MapGet("/api/pedidos", () => pedidos);
app.MapGet("/api/pedidos/{id:int}", (int id) => {
    var ped = pedidos.FirstOrDefault(p => p.Id == id);
    return ped is null ? Results.NotFound(new { message = "Pedido no encontrado" }) : Results.Ok(ped);
});
app.MapPost("/api/pedidos", (Pedido datos) => {
    var nuevo = datos with { Id = siguientePedidoId++ };
    pedidos.Add(nuevo);
    return Results.Created($"/api/pedidos/{nuevo.Id}", nuevo);
});

// ==================== PROMOCIONES ====================
var promociones = new List<Promocion>
{
    new(1, "Semana del Pollo", "10% de descuento en pollo entero", 10, new DateTime(2026, 10, 1), new DateTime(2026, 10, 10)),
    new(2, "Lácteos Frescos", "15% en yogurt Laive", 15, new DateTime(2026, 10, 5), new DateTime(2026, 10, 15))
};

app.MapGet("/api/promociones", () => promociones);
app.MapGet("/api/promociones/{id:int}", (int id) => {
    var promo = promociones.FirstOrDefault(p => p.Id == id);
    return promo is null ? Results.NotFound(new { message = "Promoción no encontrada" }) : Results.Ok(promo);
});

// ==================== CONFIGURACIÓN DE PUERTO ====================
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");

// ==================== MODELOS (RECORDS) — SIEMPRE AL FINAL ====================
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

record Categoria(int Id, string Nombre, string Descripcion);
record Cliente(int Id, string Nombre, string Dni, string Email, string Telefono);
record Pedido(int Id, int ClienteId, DateTime Fecha, decimal Total, List<DetallePedido> Detalles);
record DetallePedido(int ProductoId, string NombreProducto, int Cantidad, decimal PrecioUnitario);
record Promocion(int Id, string Titulo, string Descripcion, int DescuentoPorcentaje, DateTime FechaInicio, DateTime FechaFin);
