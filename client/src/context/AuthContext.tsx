import { createContext, useContext, useEffect, useState, type ReactNode } from 'react';
import { getCurrentUser, login as apiLogin, logout as apiLogout, type AuthUser } from '../api/authApi';

interface AuthContextValue {
    user: AuthUser | null;
    loading: boolean;
    login: (pin: string) => Promise<AuthUser>;
    logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<AuthUser | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        getCurrentUser()
        .then(setUser)
        .finally(() => setLoading(false));
    }, []);
    async function login(pin: string) {
        const loggedInUser = await apiLogin(pin);
        setUser(loggedInUser);
        return loggedInUser;
        }

    async function logout() {
        await apiLogout();
        setUser(null);
    }

    return (
        <AuthContext.Provider value={{ user, loading, login, logout }}>
        {children}
        </AuthContext.Provider>
    );
}
export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside an AuthProvider');
  return ctx;
}