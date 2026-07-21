import { Outlet } from 'react-router-dom';
import Sidebar from '../components/Sidebar';
import BackgroundPattern from '../components/BackgroundPattern';

export default function AppLayout() {
  return (
    <div style={{ display: 'flex', minHeight: '100vh' }}>
      <Sidebar />
      <main style={{ flex: 1, position: 'relative', overflow: 'hidden' }}>
        <BackgroundPattern />
        <div style={{ position: 'relative', padding: '24px' }}>
          <Outlet />
        </div>
      </main>
    </div>
  );
}