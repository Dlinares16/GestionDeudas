import React from "react";

interface HomeHeaderProps {
  userName?: string;
  onCreateClick: () => void;
}

const HomeHeader: React.FC<HomeHeaderProps> = ({ userName, onCreateClick }) => {
  return (
    <div className="home-header">
      <div className="header-content">
        <h1 className="page-title">Dashboard</h1>
        <p className="page-subtitle">
          Bienvenido, {userName}. Aquí puedes gestionar todas tus deudas.
        </p>
      </div>
      <button onClick={onCreateClick} className="create-button">
        <svg
          width="20"
          height="20"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="2"
        >
          <line x1="12" y1="5" x2="12" y2="19" />
          <line x1="5" y1="12" x2="19" y2="12" />
        </svg>
        Nueva Deuda
      </button>
    </div>
  );
};

export default HomeHeader;
