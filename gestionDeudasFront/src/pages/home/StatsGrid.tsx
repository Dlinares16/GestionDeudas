import React from "react";

interface StatCardProps {
  title: string;
  value: number | string;
  iconType: "success" | "warning" | "danger" | "info";
  children?: React.ReactNode;
}

const StatCard: React.FC<StatCardProps> = ({ title, value, iconType, children }) => {
  const renderIcon = () => {
    switch (iconType) {
      case "success":
        return (
          <svg
            width="24"
            height="24"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
          >
            <path d="M20 6L9 17l-5-5" />
          </svg>
        );
      case "warning":
        return (
          <svg
            width="24"
            height="24"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
          >
            <circle cx="12" cy="12" r="10" />
            <line x1="12" y1="8" x2="12" y2="12" />
            <line x1="12" y1="16" x2="12.01" y2="16" />
          </svg>
        );
      case "danger":
        return (
          <svg
            width="24"
            height="24"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
          >
            <circle cx="12" cy="12" r="10" />
            <line x1="15" y1="9" x2="9" y2="15" />
            <line x1="9" y1="9" x2="15" y2="15" />
          </svg>
        );
      case "info":
        return (
          <svg
            width="24"
            height="24"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
          >
            <line x1="12" y1="1" x2="12" y2="23" />
            <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6" />
          </svg>
        );
      default:
        return null;
    }
  };

  return (
    <div className="stat-card">
      <div className={`stat-icon ${iconType}`}>
        {renderIcon()}
      </div>
      <div className="stat-content">
        <h3>{title}</h3>
        <p className="stat-number">
          {children || value}
        </p>
      </div>
    </div>
  );
};

interface StatsGridProps {
  paid: number;
  pending: number;
  overdue: number;
  totalOwed: number;
}

const StatsGrid: React.FC<StatsGridProps> = ({ paid, pending, overdue, totalOwed }) => {
  return (
    <div className="stats-grid">
      <StatCard
        title="Deudas Pagadas"
        value={paid}
        iconType="success"
      />
      
      <StatCard
        title="Deudas Pendientes"
        value={pending}
        iconType="warning"
      />
      
      <StatCard
        title="Deudas Vencidas"
        value={overdue}
        iconType="danger"
      />
      
      <StatCard
        title="Total Adeudado"
        value=""
        iconType="info"
      >
        ${totalOwed.toLocaleString()}
      </StatCard>
    </div>
  );
};

export default StatsGrid;
