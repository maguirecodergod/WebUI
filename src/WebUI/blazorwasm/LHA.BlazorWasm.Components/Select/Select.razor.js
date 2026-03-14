/**
 * Helper to determine if a dropdown should open upwards based on available viewport space.
 * @param {HTMLElement} element The select wrapper element
 * @param {number} dropdownHeight The expected height of the dropdown
 * @returns {boolean} True if should open upwards
 */
export function shouldOpenUpwards(element, dropdownHeight = 250) {
    if (!element) return false;
    
    const rect = element.getBoundingClientRect();
    const spaceBelow = window.innerHeight - rect.bottom;
    const spaceAbove = rect.top;
    
    // If space below is less than dropdown height AND there's more space above
    return spaceBelow < dropdownHeight && spaceAbove > spaceBelow;
}
