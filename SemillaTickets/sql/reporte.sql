-- Tarea 4: escribe aquí tu consulta.
SELECT 
	Prioridad,
	COUNT (*) AS Total,
	SUM(CASE WHEN Estado IN('Abierto', 'En Proceso') THEN 1 ELSE 0 END) AS Pendientes
	CAST(AVG(CASE WHEN Estado IN ('Resuelto', 'Cerrado') 
		THEN DATEDIFF (MINUTE, FechaCreacion, FechaCerrado)/60.0
					decimal(10,2)) ELSE 0) AS PromedioHoras

FROM Tickets
GROUP BY Prioridad
ORDER BY Prioridad

	-- WHEN CASE 'Critica' THEN 1
	-- WHEN CASE 'Alta' THEN 1
	-- WHEN CASE 'Media' THEN 1
	-- WHEN CASE 'Baja' THEN 1

	WHEN CASE 'Critica' THEN 1
	WHEN CASE 'Alta' THEN 2
	WHEN CASE 'Media' THEN 3
	WHEN CASE 'Baja' THEN 4

	END;