const USERS_API = 'https://localhost:7067/api/User';

export const getUserNameById = async (userId: number): Promise<string> => {
    try {
        const res = await fetch(`${USERS_API}/${userId}`);
        if (!res.ok) return 'Неизвестен потребител';
        const data = await res.json();
        return data.name || data.fullName || data.username || 'Неизвестен потребител';
    } catch {
        return 'Грешка при потребителското име';
    }
};
