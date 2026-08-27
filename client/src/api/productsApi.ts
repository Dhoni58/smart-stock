export interface Product {
    id: number;
    name: string;
    description: string | null;
    purchasePrice: number;
    sellingPrice: number;
    dphRate: number;
    purchasePriceWithDph: number;
    sellingPriceWithDph: number;
    warehouseInv: number;
    minimumInv: number;
    categoryId: number | null;
    categoryName: string | null;
}

export interface ProductInput {
    name: string;
    description: string | null;
    purchasePrice: number;
    sellingPrice: number;
    minimumInv: number;
    categoryId: number | null;
}

async function handleResponse<T>(res: Response): Promise<T> {
    if (!res.ok) {
        const body = await res.json().catch(() => null);
        throw new Error(body?.message ?? 'Požadavek se nezdařil');
    }
    if (res.status === 204) return undefined as T;
    return res.json();
}

export async function getProducts(): Promise<Product[]> {
    const res = await fetch('/api/products');
    return handleResponse<Product[]>(res);
}

export async function createProduct(input: ProductInput): Promise<Product> {
    const res = await fetch('/api/products', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(input),
    });
    return handleResponse<Product>(res);
}

export async function updateProduct(id: number, input: ProductInput): Promise<void> {
    const res = await fetch(`/api/products/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(input),
    });
    return handleResponse<void>(res);
}

export async function deleteProduct(id: number): Promise<void> {
    const res = await fetch(`/api/products/${id}`, { method: 'DELETE' });
    return handleResponse<void>(res);
}