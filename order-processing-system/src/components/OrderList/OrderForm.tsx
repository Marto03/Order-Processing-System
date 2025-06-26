interface Props {
    userId: string;
    totalAmount: string;
    onUserIdChange: (val: string) => void;
    onAmountChange: (val: string) => void;
    onSubmit: (e: React.FormEvent) => void;
}

export function OrderForm({ userId, totalAmount, onUserIdChange, onAmountChange, onSubmit }: Props) {
    return (
        <form onSubmit={onSubmit} style={{ marginBottom: 20 }}>
            <input
                type="number"
                placeholder="User ID"
                value={userId}
                onChange={e => onUserIdChange(e.target.value)}
                required
                style={{ marginRight: 10 }}
            />
            <input
                type="number"
                placeholder="Сума"
                value={totalAmount}
                onChange={e => onAmountChange(e.target.value)}
                required
                step="0.01"
                style={{ marginRight: 10 }}
            />
            <button type="submit">Създай</button>
        </form>
    );
}
