import { formatDate, isValidAmount } from '../../utils/FormatUtils';
import type { OrderDto } from '../../models/OrderDto';

interface Props {
    order: OrderDto;
    editedAmount: string;
    onAmountChange: (value: string) => void;
    onUpdate: () => void;
    onDelete: () => void;
    changed: boolean;
    valid: boolean;
}

export function OrderRow({ order, editedAmount, onAmountChange, onUpdate, onDelete, changed, valid }: Props) {
    return (
        <tr key={order.id}>
            <td>{order.id}</td>
            <td>{order.customerName}</td>
            <td>
                <input
                    type="number"
                    value={editedAmount}
                    onChange={e => onAmountChange(e.target.value)}
                    style={{ width: 100, marginRight: 8 }}
                />
            </td>
            <td>{formatDate(order.createdAt)}</td>
            <td>
                <button
                    onClick={onUpdate}
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
                <button onClick={onDelete} style={{ color: 'Red' }}>Изтрий</button>
            </td>
        </tr>
    );
}
