// OrderApi.ts
import type {
    OrderDto
} from '../models/OrderDto';

const API_URL = 'https://localhost:7067/api/Order';
//const USERS_API = 'https://localhost:7067/api/User';

//export async function getAllOrders(): Promise<OrderDto[]> {
//    const res = await fetch(API_URL);
//    if (!res.ok) throw new Error('Неуспешно извличане на поръчки');
//    return res.json();
//}

//export async function getOrderById(orderId: string): Promise<OrderDto> {
//    const res = await fetch(`${API_URL}/${orderId}`);
//    if (!res.ok) throw new Error('Поръчката не е намерена');
//    return res.json();
//}

////export async function getUserNameById(userId: number): Promise<string> {
////    const res = await fetch(`${USERS_API}/${userId}`);
////    if (!res.ok) return 'Неизвестен потребител';
////    const data = await res.json();
////    return data.name || data.fullName || data.username || 'Неизвестен потребител';
////}

//export async function updateOrderAmount(id: number, amount: number): Promise<void> {
//    const res = await fetch(`${API_URL}/${id}`, {
//        method: 'PUT',
//        headers: { 'Content-Type': 'application/json' },
//        body: JSON.stringify({ totalAmount: amount }),
//    });
//    if (!res.ok) throw new Error('Грешка при обновяване на поръчката');
//}

//export async function deleteOrder(id: number): Promise<void> {
//    const res = await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
//    if (!res.ok) throw new Error('Грешка при изтриване на поръчка');
//}

//export async function createOrder(userId: number, totalAmount: number): Promise<void> {
//    const res = await fetch(API_URL, {
//        method: 'POST',
//        headers: { 'Content-Type': 'application/json' },
//        body: JSON.stringify({ userId, totalAmount }),
//    });
//    if (!res.ok) throw new Error('Грешка при създаване на поръчка');
//}


export const getAllOrders = () => fetch(API_URL).then(res => res.json());

export const getOrderById = (id: string) =>
    fetch(`${API_URL}/${id}`).then(res => {
        if (!res.ok) throw new Error('Order not found');
        return res.json();
    });

export const updateOrderAmount = (id: number, amount: number) =>
    fetch(`${API_URL}/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ totalAmount: amount })
    });

export const deleteOrder = (id: number) =>
    fetch(`${API_URL}/${id}`, { method: 'DELETE' });

export const createOrder = (order: { userId: number; totalAmount: number }) =>
    fetch(API_URL, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(order)
    });