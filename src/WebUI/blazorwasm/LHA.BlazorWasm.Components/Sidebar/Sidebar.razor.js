/**
 * LHA Sidebar – Isolated JavaScript Interop Module
 * Handles resize drag, responsive breakpoint detection, and cleanup.
 *
 * Design: All document-level listeners (mousemove, mouseup) are ONLY attached
 * while actively dragging and removed immediately on drag-end to prevent leaks.
 */

/** @type {DotNetObjectReference|null} */
let _dotNetRef = null;

/** @type {HTMLElement|null} */
let _sidebarEl = null;

/** @type {ResizeObserver|null} */
let _resizeObserver = null;

/** @type {MediaQueryList|null} */
let _mobileQuery = null;

/** @type {MediaQueryList|null} */
let _tabletQuery = null;

/** @type {Function|null} */
let _mobileHandler = null;

/** @type {Function|null} */
let _tabletHandler = null;

// ─── Resize drag state ───
let _isResizing = false;
let _resizeDotNetRef = null;
let _minWidth = 200;
let _maxWidth = 480;

/**
 * Initializes the sidebar JS module.
 * Sets up responsive breakpoint watchers using matchMedia.
 *
 * @param {HTMLElement} sidebarEl - The sidebar root element reference.
 * @param {DotNetObjectReference} dotNetRef - Reference to the Blazor Sidebar component.
 * @param {number} tabletBreakpoint - Pixel threshold for tablet mode.
 * @param {number} mobileBreakpoint - Pixel threshold for mobile mode.
 */
export function initialize(sidebarEl, dotNetRef, tabletBreakpoint, mobileBreakpoint) {
    _sidebarEl = sidebarEl;
    _dotNetRef = dotNetRef;

    // ── Setup responsive media query listeners ──
    _mobileQuery = window.matchMedia(`(max-width: ${mobileBreakpoint}px)`);
    _tabletQuery = window.matchMedia(`(min-width: ${mobileBreakpoint + 1}px) and (max-width: ${tabletBreakpoint}px)`);

    _mobileHandler = (e) => {
        if (e.matches && _dotNetRef) {
            _dotNetRef.invokeMethodAsync('OnBreakpointChanged', 'mobile');
        }
    };

    _tabletHandler = (e) => {
        if (e.matches && _dotNetRef) {
            _dotNetRef.invokeMethodAsync('OnBreakpointChanged', 'tablet');
        }
    };

    // Use addEventListener for modern browsers (addListener is deprecated)
    _mobileQuery.addEventListener('change', _mobileHandler);
    _tabletQuery.addEventListener('change', _tabletHandler);

    // Also listen for "desktop" breakpoint (above tablet)
    const desktopQuery = window.matchMedia(`(min-width: ${tabletBreakpoint + 1}px)`);
    desktopQuery.addEventListener('change', (e) => {
        if (e.matches && _dotNetRef) {
            _dotNetRef.invokeMethodAsync('OnBreakpointChanged', 'desktop');
        }
    });

    // ── Fire initial breakpoint check ──
    if (_mobileQuery.matches) {
        _dotNetRef.invokeMethodAsync('OnBreakpointChanged', 'mobile');
    } else if (_tabletQuery.matches) {
        _dotNetRef.invokeMethodAsync('OnBreakpointChanged', 'tablet');
    }
    // else: desktop is the default, no need to notify
}

/**
 * Starts a resize drag operation.
 * Attaches mousemove and mouseup listeners to the document for the duration of the drag.
 *
 * @param {HTMLElement} sidebarEl - The sidebar element being resized.
 * @param {DotNetObjectReference} dotNetRef - Blazor component reference for callbacks.
 * @param {number} minWidth - Minimum allowed width in pixels.
 * @param {number} maxWidth - Maximum allowed width in pixels.
 */
export function startResize(sidebarEl, dotNetRef, minWidth, maxWidth) {
    if (_isResizing) return;

    _isResizing = true;
    _resizeDotNetRef = dotNetRef;
    _minWidth = minWidth;
    _maxWidth = maxWidth;

    // Prevent text selection and set cursor globally during drag
    document.body.style.cursor = 'col-resize';
    document.body.style.userSelect = 'none';
    document.body.style.webkitUserSelect = 'none';

    // Attach document-level listeners (removed on drag-end)
    document.addEventListener('mousemove', onResizeMove, { passive: true });
    document.addEventListener('mouseup', onResizeEnd);

    // Also handle touch for hybrid devices
    document.addEventListener('touchmove', onTouchResizeMove, { passive: true });
    document.addEventListener('touchend', onResizeEnd);
}

/**
 * Handles mousemove during resize.
 * Uses requestAnimationFrame to batch DOM updates and avoid jank.
 * @param {MouseEvent} e
 */
function onResizeMove(e) {
    if (!_isResizing) return;

    requestAnimationFrame(() => {
        const newWidth = Math.round(e.clientX);
        const clamped = Math.max(_minWidth, Math.min(_maxWidth, newWidth));

        if (_sidebarEl) {
            // Update CSS variable directly for immediate visual feedback (no Blazor roundtrip)
            _sidebarEl.style.setProperty('--sidebar-width', `${clamped}px`);
        }

        // Notify Blazor (debounced by rAF naturally)
        if (_resizeDotNetRef) {
            _resizeDotNetRef.invokeMethodAsync('OnResizeUpdate', clamped);
        }
    });
}

/**
 * Handles touchmove during resize for hybrid/touch devices.
 * @param {TouchEvent} e
 */
function onTouchResizeMove(e) {
    if (!_isResizing || !e.touches.length) return;

    const touch = e.touches[0];
    requestAnimationFrame(() => {
        const newWidth = Math.round(touch.clientX);
        const clamped = Math.max(_minWidth, Math.min(_maxWidth, newWidth));

        if (_sidebarEl) {
            _sidebarEl.style.setProperty('--sidebar-width', `${clamped}px`);
        }

        if (_resizeDotNetRef) {
            _resizeDotNetRef.invokeMethodAsync('OnResizeUpdate', clamped);
        }
    });
}

/**
 * Handles mouseup / touchend – ends the resize operation and cleans up listeners.
 */
function onResizeEnd() {
    if (!_isResizing) return;

    _isResizing = false;

    // Restore cursor and selection
    document.body.style.cursor = '';
    document.body.style.userSelect = '';
    document.body.style.webkitUserSelect = '';

    // Remove document-level listeners immediately to prevent memory leaks
    document.removeEventListener('mousemove', onResizeMove);
    document.removeEventListener('mouseup', onResizeEnd);
    document.removeEventListener('touchmove', onTouchResizeMove);
    document.removeEventListener('touchend', onResizeEnd);

    // Notify Blazor of the final width
    if (_resizeDotNetRef && _sidebarEl) {
        const finalWidth = parseInt(
            getComputedStyle(_sidebarEl).getPropertyValue('--sidebar-width'), 10
        ) || 280;
        _resizeDotNetRef.invokeMethodAsync('OnResizeEnd', finalWidth);
    }

    _resizeDotNetRef = null;
}

/**
 * Cleans up all listeners and references.
 * Called from Blazor's DisposeAsync.
 */
export function dispose() {
    // Clean up resize if still in progress
    if (_isResizing) {
        onResizeEnd();
    }

    // Remove media query listeners
    if (_mobileQuery && _mobileHandler) {
        _mobileQuery.removeEventListener('change', _mobileHandler);
    }
    if (_tabletQuery && _tabletHandler) {
        _tabletQuery.removeEventListener('change', _tabletHandler);
    }

    _dotNetRef = null;
    _sidebarEl = null;
    _mobileQuery = null;
    _tabletQuery = null;
    _mobileHandler = null;
    _tabletHandler = null;
}
