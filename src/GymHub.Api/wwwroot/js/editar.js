'use strict';
const form = document.getElementById('socio-form');
const idOculto = document.getElementById('socio-id');
const botonGuardar = document.getElementById('guardar');
const botonCancelar = document.getElementById('cancelar');

function salirDeEdicion() {
  form.reset();
  document.getElementById('f-active').checked = true;
  idOculto.value = '';
  botonGuardar.textContent = 'Agregar socio';
  botonCancelar.hidden = true;
  document.getElementById('form-title').textContent = 'Nuevo socio';
}

function entrarEnEdicion(socio) {
  idOculto.value = socio.id;
  for (const campo of ['firstName', 'lastName', 'age', 'email', 'phoneNumber'])
    document.getElementById(`f-${campo}`).value = socio[campo] ?? '';
  document.getElementById('f-active').checked = socio.active;
  botonGuardar.textContent = 'Guardar cambios';
  botonCancelar.hidden = false;
  document.getElementById('form-title').textContent = `Editando a ${socio.firstName} ${socio.lastName}`;
  document.getElementById('f-firstName').focus();
}

document.addEventListener('socio:fila', (e) => {
  const { socio, acciones } = e.detail;
  const editar = Api.el('button', { className: 'secondary', textContent: 'Editar' });
  editar.onclick = () => entrarEnEdicion(socio);
  const estado = Api.el('button', { className: 'secondary', textContent: socio.active ? 'Desactivar' : 'Activar' });
  estado.onclick = async () => {
    try {
      await Api.request('PATCH', `/api/members/${socio.id}/status`, { active: !socio.active });
      Api.message('Estado actualizado', true);
      await cargarSocios();
    } catch (err) {
      Api.message(err.message);
    }
  };
  acciones.append(editar, estado);
});

form.addEventListener('submit', async (e) => {
  e.preventDefault();
  const id = idOculto.value;
  if (!id) return;
  try {
    await Api.request('PUT', `/api/members/${id}`, leerFormulario());
    Api.message('Cambios guardados', true);
    salirDeEdicion();
    await cargarSocios();
  } catch (err) {
    Api.message(err.message);
  }
});

botonCancelar.addEventListener('click', salirDeEdicion);
