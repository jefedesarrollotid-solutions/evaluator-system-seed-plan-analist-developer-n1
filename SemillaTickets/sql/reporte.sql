-- Tarea 4: reporte de tickets por prioridad.
-- Para cada prioridad muestra:
--   * TotalTickets            : cantidad total de tickets.
--   * Pendientes              : cuántos están Abiertos o En proceso.
--   * HorasPromedioResolucion : promedio, en horas, de (FechaCierre - FechaCreacion)
--                               de los tickets Resueltos o Cerrados. NULL si ninguno se resolvió.
-- Ordenado de Critica a Baja.

SELECT
    t.Prioridad,
    COUNT(*)                                                               AS TotalTickets,
    SUM(CASE WHEN t.Estado IN ('Abierto', 'En proceso') THEN 1 ELSE 0 END) AS Pendientes,
    ROUND(AVG(CASE
                  WHEN t.Estado IN ('Resuelto', 'Cerrado') AND t.FechaCierre IS NOT NULL
                  THEN DATEDIFF(MINUTE, t.FechaCreacion, t.FechaCierre) / 60.0
              END), 2)                                                     AS HorasPromedioResolucion
FROM dbo.Tickets AS t
GROUP BY t.Prioridad
ORDER BY CASE t.Prioridad
             WHEN 'Critica' THEN 1
             WHEN 'Alta'    THEN 2
             WHEN 'Media'   THEN 3
             WHEN 'Baja'    THEN 4
             ELSE 5
         END;
