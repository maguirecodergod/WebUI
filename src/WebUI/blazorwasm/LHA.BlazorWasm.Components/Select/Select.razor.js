/**
 * Helper to determine smart placement for a dropdown based on available viewport space.
 * @param {HTMLElement} element The select wrapper element
 * @param {number} dropdownHeight The expected height of the dropdown
 * @param {number} dropdownWidth The expected width of the dropdown
 * @returns {object} { openUpwards: boolean, alignRight: boolean }
 */
export function getSmartPlacement(element, dropdownHeight = 300, dropdownWidth = 200) {
    if (!element) return { openUpwards: false, alignRight: false };
    
    const rect = element.getBoundingClientRect();
    const viewportWidth = window.innerWidth;
    const viewportHeight = window.innerHeight;

    const spaceBelow = viewportHeight - rect.bottom;
    const spaceAbove = rect.top;
    const spaceRight = viewportWidth - rect.left;
    const spaceLeft = rect.right;
    
    // Vertical placement
    const openUpwards = spaceBelow < dropdownHeight && spaceAbove > spaceBelow;
    
    // Horizontal alignment
    // If there is not enough space to the right for the dropdown width, align right
    const alignRight = spaceRight < dropdownWidth && spaceLeft > spaceRight;
    
    return { openUpwards, alignRight };
}

/**
 * Legacy support for the original method name
 */
export function shouldOpenUpwards(element, dropdownHeight = 250) {
    const result = getSmartPlacement(element, dropdownHeight);
    return result.openUpwards;
}
