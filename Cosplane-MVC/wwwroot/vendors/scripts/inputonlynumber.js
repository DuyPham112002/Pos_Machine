
// Event listener cho sự kiện input
document.addEventListener('input', function (e) {
    const target = e.target;
    if (target.classList.contains('OnlyNumber')) {
        let sanitized = target.value.replace(/[^0-9]/g, '');
        target.value = sanitized;
    }
});


