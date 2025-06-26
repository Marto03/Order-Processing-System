export const formatDate = (dateStr: string) =>
    new Date(dateStr).toLocaleString('bg-BG');

export const isValidAmount = (value: string) =>
    !isNaN(parseFloat(value.replace(',', '.')));
