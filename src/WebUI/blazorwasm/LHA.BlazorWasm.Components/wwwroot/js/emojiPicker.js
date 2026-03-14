window.LHA = window.LHA || {};
window.LHA.EmojiPicker = {
    scrollTo: function (elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
    }
};
