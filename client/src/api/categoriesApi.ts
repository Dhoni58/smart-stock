export interface Category {
    id: number;
    name: string;
}

export async function getCategories(): Promise<Category[]> {
    const res = await fetch('/api/categories');
    if (!res.ok) throw new Error('Kategorie se nepodařilo načíst');
    return res.json();
}