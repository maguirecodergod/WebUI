export function initTopbar(dotNetHelper) {
    window.addEventListener('keydown', (e) => {
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            dotNetHelper.invokeMethodAsync('OpenCommandPalette');
        }
    });

    // Close dropdowns on click outside
    window.addEventListener('click', (e) => {
        const topbar = document.querySelector('.topbar');
        if (topbar && !topbar.contains(e.target)) {
            dotNetHelper.invokeMethodAsync('CloseDropdowns');
        }
    });
}
