# Prueba técnica – Analista Programador Nivel I

Gracias por participar. Tienes **80 minutos** de trabajo y **3 minutos** al final para explicar tu solución.
No buscamos perfección: queremos ver cómo entiendes código existente, cómo lo mejoras y cómo trabajas con Git.

## El proyecto

Es una aplicación **ASP.NET Core MVC** para registrar tickets de incidencias, con **SQL Server LocalDB** y **Entity Framework Core**.
Compila y arranca con **F5**, pero tiene errores, malas prácticas y no tiene pruebas.
La base de datos se crea sola al iniciar la aplicación y ya tiene datos de ejemplo.

```
SemillaTickets.sln
├── src/SemillaTickets.Web      → aplicación MVC
├── tests/SemillaTickets.Tests  → proyecto de pruebas xUnit
└── sql/reporte.sql             → archivo para la tarea 4
```

## Reglas

- Trabaja en la rama ya creada `candidato/<tu-apellido>`. No cambies de rama.
- Haz **al menos un commit por tarea**, con un mensaje claro (por ejemplo: `Corrige guardado en Edit`).
- Puedes consultar learn.microsoft.com. No se permiten asistentes de IA, código propio previo ni ayuda de terceros.
- Si no terminas una tarea, haz commit de lo que tengas y explícalo en `PR.md`.

## Tareas obligatorias

1. **Corrección de error.** Los usuarios reportan que al editar un ticket "no se guardan los cambios".
   Además, hoy se puede guardar un ticket sin título. Corrige el guardado y valida que:
   - el título sea obligatorio y tenga entre 5 y 100 caracteres;
   - la prioridad sea Baja, Media, Alta o Critica.

   Los errores deben mostrarse en la pantalla de edición.

2. **Refactorización.** El controlador accede directamente a la base de datos.
   Mueve esa lógica a una capa de servicio y úsala desde el controlador mediante inyección de dependencias.

3. **Nueva funcionalidad en el listado.**
   - a) Filtros por **Estado** y por **Prioridad** (combinables), con los tickets ordenados del más reciente al más antiguo.
   - b) Un control para cambiar el estado de un ticket **sin recargar la página** (Ajax).
     Reglas: un ticket *Cerrado* no puede cambiar de estado; al pasar a *Resuelto* o *Cerrado* se registra `FechaCierre`.

4. **Consulta SQL.** En `sql/reporte.sql` escribe una consulta que muestre, por prioridad:
   el total de tickets, cuántos están *Abiertos* o *En proceso*, y el tiempo promedio de resolución en horas
   de los *Resueltos* o *Cerrados*, ordenado de Critica a Baja.
   Puedes ejecutarla en SSMS contra `(localdb)\MSSQLLocalDB`, base de datos `SemillaTickets`.

5. **Pruebas unitarias.** Escribe al menos **2 pruebas xUnit** sobre tu servicio
   (por ejemplo, que no se acepte un título vacío y que un ticket Cerrado no cambie de estado).
   El proyecto de pruebas ya tiene instalado `Microsoft.EntityFrameworkCore.InMemory`.

## Tareas adicionales (solo si terminaste las obligatorias)

- **A.** El equipo de seguridad reportó un riesgo en la búsqueda por título del listado. Identifícalo y corrígelo.
- **B.** Los usuarios reportan que la columna "SLA vencido" nunca marca los tickets críticos atrasados
  (el SLA de un ticket crítico es de 4 horas). Corrígelo.

## Entrega

Crea el archivo `PR.md` en la raíz con:

- qué hiciste en cada tarea;
- qué quedó pendiente;
- por qué tomaste tus decisiones.

Haz el commit final y avisa al evaluador. Él subirá tu rama y abrirá el Pull Request.
