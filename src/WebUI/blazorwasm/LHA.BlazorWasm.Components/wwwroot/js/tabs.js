// lhaTabs.js — Ink-bar measurement helper for the Tabs component.

window.lhaTabs = {
    /**
     * Measures the offset and size (width or height) of the active tab button
     * relative to its scroll-container (tabs-header-inner).
     *
     * @param {string} tabButtonId   - The id of the active <button> element.
     * @param {boolean} isVertical   - true = measure height/top, false = measure width/left.
     * @returns {{ offset: number, size: number }}
     */
    measureActiveTab: function (tabButtonId, isVertical) {
        const btn = document.getElementById(tabButtonId);
        if (!btn) return { offset: 0, size: 0 };

        // Walk up to find the scroll container
        const container = btn.closest('.tabs-header-inner');
        if (!container) return { offset: 0, size: 0 };

        const btnRect       = btn.getBoundingClientRect();
        const containerRect = container.getBoundingClientRect();

        if (isVertical) {
            return {
                offset: btnRect.top - containerRect.top + container.scrollTop,
                size:   btnRect.height
            };
        } else {
            return {
                offset: btnRect.left - containerRect.left + container.scrollLeft,
                size:   btnRect.width
            };
        }
    }
};
