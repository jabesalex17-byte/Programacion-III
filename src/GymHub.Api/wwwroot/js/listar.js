'use strict';
const cuerpo = document.getElementById('socios');
const buscador = document.getElementById('search');

async function cargarSocios() {
  const q = encodeURIComponent(buscador.value);
  const socios = await Api.request('GET', `/api/members?q=${q}`);
  cuerpo.replaceChildren();
  for (const s of socios) {
    const acciones = Api.el('td', { className: 'actions' });
    const fila = Api.el('tr', {},
      ...[s.firstName, s.lastName, s.age, s.email, s.phoneNumber ?? '', s.active ? 'Sí' : 'No']
        .map((valor) => Api.el('td', { textContent: valor })),
      acciones);
    cuerpo.append(fila);
    document.dispatchEvent(new CustomEvent('socio:fila', { detail: { socio: s, acciones } }));
  }
  document.getElementById('empty').hidden = socios.length > 0;
}

buscador.addEventListener('input', () => cargarSocios().catch((e) => Api.message(e.message)));
cargarSocios().catch((e) => Api.message(e.message));
