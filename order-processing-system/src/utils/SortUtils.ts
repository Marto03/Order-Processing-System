export function sortArray<T>(arr: T[], column: keyof T, direction: 'asc' | 'desc') {
    return [...arr].sort((a, b) => {
        const aVal = a[column];
        const bVal = b[column];

        if (typeof aVal === 'number' && typeof bVal === 'number') {
            return direction === 'asc' ? aVal - bVal : bVal - aVal;
        }

        if (typeof aVal === 'string' && typeof bVal === 'string') {
            return direction === 'asc'
                ? aVal.localeCompare(bVal)
                : bVal.localeCompare(aVal);
        }

        return 0;
    });
}
