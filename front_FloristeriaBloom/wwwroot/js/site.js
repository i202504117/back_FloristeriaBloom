// ===================== CARRITO - AJAX / JSON =====================

document.addEventListener('DOMContentLoaded', function () {
    actualizarContadorCarrito();

    document.querySelectorAll('.agregar-carrito-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var id = this.getAttribute('data-id');
            agregarAlCarrito(id, this);
        });
    });

    document.querySelectorAll('.quitar-carrito-btn').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            var lineId = this.getAttribute('data-line');
            quitarDelCarrito(lineId, this);
        });
    });

    document.querySelectorAll('.abrir-adicionales-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var lineId = this.getAttribute('data-line');
            abrirModalAdicionales(lineId);
        });
    });

    var btnConfirmarAdicionales = document.getElementById('btnConfirmarAdicionales');
    if (btnConfirmarAdicionales) {
        btnConfirmarAdicionales.addEventListener('click', confirmarAdicionales);
    }

    var btnGuardarMensaje = document.getElementById('btnGuardarMensaje');
    if (btnGuardarMensaje) {
        btnGuardarMensaje.addEventListener('click', guardarMensaje);
    }
});

function agregarAlCarrito(id, boton) {
    if (boton) boton.disabled = true;

    fetch('/Carrito/AgregarAjax', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ id: parseInt(id, 10) })
    })
        .then(function (res) { return res.json(); })
        .then(function (data) {
            if (data.requiereLogin) {
                window.location.href = '/Account/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
                return;
            }
            if (data.ok) {
                actualizarBadgeCarrito(data.totalItems);
                mostrarToast(data.mensaje, true);
            } else {
                mostrarToast(data.mensaje || 'No se pudo agregar el producto.', false);
            }
        })
        .catch(function () {
            mostrarToast('Error de conexión con el servidor.', false);
        })
        .finally(function () {
            if (boton) boton.disabled = false;
        });
}

function quitarDelCarrito(lineId, elemento) {
    fetch('/Carrito/EliminarAjax', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ lineId: lineId })
    })
        .then(function (res) { return res.json(); })
        .then(function (data) {
            if (data.ok) {
                actualizarBadgeCarrito(data.totalItems);

                var fila = elemento.closest('tr');
                if (fila) fila.remove();

                var totalEl = document.querySelector('.cart-total strong');
                if (totalEl) totalEl.textContent = 'S/ ' + data.totalPrecio.toFixed(2);

                if (data.totalItems === 0) {
                    window.location.reload();
                }
            }
        })
        .catch(function () {
            mostrarToast('Error de conexión con el servidor.', false);
        });
}

function actualizarContadorCarrito() {
    fetch('/Carrito/ContarItems')
        .then(function (res) { return res.json(); })
        .then(function (data) {
            actualizarBadgeCarrito(data.totalItems);
        })
        .catch(function () { /* sin sesión: el contador queda en 0 */ });
}

function actualizarBadgeCarrito(total) {
    document.querySelectorAll('.cart-count').forEach(function (el) {
        el.textContent = total;
    });
}

function mostrarToast(mensaje, exito) {
    var toast = document.createElement('div');
    toast.className = 'toast-flor' + (exito ? '' : ' toast-flor-error');
    toast.textContent = mensaje;
    document.body.appendChild(toast);

    requestAnimationFrame(function () { toast.classList.add('show'); });

    setTimeout(function () {
        toast.classList.remove('show');
        setTimeout(function () { toast.remove(); }, 300);
    }, 2200);
}

// ===================== ADICIONALES (por línea del carrito) =====================

var lineaAdicionalActual = null;

function abrirModalAdicionales(lineId) {
    lineaAdicionalActual = lineId;

    fetch('/Carrito/Adicionales')
        .then(function (res) { return res.json(); })
        .then(function (lista) {
            var cont = document.getElementById('listaAdicionalesModal');
            if (!cont) return;
            cont.innerHTML = '';

            if (lista.length === 0) {
                cont.innerHTML = '<p class="text-muted">Todavía no hay adicionales disponibles.</p>';
            }

            lista.forEach(function (a) {
                var div = document.createElement('div');
                div.className = 'form-check mb-2';
                div.innerHTML =
                    '<input class="form-check-input" type="checkbox" value="' + a.id + '" id="adic-' + a.id + '">' +
                    '<label class="form-check-label" for="adic-' + a.id + '">' +
                    a.nombre + ' (+S/ ' + Number(a.precio).toFixed(2) + ')</label>';
                cont.appendChild(div);
            });

            var modalEl = document.getElementById('modalAdicionales');
            if (modalEl && window.bootstrap) {
                new bootstrap.Modal(modalEl).show();
            }
        })
        .catch(function () {
            mostrarToast('No se pudieron cargar los adicionales.', false);
        });
}

function confirmarAdicionales() {
    var seleccionados = Array.from(document.querySelectorAll('#listaAdicionalesModal input:checked'))
        .map(function (cb) { return parseInt(cb.value, 10); });

    fetch('/Carrito/AgregarAdicionalesAjax', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ lineId: lineaAdicionalActual, adicionalesIds: seleccionados })
    })
        .then(function (res) { return res.json(); })
        .then(function (data) {
            if (data.ok) {
                window.location.reload(); // refresca la tabla del carrito con los adicionales y totales nuevos
            } else {
                mostrarToast(data.mensaje || 'No se pudieron guardar los adicionales.', false);
            }
        });
}

// ===================== MENSAJE / DEDICATORIA =====================

function guardarMensaje() {
    var ocasionEl = document.getElementById('mensajeOcasion');
    var mensajeEl = document.getElementById('mensajeTexto');
    var firmaEl = document.getElementById('mensajeFirma');
    var anonimoEl = document.getElementById('mensajeAnonimo');

    fetch('/Carrito/GuardarMensajeAjax', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            ocasion: ocasionEl ? ocasionEl.value : null,
            mensaje: mensajeEl ? mensajeEl.value : null,
            firma: firmaEl ? firmaEl.value : null,
            anonimo: anonimoEl ? anonimoEl.checked : false
        })
    })
        .then(function (res) { return res.json(); })
        .then(function (data) {
            if (data.ok) {
                mostrarToast('Mensaje guardado para tu pedido', true);
            } else {
                mostrarToast('No se pudo guardar el mensaje.', false);
            }
        })
        .catch(function () {
            mostrarToast('Error de conexión con el servidor.', false);
        });
}