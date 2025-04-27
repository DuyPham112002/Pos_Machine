function toastError(message) {
    var notifications = document.querySelector(".notifications");
    var toast = document.createElement("li");
    toast.style.display = "flex";
    toast.className = `toast error`;
    toast.innerHTML = `<div class="column">
        <i class="fa-solid fa-circle-xmark"></i>
        <span>${message}</span>
    </div>
    <i class="fa-solid fa-xmark" onclick="removeToast(this.parentElement)"></i>`;
    notifications.appendChild(toast);
    toast.timeoutId = setTimeout(() => removeToast(toast), 5000);
}

function toastSuccess(message) {
    var notifications = document.querySelector(".notifications");
    var toast = document.createElement("li");
    toast.style.display = "flex";
    toast.className = `toast success`;
    toast.innerHTML = `<div class="column">
        <i class="fa-solid fa-circle-check"></i>
        <span>${message}</span>
    </div>
    <i class="fa-solid fa-check" onclick="removeToast(this.parentElement)"></i>`;
    notifications.appendChild(toast);
    toast.timeoutId = setTimeout(() => removeToast(toast), 5000);
}

function toastWarning(message) {
    var notifications = document.querySelector(".notifications");
    var toast = document.createElement("li");
    toast.style.display = "flex";
    toast.className = `toast warning`;
    toast.innerHTML = `<div class="column">
        <i class="fa-solid fa-circle-exclamation"></i>
        <span>${message}</span>
    </div>
    <i class="fa-solid fa-circle-exclamation" onclick="removeToast(this.parentElement)"></i>`;
    notifications.appendChild(toast);
    toast.timeoutId = setTimeout(() => removeToast(toast), 5000);
}

function removeToast(toast) {
    toast.classList.add("hide");
    if (toast.timeoutId) clearTimeout(toast.timeoutId); // Clearing the timeout for the toast
    setTimeout(() => toast.remove(), 500); // Removing the toast after 500ms
}
