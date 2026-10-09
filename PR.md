# Prueba técnica – Mamani Cesar

## Postulante

- **Nombre y apellido:** Cesar Mamani
- **Rama:** `candidato/dominguez`
- **Entorno usado:** VS Code, .NET SDK 8.0.416, EF Core 8.0.31. Pruebas con EF Core InMemory (`dotnet build` y `dotnet test` en verde: 27 pruebas, incluida `EjemploTests`).

## Tareas obligatorias

| Tarea | Estado | Qué hice |
| --- | --- | --- |
| 1. Corrección del guardado y validaciones | Completa | El POST de `Edit` cargaba el ticket con `AsNoTracking()` y nunca llamaba a `SaveChanges()`, por eso "no se guardaban los cambios". Ahora se carga la entidad con seguimiento, se copian los campos y se guarda. Agregué Data Annotations en `Ticket` (`[Required]` y `[StringLength(100, MinimumLength = 5)]` en el título, `[RegularExpression]` para que la prioridad sea Baja/Media/Alta/Critica, `[StringLength(500)]` en la descripción). El controlador revisa `ModelState.IsValid` y devuelve la vista con el formulario; `Edit.cshtml` muestra los errores con `asp-validation-summary` y `asp-validation-for`, y carga `_ValidationScriptsPartial` para validar también en el navegador. Agregué `[ValidateAntiForgeryToken]` al POST. |
| 2. Capa de servicio e inyección de dependencias | Completa | Nueva carpeta `Services` con `ITicketService` y `TicketService` (`Listar`, `Obtener`, `Actualizar`, `CambiarEstado`). Se registra en `Program.cs` con `AddScoped<ITicketService, TicketService>()`. `TicketsController` recibe la interfaz por constructor y ya no referencia `AppDbContext`. El servicio vuelve a validar el modelo con `Validator.ValidateObject` (las mismas Data Annotations) antes de guardar. |
| 3a. Filtros por Estado y Prioridad | Completa | `Listar(buscar, estado, prioridad)` compone los `Where` que correspondan sobre un `IQueryable` y ordena con `OrderByDescending(FechaCreacion)` (desempate por `Id`). Creé los catálogos `Estados` y `Prioridades` con los valores permitidos y un `TicketsIndexViewModel` (filtros, lista y hora de referencia) que reemplaza el uso de `ViewData` y `Request.Query`. La vista tiene dos `select` combinables con la búsqueda, botón Limpiar y mensaje cuando no hay resultados. |
| 3b. Cambio de estado por Ajax | Completa | Cada fila tiene un `select` de estado (deshabilitado si el ticket está Cerrado). `site.js` envía con `fetch` un POST a `Tickets/CambiarEstado` con el token antifalsificación en la cabecera `RequestVerificationToken`; la acción devuelve JSON (`estado`, `fechaCierre`, `slaVencido`) y la fila se actualiza sin recargar. Las reglas viven en el servicio: un ticket Cerrado lanza `InvalidOperationException` (la acción responde 400 con el mensaje y el select vuelve al valor anterior); al pasar a Resuelto o Cerrado se registra `FechaCierre`. |
| 4. Consulta SQL (`sql/reporte.sql`) | Completa | `GROUP BY Prioridad` con `COUNT(*)`, `SUM(CASE ...)` para los Abiertos/En proceso y `AVG(CASE ... DATEDIFF(MINUTE, FechaCreacion, FechaCierre) / 60.0 END)` para el promedio de horas de los Resueltos/Cerrados (`AVG` ignora los `NULL`, así los pendientes no cuentan). El orden Critica → Baja se logra con un `CASE` en el `ORDER BY`. Con los datos semilla debe devolver: Critica 3 / 1 / 2.50; Alta 2 / 1 / 20.00; Media 1 / 1 / NULL; Baja 1 / 1 / NULL. |
| 5. Pruebas unitarias | Completa | `TicketServiceTests` (17 pruebas) con EF Core InMemory, una base por prueba: título vacío, título corto y prioridad inválida lanzan `ValidationException`; datos válidos persisten (verificado leyendo con otro contexto); un Cerrado no cambia de estado; Resuelto/Cerrado registran `FechaCierre`; reabrir la limpia; orden descendente; filtros combinados; búsqueda por título y búsqueda con comillas. `SlaHelperTests` (9 pruebas) para el bonus B. |

## Tareas adicionales

| Tarea | Estado | Qué hice |
| --- | --- | --- |
| A. Riesgo de seguridad en la búsqueda | Completa | La búsqueda concatenaba el texto del usuario dentro de `FromSqlRaw("... LIKE '%" + buscar + "%'")`: inyección SQL (con `' OR '1'='1` se listaba todo, y una comilla rompía la consulta). Lo reemplacé por LINQ `t.Titulo.Contains(texto)`, que EF Core traduce a un `LIKE` con parámetro, y agregué una prueba con ese texto malicioso que debe devolver vacío. |
| B. Columna "SLA vencido" | Completa | `SlaHelper` tenía dos errores: comparaba con `"Crítica"` (con tilde) cuando los datos guardan `"Critica"`, y usaba `TimeSpan.Hours` (componente de 0 a 23) en lugar de `TotalHours`, así que un ticket de 2 días daba 0 horas. Ahora usa `Prioridades.Critica`, `TotalHours` y una constante `HorasSlaCritico = 4`. Además, si el ticket ya tiene `FechaCierre` el tiempo se mide hasta esa fecha, para que un crítico resuelto en 2 horas no aparezca vencido días después. |

## Decisiones que tomé y por qué

- **Validación en dos capas.** Las reglas están una sola vez, como Data Annotations en `Ticket`. MVC las aplica al enlazar el formulario (`ModelState`) y el servicio las vuelve a aplicar con `Validator.ValidateObject`, de modo que las pruebas y cualquier otro consumidor del servicio también quedan protegidos sin duplicar reglas.
- **Tiempo de vida Scoped.** `TicketService` depende de `AppDbContext`, que es Scoped (uno por petición). Un Singleton compartiría el contexto entre peticiones y Transient crearía instancias de más sin beneficio.
- **Errores del servicio.** Datos inválidos lanzan `ValidationException`; una regla de negocio incumplida (ticket Cerrado) lanza `InvalidOperationException`; un ticket inexistente devuelve `null`. El controlador traduce eso a vista con errores, 400 con JSON o 404 según el caso.
- **`FechaCierre`.** Al pasar a Resuelto o Cerrado se registra con `??=`: si el ticket ya estaba Resuelto y se Cierra, se conserva la fecha original de resolución (es la que usa el reporte de tiempos). Al reabrir (Abierto / En proceso) se limpia para no tener un ticket abierto con fecha de cierre.
- **ViewModel en el listado.** Reemplacé `ViewData[$"sla_{id}"]` y `Context.Request.Query` por `TicketsIndexViewModel`: la vista recibe tipado lo que necesita y el SLA se calcula con una única hora de referencia.
- **Ajax con `fetch` nativo**, sin librerías extra. El `select` de un ticket Cerrado se deshabilita en el cliente por usabilidad, pero la regla se valida siempre en el servidor. Tras una respuesta correcta solo se actualiza la fila (estado, fecha de cierre y SLA).
- **SQL.** `DATEDIFF(MINUTE) / 60.0` en vez de `DATEDIFF(HOUR)` para no perder las fracciones de hora (3 h 30 min = 3.50). Solo se promedian filas Resueltas/Cerradas con `FechaCierre`.
- **Pruebas con InMemory** y una base con nombre distinto por prueba para que no compartan datos; no se llama `EnsureCreated()` para no cargar los datos semilla y controlar los casos de cada prueba.

## Qué quedó pendiente

Nada de lo pedido quedó pendiente. Con más tiempo haría:

- Métodos asíncronos en el servicio y el controlador (`ToListAsync`, `SaveChangesAsync`).
- Paginación del listado y conservar los filtros al volver desde Edit.
- Un `enum` para Estado y Prioridad en lugar de cadenas, con conversión en EF Core.

---

- [x] Creé el archivo `PR.md` en la raíz del repositorio.
- [x] Hice al menos un commit por tarea.
- [x] No borré ni desactivé la prueba de ejemplo `EjemploTests.cs`.
- [x] `dotnet build` y `dotnet test` pasan en mi máquina.
