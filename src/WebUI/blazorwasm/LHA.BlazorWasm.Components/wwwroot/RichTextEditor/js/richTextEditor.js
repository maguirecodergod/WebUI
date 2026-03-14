// RichTextEditor JS Interop Engine
// Manages contentEditable areas, execCommand bridge, and state tracking

const editors = new Map();

/**
 * Initialize a rich text editor instance.
 * @param {string} editorId - Unique ID of the editor container
 * @param {object} dotNetRef - .NET object reference for callbacks
 * @param {object} options - Editor configuration
 */
export function initEditor(editorId, dotNetRef, options) {
    const container = document.getElementById(editorId);
    if (!container) return;

    const contentArea = container.querySelector('.rte-content-area');
    if (!contentArea) return;

    const editor = {
        id: editorId,
        container,
        contentArea,
        dotNetRef,
        options: options || {},
        isSourceView: false,
        debounceTimer: null,
        debounceMs: options?.debounceMs || 300
    };

    // Make content area editable
    contentArea.setAttribute('contenteditable', options?.readOnly ? 'false' : 'true');
    contentArea.setAttribute('spellcheck', 'true');

    // Set placeholder
    if (options?.placeholder) {
        contentArea.dataset.placeholder = options.placeholder;
    }

    // Attach event listeners
    contentArea.addEventListener('input', () => onContentChanged(editor));
    contentArea.addEventListener('keyup', () => onSelectionChanged(editor));
    contentArea.addEventListener('mouseup', () => onSelectionChanged(editor));
    contentArea.addEventListener('focus', () => onSelectionChanged(editor));
    contentArea.addEventListener('paste', (e) => onPaste(e, editor));

    // Track click outside to close popups
    document.addEventListener('click', (e) => onDocumentClick(e, editor));

    editors.set(editorId, editor);

    // Initialize drag resizer for images and iframes
    initResizer(editor);

    // Initial state sync
    setTimeout(() => onSelectionChanged(editor), 100);
}

/**
 * Execute a formatting command.
 * @param {string} editorId
 * @param {string} command - execCommand name
 * @param {string|null} value - Optional value for the command
 */
export function execCommand(editorId, command, value) {
    const editor = editors.get(editorId);
    if (!editor) return;

    editor.contentArea.focus();

    // Special handling for certain commands
    switch (command) {
        case 'formatBlock':
            document.execCommand('formatBlock', false, value || 'p');
            break;
        case 'createLink':
            if (value) {
                document.execCommand('createLink', false, value);
            }
            break;
        case 'insertImage':
            if (value) {
                document.execCommand('insertImage', false, value);
            }
            break;
        case 'foreColor':
            document.execCommand('foreColor', false, value || '#000000');
            break;
        case 'backColor':
            document.execCommand('backColor', false, value || 'transparent');
            break;
        default:
            document.execCommand(command, false, value);
            break;
    }

    // Sync state after command
    onContentChanged(editor);
    onSelectionChanged(editor);
}

/**
 * Get the HTML content of the editor.
 */
export function getHtml(editorId) {
    const editor = editors.get(editorId);
    if (!editor) return '';
    return editor.contentArea.innerHTML;
}

/**
 * Set the HTML content of the editor.
 */
export function setHtml(editorId, html) {
    const editor = editors.get(editorId);
    if (!editor) return;
    editor.contentArea.innerHTML = html || '';
}

/**
 * Get the plain text content.
 */
export function getText(editorId) {
    const editor = editors.get(editorId);
    if (!editor) return '';
    return editor.contentArea.innerText || '';
}

/**
 * Get the currently selected text.
 */
export function getSelectedText(editorId) {
    const sel = window.getSelection();
    return sel ? sel.toString() : '';
}

/**
 * Insert HTML at the current cursor position.
 */
export function insertHtml(editorId, html) {
    const editor = editors.get(editorId);
    if (!editor) return;

    editor.contentArea.focus();

    const sel = window.getSelection();
    if (sel && sel.rangeCount > 0) {
        const range = sel.getRangeAt(0);
        range.deleteContents();

        const temp = document.createElement('div');
        temp.innerHTML = html;
        const frag = document.createDocumentFragment();
        let lastNode;
        while (temp.firstChild) {
            lastNode = frag.appendChild(temp.firstChild);
        }
        range.insertNode(frag);

        // Move cursor after inserted content
        if (lastNode) {
            const newRange = document.createRange();
            newRange.setStartAfter(lastNode);
            newRange.collapse(true);
            sel.removeAllRanges();
            sel.addRange(newRange);
        }
    }

    onContentChanged(editor);
}

/**
 * Insert a table with the given rows and columns.
 */
export function insertTable(editorId, rows, cols) {
    const editor = editors.get(editorId);
    if (!editor) return;

    let html = '<table class="rte-table" style="border-collapse:collapse;width:100%">';
    for (let r = 0; r < rows; r++) {
        html += '<tr>';
        for (let c = 0; c < cols; c++) {
            html += '<td style="border:1px solid #ccc;padding:8px;min-width:40px">&nbsp;</td>';
        }
        html += '</tr>';
    }
    html += '</table><p>&nbsp;</p>';

    insertHtml(editorId, html);
}

/**
 * Insert a code block with syntax highlighting.
 */
export function insertCodeBlock(editorId, code, language) {
    const editor = editors.get(editorId);
    if (!editor) return;

    // Create a temporary container to get the highlighted HTML
    const pre = document.createElement('pre');
    const codeEl = document.createElement('code');
    codeEl.className = `language-${language}`;
    codeEl.textContent = code;
    pre.appendChild(codeEl);

    if (window.hljs) {
        window.hljs.highlightElement(codeEl);
    }

    // Wrap in a div to ensure it's treated as a block and followed by a newline
    const wrapper = document.createElement('div');
    wrapper.appendChild(pre);
    
    const p = document.createElement('p');
    p.innerHTML = '<br>';
    wrapper.appendChild(p);

    insertHtml(editorId, wrapper.innerHTML);
}

/**
 * Get the current formatting state for toolbar sync.
 */
export function getFormatState(editorId) {
    const editor = editors.get(editorId);
    if (!editor) return null;

    const state = {
        isBold: document.queryCommandState('bold'),
        isItalic: document.queryCommandState('italic'),
        isUnderline: document.queryCommandState('underline'),
        isStrikeThrough: document.queryCommandState('strikeThrough'),
        isSubscript: document.queryCommandState('subscript'),
        isSuperscript: document.queryCommandState('superscript'),
        isOrderedList: document.queryCommandState('insertOrderedList'),
        isUnorderedList: document.queryCommandState('insertUnorderedList'),
        alignment: getAlignment(),
        fontColor: document.queryCommandValue('foreColor') || '#000000',
        backColor: document.queryCommandValue('backColor') || 'transparent',
        fontName: document.queryCommandValue('fontName') || '',
        fontSize: document.queryCommandValue('fontSize') || '',
        currentBlock: getBlockFormat(),
        wordCount: getWordCount(editor),
        charCount: getCharCount(editor),
        elementPath: getElementPath(editor),
        isSourceView: editor.isSourceView || false,
        isFullscreen: editor.container.classList.contains('rte-fullscreen') || false
    };

    return state;
}

/**
 * Toggle source code view.
 */
export function toggleSourceView(editorId) {
    const editor = editors.get(editorId);
    if (!editor) return false;

    const sourceArea = editor.container.querySelector('.rte-source-area');
    if (!sourceArea) return false;

    editor.isSourceView = !editor.isSourceView;

    if (editor.isSourceView) {
        // Switch to source view
        sourceArea.value = formatHtml(editor.contentArea.innerHTML);
        editor.contentArea.style.display = 'none';
        sourceArea.style.display = 'block';
    } else {
        // Switch back to WYSIWYG view
        editor.contentArea.innerHTML = sourceArea.value;
        sourceArea.style.display = 'none';
        editor.contentArea.style.display = 'block';
        onContentChanged(editor);
    }

    return editor.isSourceView;
}

/**
 * Toggle fullscreen mode.
 */
export function toggleFullscreen(editorId) {
    const editor = editors.get(editorId);
    if (!editor) return false;

    const isFullscreen = editor.container.classList.toggle('rte-fullscreen');
    document.body.classList.toggle('rte-fullscreen-active', isFullscreen);

    return isFullscreen;
}

/**
 * Set read-only state.
 */
export function setReadOnly(editorId, readOnly) {
    const editor = editors.get(editorId);
    if (!editor) return;
    editor.contentArea.setAttribute('contenteditable', readOnly ? 'false' : 'true');
}

/**
 * Focus the editor content area.
 */
export function focusEditor(editorId) {
    const editor = editors.get(editorId);
    if (!editor) return;
    editor.contentArea.focus();
}

/**
 * Dispose and cleanup an editor instance.
 */
export function dispose(editorId) {
    const editor = editors.get(editorId);
    if (!editor) return;

    if (editor.debounceTimer) {
        clearTimeout(editor.debounceTimer);
    }

    if (editor.onWindowBlur) {
        window.removeEventListener('blur', editor.onWindowBlur);
    }

    editors.delete(editorId);
}

// ========== Internal Helper Functions ==========

function onContentChanged(editor) {
    if (editor.debounceTimer) {
        clearTimeout(editor.debounceTimer);
    }

    editor.debounceTimer = setTimeout(async () => {
        try {
            const html = editor.contentArea.innerHTML;
            await editor.dotNetRef.invokeMethodAsync('OnContentChangedFromJs', html);
        } catch (e) {
            // dotNetRef may have been disposed
        }
    }, editor.debounceMs);
}

async function onSelectionChanged(editor) {
    try {
        const state = getFormatState(editor.id);
        if (state) {
            await editor.dotNetRef.invokeMethodAsync('OnSelectionChangedFromJs', state);
        }
    } catch (e) {
        // dotNetRef may have been disposed
    }
}

function onPaste(e, editor) {
    // Allow default paste behavior; clean up can be added later
}

function onDocumentClick(e, editor) {
    // Close any open dropdowns if clicking outside
    if (!editor.container.contains(e.target)) {
        const dropdowns = editor.container.querySelectorAll('.rte-dropdown-popup.open');
        dropdowns.forEach(d => d.classList.remove('open'));
    }
}

function getAlignment() {
    if (document.queryCommandState('justifyCenter')) return 'center';
    if (document.queryCommandState('justifyRight')) return 'right';
    if (document.queryCommandState('justifyFull')) return 'justify';
    return 'left';
}

function getBlockFormat() {
    try {
        const val = document.queryCommandValue('formatBlock');
        return val ? val.toLowerCase().replace(/[<>]/g, '') : 'p';
    } catch {
        return 'p';
    }
}

function getWordCount(editor) {
    const text = editor.contentArea.innerText || '';
    const trimmed = text.trim();
    if (!trimmed) return 0;
    return trimmed.split(/\s+/).length;
}

function getCharCount(editor) {
    const text = editor.contentArea.innerText || '';
    return text.length;
}

function getElementPath(editor) {
    const sel = window.getSelection();
    if (!sel || sel.rangeCount === 0) return '';

    let node = sel.anchorNode;
    if (!node) return '';

    if (node.nodeType === Node.TEXT_NODE) {
        node = node.parentNode;
    }

    const path = [];
    while (node && node !== editor.contentArea && node !== document.body) {
        const tag = node.nodeName.toLowerCase();
        if (tag !== '#document' && tag !== 'html') {
            path.unshift(tag);
        }
        node = node.parentNode;
    }

    return path.join(' › ');
}

function formatHtml(html) {
    // Simple HTML formatter for source view
    let formatted = '';
    let indent = 0;
    const tags = html.replace(/>\s*</g, '>\n<').split('\n');

    for (const tag of tags) {
        const trimmed = tag.trim();
        if (!trimmed) continue;

        if (trimmed.startsWith('</')) {
            indent = Math.max(0, indent - 1);
        }

        formatted += '  '.repeat(indent) + trimmed + '\n';

        if (trimmed.startsWith('<') && !trimmed.startsWith('</') &&
            !trimmed.startsWith('<!') && !trimmed.endsWith('/>') &&
            !/<\/(br|hr|img|input|meta|link)>/i.test(trimmed) &&
            !/^<(br|hr|img|input|meta|link)\b/i.test(trimmed)) {
            // Check if it's not a self-closing or void element
            if (!trimmed.includes('</')) {
                indent++;
            }
        }
    }

    return formatted.trim();
}

function initResizer(editor) {
    const resizer = document.createElement('div');
    resizer.className = 'rte-resizer';
    resizer.innerHTML = `
        <div class="rte-resizer-handle nw" data-dir="nw"></div>
        <div class="rte-resizer-handle ne" data-dir="ne"></div>
        <div class="rte-resizer-handle sw" data-dir="sw"></div>
        <div class="rte-resizer-handle se" data-dir="se"></div>
        <div class="rte-resizer-handle n" data-dir="n"></div>
        <div class="rte-resizer-handle s" data-dir="s"></div>
        <div class="rte-resizer-handle e" data-dir="e"></div>
        <div class="rte-resizer-handle w" data-dir="w"></div>
    `;
    resizer.style.display = 'none';
    editor.contentArea.parentNode.appendChild(resizer);

    editor.resizer = resizer;
    let currentElement = null;
    let isResizing = false;
    let startX, startY;
    let startWidth, startHeight;
    let currentDir = '';

    const updateResizerPos = () => {
        if (!currentElement) return;
        const rect = currentElement.getBoundingClientRect();
        const wrapperRect = editor.contentArea.parentNode.getBoundingClientRect();

        resizer.style.display = 'block';
        resizer.style.top = (rect.top - wrapperRect.top) + 'px';
        resizer.style.left = (rect.left - wrapperRect.left) + 'px';
        resizer.style.width = rect.width + 'px';
        resizer.style.height = rect.height + 'px';
    };

    const selectElement = (el) => {
        currentElement = el;
        updateResizerPos();
        try {
            const sel = window.getSelection();
            const range = document.createRange();
            range.selectNode(currentElement);
            if (sel) {
                sel.removeAllRanges();
                sel.addRange(range);
            }
        } catch(e) {}
    };

    editor.contentArea.addEventListener('click', (e) => {
        if (e.target.tagName === 'IMG' || e.target.tagName === 'IFRAME' || e.target.tagName === 'VIDEO') {
            selectElement(e.target);
        } else {
            currentElement = null;
            resizer.style.display = 'none';
        }
    });

    editor.onWindowBlur = () => {
        setTimeout(() => {
            if (document.activeElement && 
               (document.activeElement.tagName === 'IFRAME' || document.activeElement.tagName === 'VIDEO') && 
               editor.contentArea.contains(document.activeElement)) {
                selectElement(document.activeElement);
            }
        }, 100);
    };
    window.addEventListener('blur', editor.onWindowBlur);

    editor.contentArea.addEventListener('scroll', () => {
        if (currentElement) updateResizerPos();
    });

    resizer.addEventListener('mousedown', (e) => {
        if (e.target.classList.contains('rte-resizer-handle')) {
            e.preventDefault();
            e.stopPropagation();
            isResizing = true;
            currentDir = e.target.dataset.dir;
            startX = e.clientX;
            startY = e.clientY;
            const rect = currentElement.getBoundingClientRect();
            startWidth = rect.width;
            startHeight = rect.height;

            document.addEventListener('mousemove', onMouseMove);
            document.addEventListener('mouseup', onMouseUp);
        }
    });

    const onMouseMove = (e) => {
        if (!isResizing || !currentElement) return;
        e.preventDefault();

        const dx = e.clientX - startX;
        const dy = e.clientY - startY;

        let newWidth = startWidth;
        let newHeight = startHeight;

        if (currentDir.includes('e')) newWidth = startWidth + dx;
        if (currentDir.includes('w')) newWidth = startWidth - dx;
        if (currentDir.includes('s')) newHeight = startHeight + dy;
        if (currentDir.includes('n')) newHeight = startHeight - dy;

        if (newWidth > 20) {
            currentElement.style.width = newWidth + 'px';
            currentElement.width = newWidth;
        }
        if (newHeight > 20) {
            currentElement.style.height = newHeight + 'px';
            currentElement.height = newHeight;
        }
        updateResizerPos();
    };

    const onMouseUp = (e) => {
        isResizing = false;
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);
        onContentChanged(editor);
    };

    editor.contentArea.addEventListener('input', () => {
        if (currentElement && !editor.contentArea.contains(currentElement)) {
            currentElement = null;
            resizer.style.display = 'none';
        } else if (currentElement) {
            updateResizerPos();
        }
    });
}
