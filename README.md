# TechStoreSA
TechStoreSA es una aplicación web para la gestión de inventarios y órdenes de compra de productos electrónicos en una tienda en línea. Desarrollada con Blazor WebAssembly para el frontend y ASP.NET Core para el backend, la aplicación permite a los administradores gestionar el inventario de productos y visualizar y procesar órdenes de compra.

## Características
- Gestión de Productos: Crear, editar, eliminar y consultar productos en el inventario.
- Gestión de Órdenes: Crear, actualizar, eliminar y consultar órdenes de compra, incluyendo detalles de los productos ordenados.
- Búsqueda Avanzada: Buscar productos por nombre y descripción.
- Paginación: Navegación fluida a través de listados paginados de productos y órdenes.
- Notificaciones: Notificaciones amigables y efectivas utilizando SweetAlert.

## Tecnologías Utilizadas
- **Frontend**: Blazor WebAssembly
- **Backend**: ASP.NET Core Web API
- **Base de Datos**: Entity Framework Core con SQL Server
- **UI/UX**: Bootstrap
- **Notificaciones**: SweetAlert2

## Requisitos Previos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Clonar el Repositorio

```bash
git clone https://github.com/HansSanchez/TechStoreSA.git
```

### Navega al proyecto del backend:

```bash
cd TechStoreSA
cd TechStoreSA.Server
```

## Configuración del Backend

### Configura la cadena de conexión a la base de datos en appsettings.json:
```bash
{
  "ConnectionStrings": {
    "connection_string": "Server=(local); Database=DB_TechStoreSA; User Id=TU_USUARIO; Password=TU_CONTRASEÑA; TrustServerCertificate=True;"
  },
}
```

### Aplica las migraciones de la base de datos:

```bash
dotnet ef database update
```

### Ejecuta el backend:
```bash
dotnet run
```

## Configuración del Frontend

### Navega al proyecto del frontend:

```bash
cd TechStoreSA.Client
```

### Ejecuta el frontend:
```bash
dotnet run
```


### Navegación
Una vez que ambos proyectos estén en ejecución, abre tu navegador y navega a `https://localhost:5163` para ver la aplicación en funcionamiento.

## Estructura del Proyecto
```bash
TechStoreSA/
├── TechStoreSA.Client/           # Proyecto del frontend (Blazor)
├── TechStoreSA.Server/           # Proyecto del backend (ASP.NET Core)
├── TechStoreSA.Shared/           # Clases y modelos compartidos
├── .gitignore                    # Archivos y carpetas ignorados por Git
├── README.md                     # Documentación del proyecto
└── TechStoreSA.sln               # Solución de Visual Studio
```

## API Endpoints

### ProductController
- GET /api/product/getProduct: Obtiene todos los productos con paginación y búsqueda.
- GET /api/product/getByIdProduct/{id}: Obtiene un producto por su ID.
- POST /api/product/storeProduct: Crea un nuevo producto.
- PUT /api/product/updateProduct/{id}: Actualiza un producto existente.
- DELETE /api/product/deleteProduct/{id}: Elimina un producto.

### OrderController
- GET /api/order/getOrder: Obtiene todas las órdenes con paginación y búsqueda.
- GET /api/order/getByIdOrder/{id}: Obtiene una orden por su ID.
- POST /api/order/storeOrder: Crea una nueva orden.
- PUT /api/order/updateOrder/{id}: Actualiza una orden existente.
- DELETE /api/order/deleteOrder/{id}: Elimina una orden.

### OrderDetailController
- GET /api/orderdetail/getOrderDetails/{orderId}: Obtiene los detalles de una orden específica.
- GET /api/orderdetail/getByIdOrderDetail/{id}: Obtiene un detalle de orden por su ID.
- POST /api/orderdetail/storeOrderDetail: Crea un nuevo detalle de orden.
- PUT /api/orderdetail/updateOrderDetail/{id}: Actualiza un detalle de orden existente.
- DELETE /api/orderdetail/deleteOrderDetail/{id}: Elimina un detalle de orden.

## Ejemplos de Uso

### Crear un Nuevo Producto
1. Haz clic en el botón "Nuevo producto".
2. Rellena el formulario con los detalles del producto.
3. Haz clic en "Guardar" para añadir el producto a la base de datos.

### Crear una Nueva Orden
1. Navega a la sección de "Órdenes".
2. Haz clic en "Nueva Orden".
3. Selecciona los productos, define las cantidades y completa la información requerida.
4. Haz clic en "Guardar" para crear la orden.

### Navegar por Páginas

1. Utiliza los botones "Anterior" y "Siguiente" para navegar por las páginas de la tabla.

## Contacto
Para cualquier consulta o sugerencia, por favor contacta a [hanssanchez427@gmail.com](mailto:hanssanchez427@gmail.com).
