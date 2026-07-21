export default function BackgroundPattern() {
  return (
    <svg
      width="100%"
      height="100%"
      style={{ position: 'absolute', inset: 0, pointerEvents: 'none' }}
      preserveAspectRatio="none"
    >
      <defs>
        <pattern id="grid" width="42" height="42" patternUnits="userSpaceOnUse">
          <path d="M 42 0 L 0 0 0 42" fill="none" stroke="var(--accent)" strokeWidth="0.6" opacity="0.12" />
        </pattern>

        {/* One reusable "block cluster" shape, echoing the icon's stacked squares.
            Defined once, stamped around the screen with <use> below. */}
        <symbol id="blockCluster" viewBox="0 0 60 60">
          <rect x="0" y="20" width="24" height="24" rx="4" />
          <rect x="26" y="0" width="24" height="24" rx="4" />
          <rect x="26" y="26" width="24" height="24" rx="4" />
        </symbol>
      </defs>

      <rect width="100%" height="100%" fill="url(#grid)" />

      <g stroke="var(--accent)" fill="none" strokeWidth="1.2">
        <use href="#blockCluster" x="40" y="40" width="70" height="70" opacity="0.14" />
        <use href="#blockCluster" x="88%" y="6%" width="80" height="80" opacity="0.08" />
        <use href="#blockCluster" x="4%" y="68%" width="60" height="60" opacity="0.1" />
        <use href="#blockCluster" x="76%" y="72%" width="100" height="100" opacity="0.07" />
        <use href="#blockCluster" x="46%" y="88%" width="50" height="50" opacity="0.06" />
      </g>
    </svg>
  );
}