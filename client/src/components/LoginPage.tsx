import { useState } from 'react';
import type { FormEvent } from 'react';
import { useAuth } from '../context/AuthContext';
import AuthBackground from './AuthBackground';
import logo from '../assets/brand/logo_white.png';

export default function LoginPage() {
  const { login } = useAuth();
  const [pin, setPin] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      await login(pin);
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
          background: 'var(--card)',
          border: '0.5px solid var(--border)',
          borderRadius: '10px',
          padding: '36px 30px',
          width: '280px',
          textAlign: 'center',
        }}
      >
        <img
          src={logo}
          alt="smart Stock"
          style={{ width: '160px', display: 'block', margin: '0 auto 20px'}}></img>

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
            background: 'var(--background)',
            color: 'var(--foreground)',
            border: '1px solid var(--input)',
            borderRadius: '6px',
            padding: '8px 0',
          }}
        />

        <button
          type="submit"
          disabled={loading || pin.length !== 5}
          style={{
            width: '100%',
            background: 'var(--primary)',
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
          <p style={{ fontSize: '12px', color: 'var(--destructive)', marginTop: '10px' }}>
            {error}
          </p>
        )}
      </form>
    </AuthBackground>
  );
}