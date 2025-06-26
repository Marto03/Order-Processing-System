const API_URL = 'https://localhost:7067/api/Order';

export const getAllOrders = async () => {
    const res = await fetch(API_URL);
    if (!res.ok) throw new Error('Неуспешно извличане на поръчки');
    return res.json();
};

export const getOrderById = async (id) => {
    const res = await fetch(`${API_URL}/${id}`);
    if (!res.ok) throw new Error('Поръчката не е намерена');
    return res.json();
};

export const updateOrderAmount = async (id, amount) => {
    const res = await fetch(`${API_URL}/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ totalAmount: amount }),
    });
    if (!res.ok) throw new Error('Грешка при обновяване на поръчката');
};

export const deleteOrder = async (id) => {
    const res = await fetch(`${API_URL}/${id}`, {
        method: 'DELETE',
    });
    if (!res.ok) throw new Error('Грешка при изтриване на поръчка');
};

export const createOrder = async (order) => {
    const res = await fetch(API_URL, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(order),
    });
    if (!res.ok) throw new Error('Грешка при създаване на поръчка');
};
