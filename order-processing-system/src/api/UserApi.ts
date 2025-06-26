const USERS_API = 'https://localhost:7234/api/Users';

export const getUserNameById = async (userId: number): Promise<string> => {
    try {
        const res = await fetch(`${USERS_API}/${userId}`);
        if (!res.ok) return 'Неизвестен потребител4';
        const data = await res.json(); // това е масив
        const order = data.find((o: any) => o.id === userId); // или o.userId === userId
        return order?.userName || order?.customerName || order?.username || 'Неизвестен потребител2';
    } catch {
        return 'Грешка при потребителското име';
    }
};
export const getUserNameById1 = async (userId: number): Promise<string> => {
    try {
        const res = await fetch(`${USERS_API}/${userId}`);
        console.log("Status:", res.status);
        const text = await res.text();
        console.log("Response body:", text);
        if (!res.ok) return 'Неизвестен потребител';

        const data = await res.json(); // вече НЕ е масив, а обект

        // Вземи username
        return data.username || data.userName || data.fullName || 'Неизвестен потребител';
    } catch (err) {
        return 'Грешка при потребителското име';
    }
};
