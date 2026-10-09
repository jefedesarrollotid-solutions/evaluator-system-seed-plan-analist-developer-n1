# SemillaTickets

Las instrucciones completas de la prueba (requisitos, flujo de fork y entrega, reglas y tareas)
están en el [README de la raíz del repositorio](../README.md).

```
SemillaTickets.sln
├── src/SemillaTickets.Web      → aplicación MVC
├── tests/SemillaTickets.Tests  → proyecto de pruebas xUnit
└── sql/reporte.sql             → archivo para la tarea 4
```

Arranque rápido:

```
dotnet build
dotnet test
dotnet run --project src/SemillaTickets.Web
```

La aplicación responde en http://localhost:5180 y crea sola la base de datos `SemillaTickets`
en `(localdb)\MSSQLLocalDB` la primera vez que arranca.
