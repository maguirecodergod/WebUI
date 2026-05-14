/** @type {Function|null} */
let _keydownHandler = null;

/** @type {Function|null} */
let _mousedownHandler = null;

export function initTopbar(dotNetHelper) {
    // Store handler references so they can be removed later
    _keydownHandler = (e) => {
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            if (dotNetHelper) {
                dotNetHelper.invokeMethodAsync('OpenCommandPalette');
            }
        }
    };

    _mousedownHandler = (e) => {
        if (!dotNetHelper) return;
        
        const topbar = document.querySelector('.topbar');
        const palette = document.querySelector('.command-palette-wrapper');
        
        // If clicking outside BOTH the topbar and the search palette, close all
        if (topbar && !topbar.contains(e.target) && (!palette || !palette.contains(e.target))) {
            dotNetHelper.invokeMethodAsync('CloseDropdowns');
        }
    };

    window.addEventListener('keydown', _keydownHandler);
    window.addEventListener('mousedown', _mousedownHandler);
}

/**
 * Cleans up all event listeners to prevent memory leaks and disposed reference errors.
 * Called from Blazor's Dispose() method.
 */
export function dispose() {
    if (_keydownHandler) {
        window.removeEventListener('keydown', _keydownHandler);
        _keydownHandler = null;
    }

    if (_mousedownHandler) {
        window.removeEventListener('mousedown', _mousedownHandler);
        _mousedownHandler = null;
    }
}
