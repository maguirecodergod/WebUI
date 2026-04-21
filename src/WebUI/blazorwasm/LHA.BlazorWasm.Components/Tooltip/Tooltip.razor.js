let floatingCleanupMap = new Map();

export function showTooltip(triggerEl, tooltipEl, options) {
    if (!triggerEl || !tooltipEl) return;

    // Check if Floating UIDOM is available
    if (!window.FloatingUIDOM) {
        console.warn("FloatingUIDOM is missing. Cannot position tooltip smartly.");
        return;
    }

    const { computePosition, flip, shift, offset, arrow, autoUpdate } = window.FloatingUIDOM;
    const arrowEl = tooltipEl.querySelector('.lha-tooltip-arrow');
    const placement = options.placement || 'top';

    // Cleanup previous positioning loop if tooltip is re-shown quickly
    if (floatingCleanupMap.has(tooltipEl)) {
        floatingCleanupMap.get(tooltipEl)();
        floatingCleanupMap.delete(tooltipEl);
    }

    function updatePosition() {
        computePosition(triggerEl, tooltipEl, {
            placement: placement,
            middleware: [
                offset(8),
                flip({ fallbackAxisSideDirection: 'end', padding: 8 }),
                shift({ padding: 8 }),
                arrowEl ? arrow({ element: arrowEl }) : null
            ].filter(Boolean)
        }).then(({ x, y, placement: resultingPlacement, middlewareData }) => {
            // Apply coordinates
            Object.assign(tooltipEl.style, {
                left: `${x}px`,
                top: `${y}px`,
                bottom: 'auto',
                right: 'auto',
                transform: 'translate(0, 0)', // override CSS transform
                margin: '0',
            });

            // Set data attribute for arrow CSS rules
            tooltipEl.setAttribute('data-placement', resultingPlacement);

            // Position the arrow
            if (middlewareData.arrow && arrowEl) {
                const { x: arrowX, y: arrowY } = middlewareData.arrow;
                const staticSide = {
                    top: 'bottom',
                    right: 'left',
                    bottom: 'top',
                    left: 'right'
                }[resultingPlacement.split('-')[0]];

                Object.assign(arrowEl.style, {
                    left: arrowX != null ? `${arrowX}px` : '',
                    top: arrowY != null ? `${arrowY}px` : '',
                    right: '',
                    bottom: '',
                    [staticSide]: '-6px',
                });
            }
        });
    }

    // Attach autoUpdate to recalculate position on scroll, resize, or DOM changes
    const cleanup = autoUpdate(triggerEl, tooltipEl, updatePosition, {
        ancestorScroll: true,
        ancestorResize: true,
        elementResize: true,
        layoutShift: true
    });

    floatingCleanupMap.set(tooltipEl, cleanup);
}

export function hideTooltip(tooltipEl) {
    if (floatingCleanupMap.has(tooltipEl)) {
        floatingCleanupMap.get(tooltipEl)(); // Invoke autoUpdate cleanup
        floatingCleanupMap.delete(tooltipEl);
    }
}

export function dispose(tooltipEl) {
    hideTooltip(tooltipEl);
}
