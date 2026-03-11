export function autoAlignDropdown(container) {
    if (!container) return;
    const menu = container.querySelector('.lang-dropdown-menu');
    if (!menu) return;

    // Reset alignment to default (Left) to measure correctly
    menu.classList.remove('lang-dropdown-right');

    const rect = menu.getBoundingClientRect();
    const viewportWidth = window.innerWidth || document.documentElement.clientWidth;

    // If the right edge of the menu goes beyond the viewport, align it to the right.
    // We add a small 10px buffer to prevent it from touching the absolute edge.
    if (rect.right > (viewportWidth - 10)) {
        menu.classList.add('lang-dropdown-right');
    }
}
