const USERS_API = 'https://localhost:7234/api/Users';

export const getUserNameById = async (userId) => {
    try {
        const res = await fetch(`${USERS_API}/${userId}`);
        if (!res.ok) return 'Неизвестен потребител';

        const data = await res.json(); // Очакваме обект, не масив
        return data.username || data.userName || data.fullName || 'Неизвестен потребител';
    } catch (err) {
        return 'Грешка при потребителското име';
    }
};

export const getUserNameByIdDebug = async (userId) => {
    try {
        const res = await fetch(`${USERS_API}/${userId}`);
        console.log("Status:", res.status);
        const text = await res.text();
        console.log("Response body:", text);

        if (!res.ok) return 'Неизвестен потребител';

        const data = JSON.parse(text); // тъй като сме прочели ръчно body
        return data.username || data.userName || data.fullName || 'Неизвестен потребител';
    } catch (err) {
        return 'Грешка при потребителското име';
    }
};
