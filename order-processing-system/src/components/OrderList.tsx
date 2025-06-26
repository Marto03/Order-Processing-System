import React, { useEffect, useState } from 'react';
import {
    getAllOrders,
    getOrderById,
    updateOrderAmount,
    deleteOrder,
    createOrder
} from '../api/OrderApi'; // API
import { getUserNameById } from '../api/UserApi';
import { getUserNameById1 } from '../api/UserApi';
import type { OrderDto } from '../models/OrderDto';

export function OrderList() {
    const [orders, setOrders] = useState<OrderDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [orderId, setOrderId] = useState('');
    const [singleOrder, setSingleOrder] = useState<OrderDto | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [editedAmounts, setEditedAmounts] = useState<{ [id: number]: string }>({});
    const [newOrder, setNewOrder] = useState({ userId: '', totalAmount: '' });
    const [searchTerm, setSearchTerm] = useState('');
    const [sortColumn, setSortColumn] = useState<keyof OrderDto | null>(null);
    const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('asc');

    const fetchOrders = () => {
        setLoading(true);
        getAllOrders()
            .then(async data => {
                const enrichedOrders = await Promise.all(
                    data.map(async (order: { userId: number; }) => {
                        const name = await getUserNameById(order.userId);
                        return { ...order, customerName: name };
                    })
                );
                setOrders(data);
                setLoading(false);
                setError(null);
                setEditedAmounts({});
            })
            .catch(() => {
                setLoading(false);
                setError('Грешка при зареждане');
            });
    };

    useEffect(() => {
        fetchOrders();
    }, []);

    const handleAmountChange = (id: number, value: string) => {
        setEditedAmounts(prev => ({ ...prev, [id]: value }));
    };

    const handleUpdateOrderAmount = (id: number) => {
        const amountStr = editedAmounts[id];
        const amount = parseFloat(amountStr.replace(',', '.'));
        if (isNaN(amount)) {
            alert("Моля въведи валидна сума.");
            return;
        }
        updateOrderAmount(id, amount)
            .then(() => {
                setEditedAmounts(prev => ({ ...prev, [id]: '' }));
                fetchOrders();
            })
            .catch(() => alert("Грешка при обновяване."));
    };

    const handleDeleteOrder = (id: number) => {
        if (!window.confirm("Сигурен ли си, че искаш да изтриеш поръчката?")) return;
        deleteOrder(id)
            .then(() => fetchOrders())
            .catch(() => alert("Грешка при изтриване"));
    };

    const handleCreateOrder = (e: React.FormEvent) => {
        e.preventDefault();
        const userId = parseInt(newOrder.userId);
        const totalAmount = parseFloat(newOrder.totalAmount.replace(',', '.'));
        if (isNaN(userId) || isNaN(totalAmount)) {
            alert("Моля въведи валидни стойности.");
            return;
        }
        createOrder({ userId, totalAmount })
            .then(() => {
                setNewOrder({ userId: '', totalAmount: '' });
                fetchOrders();
            })
            .catch(() => alert("Грешка при създаване"));
    };

    const handleFetchSingleOrder = (e: React.FormEvent) => {
        e.preventDefault();
        setSingleOrder(null);
        setError(null);
        if (!orderId) return;

        getOrderById(orderId)
            .then(async data => {
                const name = await getUserNameById1(data.userId);
                setSingleOrder({ ...data, customerName: data.customerName, totalAmount: data.totalAmount });
            })
            .catch(() => setError('Поръчката не е намерена'));
    };

    const handleSort = (column: keyof OrderDto) => {
        if (sortColumn === column) {
            setSortDirection(prev => (prev === 'asc' ? 'desc' : 'asc'));
        } else {
            setSortColumn(column);
            setSortDirection('asc');
        }
    };

    const filteredOrders = orders.filter(order =>
        order.customerName?.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const sortedOrders = [...filteredOrders].sort((a, b) => {
        if (!sortColumn) return 0;
        const aVal = a[sortColumn];
        const bVal = b[sortColumn];

        if (typeof aVal === 'number' && typeof bVal === 'number') {
            return sortDirection === 'asc' ? aVal - bVal : bVal - aVal;
        }

        if (typeof aVal === 'string' && typeof bVal === 'string') {
            return sortDirection === 'asc'
                ? aVal.localeCompare(bVal)
                : bVal.localeCompare(aVal);
        }

        return 0;
    });

    return (
        <div style={{ padding: 20 }}>
            <h2>Поръчки</h2>
            <button onClick={fetchOrders}>🔄 Обнови поръчките</button>

            <form onSubmit={handleFetchSingleOrder} style={{ margin: '10px 0' }}>
                <label>ID на поръчка: </label>
                <input
                    type="text"
                    value={orderId}
                    onChange={e => setOrderId(e.target.value)}
                    style={{ width: 80 }}
                />
                <button type="submit">🔍 Вземи</button>
            </form>

            {error && <p style={{ color: 'red' }}>{error}</p>}

            {singleOrder && (
                <div style={{ background: '#000', padding: 10, marginBottom: 10 }}>
                    <strong>Поръчка #{singleOrder.id}</strong><br />
                    Клиент: {singleOrder.customerName}<br />
                    Сума: {singleOrder.totalAmount.toFixed(2)} лв<br />
                    Дата: {new Date(singleOrder.createdAt).toLocaleString()}
                </div>
            )}

            <input
                type="text"
                placeholder="Търси по клиент"
                value={searchTerm}
                onChange={e => setSearchTerm(e.target.value)}
                style={{ marginBottom: 10 }}
            />

            <form onSubmit={handleCreateOrder}>
                <h4>➕ Създай нова поръчка</h4>
                <input
                    type="text"
                    placeholder="User ID"
                    value={newOrder.userId}
                    onChange={e => setNewOrder(prev => ({ ...prev, userId: e.target.value }))}
                />
                <input
                    type="text"
                    placeholder="Сума"
                    value={newOrder.totalAmount}
                    onChange={e => setNewOrder(prev => ({ ...prev, totalAmount: e.target.value }))}
                />
                <button type="submit">Създай</button>
            </form>

            {loading ? (
                <p>Зареждане...</p>
            ) : (
                <table border={1} cellPadding={5} style={{ width: '100%', marginTop: 20 }}>
                    <thead>
                        <tr>
                            <th onClick={() => handleSort('id')}>ID</th>
                            <th onClick={() => handleSort('customerName')}>Клиент</th>
                            <th onClick={() => handleSort('totalAmount')}>Сума</th>
                            <th onClick={() => handleSort('createdAt')}>Дата</th>
                            <th>Изтриване</th>
                        </tr>
                    </thead>
                    <tbody>
                        {sortedOrders.map(order => (
                            <tr key={order.id}>
                                <td>{order.id}</td>
                                <td>{order.customerName}</td>
                                <td>
                                    {editedAmounts[order.id] !== undefined ? (
                                        <>
                                            <input
                                                type="text"
                                                value={editedAmounts[order.id]}
                                                onChange={e => handleAmountChange(order.id, e.target.value)}
                                                style={{ width: 60 }}
                                            />
                                            <button onClick={() => handleUpdateOrderAmount(order.id)}>💾</button>
                                        </>
                                    ) : (
                                        <>
                                                <div style={{ display: 'flex', justifyContent: 'flex-end', alignItems: 'center', width: '100%' }}>
                                                    <span style={{ marginRight: '8px' }}>{order.totalAmount.toFixed(2)} лв.</span>
                                                    <button
                                                        onClick={() =>
                                                            setEditedAmounts(prev => ({ ...prev, [order.id]: order.totalAmount.toString() }))
                                                        }
                                                    >
                                                        ✏️
                                                    </button>
                                                </div>
                                        </>
                                    )}
                                            {/*{order.totalAmount.toFixed(2)} лв.&nbsp;*/}
                                            {/*<button onClick={() =>*/}
                                            {/*    setEditedAmounts(prev => ({ ...prev, [order.id]: order.totalAmount.toString() }))*/}
                                            {/*}>✏️</button>*/}
                                </td>
                                <td>{new Date(order.createdAt).toLocaleString()}</td>
                                <td>
                                    <button onClick={() => handleDeleteOrder(order.id)}>🗑️</button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}
