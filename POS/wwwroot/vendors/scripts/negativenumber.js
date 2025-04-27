// Hàm định dạng số thành VNĐ
function formatVNĐ(number) {
    return number.toLocaleString('vi-VN') + ' VNĐ';
}



// Event listener cho sự kiện input
document.addEventListener('input', function (e) {
    const target = e.target;
    if (target.classList.contains('vndInputN')) {
        let sanitized = target.value.replace(/[^0-9-]/g, '');
        target.value = sanitized;
    }
});

// Event listener cho sự kiện focusout (thay cho blur)
document.addEventListener('focusout', function (e) {
    const target = e.target;
    if (target.classList.contains('vndInputN')) {
        if (target.value === '') return;
        let number = parseInt(target.value, 10);
        if (isNaN(number)) return;
        target.value = formatVNĐ(number);
    }
});

// Event listener cho sự kiện focusin (thay cho focus)
document.addEventListener('focusin', function (e) {
    const target = e.target;
    if (target.classList.contains('vndInputN')) {
        if (target.value === '') return;
        let number = target.value.replace(/[^0-9-]/g, '');
        target.value = number;
    }
});
