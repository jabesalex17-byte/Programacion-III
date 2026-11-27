'use strict';
const formulario = document.getElementById('socio-form');
const CAMPOS = ['firstName', 'lastName', 'age', 'email', 'phoneNumber'];

function leerFormulario() {
  const datos = {};
  for (const campo of CAMPOS) {
    const valor = document.getElementById(`f-${campo}`).value;
    if (valor !== '') datos[campo] = valor;
  }
  datos.active = document.getElementById('f-active').checked;
  return datos;
}

formulario.addEventListener('submit', async (e) => {
  e.preventDefault();
  if (document.getElementById('socio-id').value) return;
  try {
    await Api.request('POST', '/api/members', leerFormulario());
    Api.message('Socio agregado', true);
    formulario.reset();
    document.getElementById('f-active').checked = true;
    await cargarSocios();
  } catch (err) {
    Api.message(err.message);
  }
});
