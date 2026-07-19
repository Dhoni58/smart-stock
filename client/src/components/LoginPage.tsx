import { useState } from 'react';
import type { FormEvent } from 'react';
import { login, type AuthUser } from '../api/authApi';
import AuthBackground from './AuthBackground';

interface LoginPageProps {
  onLoginSuccess: (user: AuthUser) => void;
}

export default function LoginPage({ onLoginSuccess }: LoginPageProps) {
  const [pin, setPin] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const user = await login(pin);
      onLoginSuccess(user);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Přihlášení se nezdařilo.');
      setPin('');
    } finally {
      setLoading(false);
    }
  }

  return (
    <AuthBackground>
      <form
        onSubmit={handleSubmit}
        style={{
          background: 'var(--bg-card)',
          border: '0.5px solid var(--border-subtle)',
          borderRadius: '10px',
          padding: '28px 22px',
          width: '220px',
          textAlign: 'center',
        }}
      >
        <p style={{ fontSize: '14px', fontWeight: 500, margin: '0 0 16px' }}>
          smart-stock
        </p>

        <input
          type="password"
          inputMode="numeric"
          maxLength={5}
          placeholder="PIN"
          value={pin}
          autoFocus
          onChange={(e) => setPin(e.target.value.replace(/\D/g, '').slice(0, 5))}
          style={{
            width: '100%',
            textAlign: 'center',
            letterSpacing: '6px',
            fontSize: '16px',
            marginBottom: '10px',
            background: 'var(--bg-page)',
            color: 'var(--text-primary)',
            border: '1px solid var(--border-input)',
            borderRadius: '6px',
            padding: '8px 0',
          }}
        />

        <button
          type="submit"
          disabled={loading || pin.length !== 5}
          style={{
            width: '100%',
            background: 'var(--accent)',
            color: '#fff',
            border: 'none',
            borderRadius: '6px',
            padding: '8px 0',
            cursor: loading ? 'default' : 'pointer',
            opacity: loading ? 0.7 : 1,
          }}
        >
          {loading ? 'Přihlašuji...' : 'Přihlásit'}
        </button>

        {error && (
          <p style={{ fontSize: '12px', color: 'var(--danger)', marginTop: '10px' }}>
            {error}
          </p>
        )}
      </form>
    </AuthBackground>
  );
}