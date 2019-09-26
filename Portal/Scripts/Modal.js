function showModal(url) {
    document.getElementById('modal-loading-image').style.display = 'flex';
    var iframe = window.frames['iframe-content'];
    iframe.style.display = 'none';
    var modal = document.getElementById('iframe-modal');
    
    if (url != null) {
        iframe.src = url;
    }
    document.getElementById('iframe-title').innerHTML = iframe.src;
    modal.classList.remove('hiddenModal');
}

function hideModal() {
    var iframe = window.frames['iframe-content'];
    var modal = document.getElementById('iframe-modal');

    iframe.src = "";
    document.getElementById('iframe-title').innerHTML = iframe.src;
    modal.classList.add('hiddenModal');
}

window.onload = function() {
    window.frames['iframe-content'].onload = function() {
        document.getElementById('modal-loading-image').style.display = 'none';
        this.style.display = 'flex';
    }
}