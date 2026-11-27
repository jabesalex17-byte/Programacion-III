'use strict';
const Api = {
  async request(method, url, body) {
    const res = await fetch(url, {
      method,
      headers: { 'Content-Type': 'application/json' },
      body: body ? JSON.stringify(body) : undefined,
    });
    const data = res.status === 204 ? null : await res.json();
    if (!res.ok) throw new Error((data.errors || [data.error]).join(' · '));
    return data;
  },

  message(text, ok = false) {
    const el = document.getElementById('message');
    el.textContent = text || '';
    el.className = ok ? 'ok' : '';
  },

  el(tag, props = {}, ...children) {
    const node = Object.assign(document.createElement(tag), props);
    node.append(...children);
    return node;
  },
};
