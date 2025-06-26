import { useEffect, useState } from 'react';

interface OrderDto {
    id: number;
    customerName: string;
    totalAmount: number;
    createdAt: string;
    userId: number;
}

const API_URL = 'https://localhost:7067/api/Order';

const USERS_API = 'https://localhost:7067/api/User';

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
        fetch(API_URL)
            .then(res => res.json())
            .then(data => {
                setOrders(data);
                setLoading(false);
                setError(null);
                setEditedAmounts({});
            })
            .catch(() => setLoading(false));
    };

    useEffect(() => {
        fetchOrders();
    }, []);

    const fetchUserNameById = async (userId: number): Promise<string> => {
        try {
            const res = await fetch(`${USERS_API}/${userId}`);
            if (!res.ok) return "Неизвестен потребител";
            const data = await res.json();
            // 🔧 Използваме правилното поле тук — промени, ако е различно
            return data.name || data.fullName || data.username || "Неизвестен потребител";
        } catch {
            return "Грешка при потребителското име";
        }
    };

    const fetchOrderById = async (e: React.FormEvent) => {
        e.preventDefault();
        setSingleOrder(null);
        setError(null);
        if (!orderId) return;
        try {
            const res = await fetch(`${API_URL}/${orderId}`);
            if (!res.ok) throw new Error();
            const data = await res.json();
            const name = await fetchUserNameById(data.userId);
            setSingleOrder({ ...data, customerName: name });
        } catch {
            setError('Поръчката не е намерена');
        }

    };

    const handleAmountChange = (id: number, value: string) => {
        setEditedAmounts(prev => ({ ...prev, [id]: value }));
    };

    const filteredOrders = orders.filter(order =>
        order.customerName.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const handleUpdateOrderAmount = (id: number) => {
        const amountStr = editedAmounts[id];
        const amount = parseFloat(amountStr.replace(',', '.'));
        if (isNaN(amount)) {
            alert("Моля въведи валидна сума.");
            return;
        }
        fetch(`${API_URL}/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ totalAmount: amount })
        })
            .then(res => {
                if (!res.ok) throw new Error("Грешка при обновяване.");
                return res.json();
            })
            .then(() => {
                setEditedAmounts(prev => ({ ...prev, [id]: '' }));
                fetchOrders();
            })
            .catch(() => alert("Грешка при обновяване."));
    };

    const handleDeleteOrder = (id: number) => {
        fetch(`${API_URL}/${id}`, { method: 'DELETE' })
            .then(() => fetchOrders())
            .catch(() => alert("Грешка при изтриване"));
    };



    const handleCreateOrder = (e: React.FormEvent) => {
        e.preventDefault();
        const newOrderDto = {
            userId: parseInt(newOrder.userId),
            totalAmount: parseFloat(newOrder.totalAmount)
        };
        fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(newOrderDto)
        })
            .then(() => {
                setNewOrder({ userId: '', totalAmount: '' });
                fetchOrders();
            })
            .catch(() => alert("Грешка при създаване"));
    };

    const handleSort = (column: keyof OrderDto) => {
        if (sortColumn === column) {
            setSortDirection(prev => (prev === 'asc' ? 'desc' : 'asc'));
        } else {
            setSortColumn(column);
            setSortDirection('asc');
        }
    };
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
        <div>
            <h2>Поръчки</h2>
            <button onClick={fetchOrders} style={{ marginBottom: 10 }}>
                Обнови всички поръчки
            </button>
            <form onSubmit={fetchOrderById} style={{ marginBottom: 20 }}>
                <label>
                    Въведи ID на поръчка:&nbsp;
                    <input
                        type="number"
                        value={orderId}
                        onChange={e => setOrderId(e.target.value)}
                        style={{ width: 80 }}
                    />
                </label>
                <button type="submit" style={{ marginLeft: 8 }}>Вземи по ID</button>
            </form>
            {error && <div style={{ color: 'red' }}>{error}</div>}
            {singleOrder && (
                <div style={{ border: '1px solid #aaa', padding: 10, marginBottom: 20 }}>
                    <strong>Поръчка #{singleOrder.id}</strong><br />
                    Клиент: {singleOrder.customerName}<br />
                    Сума: {singleOrder.totalAmount.toFixed(2)} лв.<br />
                    Дата: {new Date(singleOrder.createdAt).toLocaleString('bg-BG')}
                </div>
            )}
            <h3>Създай нова поръчка</h3>
            <form onSubmit={handleCreateOrder} style={{ marginBottom: 20 }}>
                <input
                    type="number"
                    placeholder="User ID"
                    value={newOrder.userId}
                    onChange={e => setNewOrder({ ...newOrder, userId: e.target.value })}
                    required
                    style={{ marginRight: 10 }}
                />
                <input
                    type="number"
                    placeholder="Сума"
                    value={newOrder.totalAmount}
                    onChange={e => setNewOrder({ ...newOrder, totalAmount: e.target.value })}
                    required
                    step="0.01"
                    style={{ marginRight: 10 }}
                />
                <button type="submit">Създай</button>
            </form>
            <div style={{ marginBottom: 20 }}>
                <label>
                    Търси по име на клиент:&nbsp;
                    <input
                        type="text"
                        placeholder="Име на клиент"
                        value={searchTerm}
                        onChange={e => setSearchTerm(e.target.value)}
                        style={{ width: 200 }}
                    />
                </label>
            </div>
            {loading ? (
                <p>Зареждане...</p>
            ) : (
                <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                    <thead>
                        <tr>
                                <th onClick={() => handleSort('id')}>ID</th>
                                <th onClick={() => handleSort('customerName')}>Клиент</th>
                                <th onClick={() => handleSort('totalAmount')}>Сума</th>
                                <th onClick={() => handleSort('createdAt')}>Дата</th>
                            <th>Действия</th>
                        </tr>
                    </thead>
                    <tbody>
                        {sortedOrders.map(order => {
                            const edited = editedAmounts[order.id] ?? order.totalAmount.toString();
                            const changed = edited !== '' && parseFloat(edited.replace(',', '.')) !== order.totalAmount;
                            const valid = !isNaN(parseFloat(edited.replace(',', '.')));
                            return (
                                <tr key={order.id}>
                                    <td>{order.id}</td>
                                    <td>{order.customerName}</td>
                                    <td>
                                        <input
                                            type="number"
                                            value={edited}
                                            onChange={e => handleAmountChange(order.id, e.target.value)}
                                            style={{ width: 100, marginRight: 8 }}
                                        />
                                    </td>
                                    <td>{new Date(order.createdAt).toLocaleString('bg-BG')}</td>
                                    <td>
                                        <button
                                            onClick={() => handleUpdateOrderAmount(order.id)}
                                            disabled={!changed || !valid}
                                            style={{
                                                background: changed && valid ? '#1976d2' : '#ccc',
                                                color: 'white',
                                                border: 'none',
                                                padding: '6px 12px',
                                                borderRadius: 4,
                                                cursor: changed && valid ? 'pointer' : 'not-allowed',
                                                marginRight: 8
                                            }}
                                        >
                                            Обнови
                                        </button>
                                        <button onClick={() => handleDeleteOrder(order.id)}
                                            style={{
                                                color: 'Red'
                                            }}
                                        >
                                            Изтрий</button>
                                    </td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
            )}
        </div>
    );
}
