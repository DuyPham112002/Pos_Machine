function validatePhoneLength(input) {
    if (input.value.length > 15) {
        input.value = input.value.slice(0, 15);
    }
}

// Hàm định dạng số thành VNĐ
function formatVNĐ(number) {
    return number.toLocaleString('vi-VN') + ' VNĐ';
}



// Event listener cho sự kiện input
document.addEventListener('input', function (e) {
    const target = e.target;
    if (target.classList.contains('vndInput')) {
        let sanitized = target.value.replace(/\D/g, '');
        target.value = sanitized;
    }
});

// Event listener cho sự kiện focusout (thay cho blur)
document.addEventListener('focusout', function (e) {
    const target = e.target;
    if (target.classList.contains('vndInput')) {
        if (target.value === '') return;
        let number = parseInt(target.value, 10);
        if (isNaN(number)) return;
        target.value = formatVNĐ(number);
    }
});

// Event listener cho sự kiện focusin (thay cho focus)
document.addEventListener('focusin', function (e) {
    const target = e.target;
    if (target.classList.contains('vndInput')) {
        if (target.value === '') return;
        let number = target.value.replace(/\D/g, '');
        target.value = number;
    }
});


