'use strict';
document.addEventListener('socio:fila', (e) => {
  const { socio, acciones } = e.detail;
  const boton = Api.el('button', { className: 'danger', textContent: 'Eliminar' });
  boton.onclick = async () => {
    if (!confirm(`¿Eliminar a ${socio.firstName} ${socio.lastName}?`)) return;
    try {
      await Api.request('DELETE', `/api/members/${socio.id}`);
      Api.message('Socio eliminado', true);
      await cargarSocios();
    } catch (err) {
      Api.message(err.message);
    }
  };
  acciones.append(boton);
});
