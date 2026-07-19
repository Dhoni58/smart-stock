import { useEffect, useState } from 'react';
import LoginPage from './components/LoginPage';
import { getCurrentUser, logout, type AuthUser } from './api/authApi';

export default function App() {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [checkingSession, setCheckingSession] = useState(true);

  useEffect(() => {
    getCurrentUser()
      .then(setUser)
      .finally(() => setCheckingSession(false));
  }, []);

  async function handleLogout() {
    await logout();
    setUser(null);
  }

  if (checkingSession) {
    return null;
  }

  if (!user) {
    return <LoginPage onLoginSuccess={setUser} />;
  }

  return (
    <div style={{ padding: '2rem' }}>
      <p>
        Přihlášen: {user.name} ({user.role})
      </p>
      <button onClick={handleLogout}>Odhlásit</button>
    </div>
  );
}