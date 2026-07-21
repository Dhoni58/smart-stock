import { NavLink } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import logo from '../assets/brand/logo_white.png';
import './Sidebar.css';
import { LayoutDashboard, Package, ArrowLeftRight, Truck, Receipt, UserCog } from 'lucide-react';
import { useClock } from '../hooks/useClock';


const navItems = [
  { to: '/', label: 'Přehled', icon: LayoutDashboard, roles: ['Vedouci', 'Skladnik'] },
  { to: '/products', label: 'Produkty',icon: Package, roles: ['Vedouci', 'Skladnik'] },
  { to: '/movements', label: 'Pohyby', icon: ArrowLeftRight, roles: ['Vedouci', 'Skladnik'] },
  { to: '/suppliers', label: 'Dodavatelé', icon: Truck, roles: ['Vedouci', 'Skladnik'] },
  { to: '/invoices', label: 'Faktury', icon: Receipt, roles: ['Vedouci', 'Skladnik'] },
  { to: '/users', label: 'Uživatelé', icon: UserCog, roles: ['Vedouci'] }, // admin only
];

export default function Sidebar() {
  const { user, logout } = useAuth();
  const now = useClock();

  const timeStr = now.toLocaleTimeString('cs-CZ', { hour: '2-digit', minute: '2-digit', second: '2-digit'});
  const dateStr = now.toLocaleDateString('cs-CZ', { weekday: 'long', day: 'numeric', month: 'long'}); 

  const visibleItems = navItems.filter((item) => user && item.roles.includes(user.role));

   return (
    <aside className="sidebar">
      <div className="sidebar__logo">
        <img src={logo} alt="Smart Stock" />
      </div>

      <div className="sidebar__clock">
        <p className="sidebar__clock-time">{timeStr}</p>
        <p className="sidebar__clock-date">{dateStr}</p>
      </div>

      <nav className="sidebar__nav">
        {visibleItems.map((item) => {
          const Icon = item.icon;
          return (
          <NavLink
            key={item.to}
            to={item.to}
            end={item.to === '/'}
            className={({ isActive }) =>
              isActive ? 'sidebar__link sidebar__link--active' : 'sidebar__link'
            }
          >
            <Icon size={18} strokeWidth={1.75} />
            <span>{item.label}</span>
          </NavLink>
          );
        })}
      </nav>

      <div className="sidebar__footer">
        <p>{user?.name} ({user?.role})</p>
        <button className="sidebar__logout" onClick={logout}>Odhlásit</button>
      </div>
    </aside>
  );
}