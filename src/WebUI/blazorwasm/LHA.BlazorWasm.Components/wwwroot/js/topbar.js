export function initTopbar(dotNetHelper) {
    window.addEventListener('keydown', (e) => {
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            dotNetHelper.invokeMethodAsync('OpenCommandPalette');
        }
    });

    // Close dropdowns on click outside
    window.addEventListener('mousedown', (e) => {
        const topbar = document.querySelector('.topbar');
        const searchOverlay = document.querySelector('.mobile-search-overlay');
        
        // If clicking outside BOTH the topbar and the search overlay, close all
        if (topbar && !topbar.contains(e.target) && (!searchOverlay || !searchOverlay.contains(e.target))) {
            dotNetHelper.invokeMethodAsync('CloseDropdowns');
        }
    });
}
