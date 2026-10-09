# Prueba técnica – Analista Programador Nivel I

Gracias por participar. Esta prueba se rinde sobre un proyecto .NET que ya funciona, pero que tiene
errores, malas prácticas y no tiene pruebas. Tu trabajo es mejorarlo.

- **Tiempo:** 80 minutos de trabajo desde la hora de inicio que te indique el evaluador, más 10 minutos
  para subir tu rama y abrir el Pull Request.
- **Puntaje:** 35 puntos (5 tareas obligatorias y 2 adicionales de bonus).
- **Después de entregar:** una revisión de 10 a 15 minutos con el evaluador, en la que explicas tu solución.

No buscamos perfección: queremos ver cómo entiendes código existente, cómo lo mejoras y cómo trabajas con Git.

---

## 1. Antes de empezar (hazlo el día anterior)

Necesitas tener instalado:

| Herramienta | Versión | Dónde obtenerla |
| --- | --- | --- |
| Git | cualquiera reciente | https://git-scm.com/downloads |
| .NET SDK | **8.0** o superior (el proyecto apunta a `net8.0` y corre también sobre .NET 9 o 10) | https://dotnet.microsoft.com/download/dotnet/8.0 |
| Visual Studio 2022 (17.8+) con la carga de trabajo **ASP.NET y desarrollo web**, o VS Code con la extensión C# Dev Kit | | https://visualstudio.microsoft.com/es/downloads/ |
| SQL Server LocalDB | viene con Visual Studio (componente "SQL Server Express LocalDB") | o instalador de SQL Server Express |
| Una cuenta de GitHub | | https://github.com/signup |

Comprueba en una terminal:

```
git --version
dotnet --version
sqllocaldb info
```

Si `sqllocaldb` no existe pero tienes SQL Server Express o Developer instalado, cambia la cadena de conexión
en `SemillaTickets/src/SemillaTickets.Web/appsettings.json` por la de tu instancia (por ejemplo
`Server=.\SQLEXPRESS;Database=SemillaTickets;Trusted_Connection=True;TrustServerCertificate=True`).
Ese cambio es válido y no afecta la calificación.

## 2. Obtener el proyecto (fork y clon)

No necesitas permisos sobre este repositorio: trabajas en una copia tuya.

1. Arriba a la derecha de esta página haz clic en **Fork** y acepta los valores por defecto.
   GitHub crea `https://github.com/<tu-usuario>/evaluator-system-seed-plan-analist-developer-n1`.
2. Clona **tu fork** (no este repositorio) y crea tu rama de trabajo:

   ```
   git clone https://github.com/<tu-usuario>/evaluator-system-seed-plan-analist-developer-n1.git
   cd evaluator-system-seed-plan-analist-developer-n1
   git switch -c candidato/<tu-apellido>
   git config user.name "<Tu nombre y apellido>"
   git config user.email "<tu-correo>"
   ```

   Usa tu apellido en minúsculas y sin tildes (por ejemplo `candidato/gutierrez`).
3. Restaura, compila, ejecuta la prueba de ejemplo y arranca la aplicación:

   ```
   cd SemillaTickets
   dotnet build
   dotnet test
   dotnet run --project src/SemillaTickets.Web
   ```

   Abre http://localhost:5180 : debes ver el listado con 7 tickets. La base de datos se crea sola la
   primera vez (en `(localdb)\MSSQLLocalDB`, base `SemillaTickets`).
   En Visual Studio puedes abrir `SemillaTickets/SemillaTickets.sln` y usar F5.

Haz todo esto **antes** de la hora de inicio. Si algo no funciona, escribe al evaluador con el mensaje
de error completo.

## 3. El proyecto

Es una aplicación **ASP.NET Core MVC** para registrar tickets de incidencias, con **SQL Server LocalDB** y
**Entity Framework Core**. Compila y arranca, pero tiene errores, malas prácticas y no tiene pruebas.

```
SemillaTickets/
├── SemillaTickets.sln
├── src/SemillaTickets.Web      → aplicación MVC (controladores, modelos, vistas, DbContext)
├── tests/SemillaTickets.Tests  → proyecto de pruebas xUnit (ya tiene EF Core InMemory instalado)
└── sql/reporte.sql             → archivo para la tarea 4
```

## 4. Reglas

- Trabaja solo en tu rama `candidato/<tu-apellido>`. No modifiques `main`.
- Haz **al menos un commit por tarea**, con un mensaje claro (por ejemplo: `Corrige guardado en Edit`).
- Puedes consultar documentación oficial (learn.microsoft.com). **No se permiten** asistentes de IA,
  código propio previo ni ayuda de terceros. En la revisión del PR se te pedirá explicar tu código.
- No borres ni desactives la prueba de ejemplo `EjemploTests.cs`.
- Si no terminas una tarea, haz commit de lo que tengas y explícalo en `PR.md`.

## 5. Tareas obligatorias

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

4. **Consulta SQL.** En `SemillaTickets/sql/reporte.sql` escribe una consulta que muestre, por prioridad:
   el total de tickets, cuántos están *Abiertos* o *En proceso*, y el tiempo promedio de resolución en horas
   de los *Resueltos* o *Cerrados*, ordenado de Critica a Baja.
   Puedes ejecutarla en SSMS o en Visual Studio (Explorador de objetos de SQL Server) contra
   `(localdb)\MSSQLLocalDB`, base de datos `SemillaTickets`.

5. **Pruebas unitarias.** Escribe al menos **2 pruebas xUnit** sobre tu servicio
   (por ejemplo, que no se acepte un título vacío y que un ticket Cerrado no cambie de estado).
   El proyecto de pruebas ya tiene instalado `Microsoft.EntityFrameworkCore.InMemory`.

## 6. Tareas adicionales (solo si terminaste las obligatorias)

- **A.** El equipo de seguridad reportó un riesgo en la búsqueda por título del listado. Identifícalo y corrígelo.
- **B.** Los usuarios reportan que la columna "SLA vencido" nunca marca los tickets críticos atrasados
  (el SLA de un ticket crítico es de 4 horas). Corrígelo.

## 7. Entrega (10 minutos al final)

1. Crea el archivo `PR.md` en la raíz del repositorio (junto a este README) con:
   - qué hiciste en cada tarea;
   - qué quedó pendiente;
   - por qué tomaste tus decisiones.
2. Haz el commit final y sube tu rama a tu fork:

   ```
   git add -A
   git commit -m "Agrega PR.md"
   git push -u origin candidato/<tu-apellido>
   ```

3. Abre un **Pull Request** hacia este repositorio:
   - Entra a tu fork en GitHub; verás el aviso "candidato/<tu-apellido> had recent pushes" con el botón
     **Compare & pull request** (o usa la pestaña *Pull requests* → *New pull request*).
   - Comprueba que la base sea `jefedesarrollotid-solutions/evaluator-system-seed-plan-analist-developer-n1`
     rama `main`, y el origen tu fork, rama `candidato/<tu-apellido>`.
   - Título: `Prueba técnica – <Apellido Nombre>`.
   - En la descripción, GitHub carga una plantilla: complétala (puedes pegar el contenido de tu `PR.md`).
   - Haz clic en **Create pull request**. No necesitas ningún permiso para hacerlo.
4. Envía el enlace del Pull Request al evaluador por el medio que te indicó.

El Pull Request **no se fusiona**: se usa solo para revisar tu trabajo. Al abrirlo se ejecuta
automáticamente una compilación y las pruebas; si la verificación falla, revisa el detalle, corrige y
vuelve a hacer push a la misma rama (el PR se actualiza solo).

Si tienes un problema con GitHub en el último momento, comprime la carpeta del repositorio
(**incluyendo la carpeta oculta `.git`**) y envíala al evaluador: el historial de commits forma parte de la evaluación.
