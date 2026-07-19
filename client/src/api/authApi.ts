export interface AuthUser {
    id: number;
    name: string;
    role: string;
}

export async function login(pin: string): Promise<AuthUser> {
    const res = await fetch('/api/auth/login', {
       method: 'POST',
       headers: { 'Content-Type': 'application/json' },
       body: JSON.stringify({pin}),
    });

    if (!res.ok) {
        const body = await res.json().catch(() => null);
        throw new Error(body?.message ?? 'Přihlášení se nezdařilo');
    }

    return res.json();

}

export async function logout(): Promise<void> {
    await fetch('/api/auth/logout', {method: 'POST'});
}

export async function getCurrentUser(): Promise<AuthUser | null> {
    const res = await fetch('/api/auth/me');
    if (!res.ok) return null;
    return res.json();
}