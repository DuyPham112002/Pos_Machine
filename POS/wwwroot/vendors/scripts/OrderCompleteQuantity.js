document.addEventListener("DOMContentLoaded", function () {
    fetchOrderInCompleteQuantity();
    fetchOrderCompleteQuantity();
    fetchOrderCancledQuantity();
});

async function fetchOrderInCompleteQuantity() {
    try {
        const response = await fetch('/Order/InCompleteOrderQuantity', {
            method: "GET"
        });
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status} - ${response.statusText}`);
        }
        const data = await response.json();
        const orderHtml = `
           ${data}
        `;
        document.getElementById('orderInCompleteQuantity').innerHTML = orderHtml;
    } catch (error) {
        console.error('Error fetching quantity:', error);
    }
}

async function fetchOrderCompleteQuantity() {
    try {
        const response = await fetch('/Order/CompleteOrderQuantity', {
            method: "GET"
        });
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status} - ${response.statusText}`);
        }
        const data = await response.json();
        const orderHtml = `
           ${data}
        `;
        document.getElementById('orderCompleteQuantity').innerHTML = orderHtml;
    } catch (error) {
        console.error('Error fetching quantity:', error);
    }
}

async function fetchOrderCancledQuantity() {
    try {
        const response = await fetch('/Order/CanceledOrderQuantity', {
            method: "GET"
        });
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status} - ${response.statusText}`);
        }
        const data = await response.json();
        const orderHtml = `
           ${data}
        `;
        document.getElementById('orderCanceledQuantity').innerHTML = orderHtml;
    } catch (error) {
        console.error('Error fetching quantity:', error);
    }
}
