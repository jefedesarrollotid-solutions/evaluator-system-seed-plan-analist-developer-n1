// Scripts del sitio.

// Cambio de estado de un ticket sin recargar la página (listado de tickets).
document.addEventListener("DOMContentLoaded", function () {
    var cuerpo = document.getElementById("tabla-tickets");
    if (!cuerpo) {
        return;
    }

    var url = cuerpo.dataset.urlCambiarEstado;
    var tokenInput = document.querySelector('input[name="__RequestVerificationToken"]');
    var token = tokenInput ? tokenInput.value : "";
    var mensaje = document.getElementById("mensaje-estado");

    function mostrarMensaje(texto, tipo) {
        mensaje.textContent = texto;
        mensaje.className = "alert alert-" + tipo;
    }

    cuerpo.addEventListener("change", async function (evento) {
        var select = evento.target;
        if (!select.classList.contains("cambiar-estado")) {
            return;
        }

        var fila = select.closest("tr");
        var id = select.dataset.id;
        var estadoAnterior = select.dataset.estadoActual;
        var nuevoEstado = select.value;

        select.disabled = true;

        try {
            var respuesta = await fetch(url, {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded",
                    "RequestVerificationToken": token
                },
                body: new URLSearchParams({ id: id, estado: nuevoEstado })
            });

            var datos = await respuesta.json().catch(function () { return {}; });

            if (!respuesta.ok) {
                select.value = estadoAnterior;
                mostrarMensaje(datos.error || "No se pudo cambiar el estado.", "danger");
                return;
            }

            select.dataset.estadoActual = datos.estado;
            fila.querySelector(".fecha-cierre").textContent = datos.fechaCierre || "—";
            fila.querySelector(".sla-vencido").textContent = datos.slaVencido ? "Sí" : "No";
            mostrarMensaje("Ticket #" + id + " ahora está en estado " + datos.estado + ".", "success");
        } catch (error) {
            select.value = estadoAnterior;
            mostrarMensaje("Error de red al cambiar el estado.", "danger");
        } finally {
            // Un ticket Cerrado queda bloqueado: no puede volver a cambiar de estado.
            select.disabled = select.dataset.estadoActual === "Cerrado";
        }
    });
});
