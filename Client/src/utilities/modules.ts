export const loadModule = (src: string, onload: ((this: GlobalEventHandlers, ev: Event) => any) | null) => {
  const module = document.createElement('script');

  module.src = src;
  module.onload = onload;

  document.head.appendChild(module);
};
