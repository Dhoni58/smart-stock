import type { ReactNode } from 'react';
import BackgroundPattern from './BackgroundPattern';

interface AuthBackgroundProps {
  children: ReactNode;
}

export default function AuthBackground({ children }: AuthBackgroundProps) {
  return (
    <div
      style={{
        position: 'relative',
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        overflow: 'hidden',
      }}
    >
      <BackgroundPattern />
      <div style={{ position: 'relative' }}>{children}</div>
    </div>
  );
}
