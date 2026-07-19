import type { ReactNode } from 'react';

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
      <svg
        width="100%"
        height="100%"
        style={{ position: 'absolute', inset: 0 }}
        preserveAspectRatio="none"
      >
        <defs>
          <pattern id="grid" width="42" height="42" patternUnits="userSpaceOnUse">
            <path
              d="M 42 0 L 0 0 0 42"
              fill="none"
              stroke="var(--grid-line)"
              strokeWidth="0.6"
              opacity="0.12"
            />
          </pattern>
        </defs>
        <rect width="100%" height="100%" fill="url(#grid)" />
        <g stroke="var(--grid-line)" strokeWidth="1.2" fill="none" opacity="0.14">
          <rect x="40" y="40" width="46" height="30" rx="2" />
          <line x1="40" y1="55" x2="86" y2="55" />
          <line x1="63" y1="40" x2="63" y2="70" />
        </g>
      </svg>

      <div style={{ position: 'relative' }}>{children}</div>
    </div>
  );
}