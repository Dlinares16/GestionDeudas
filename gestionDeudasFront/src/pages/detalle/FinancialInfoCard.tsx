import React from "react";
import type { Deuda } from "../../types/deudas";

interface FinancialInfoCardProps {
  deuda: Deuda;
  formatMoney: (amount: number) => string;
}

const FinancialInfoCard: React.FC<FinancialInfoCardProps> = ({
  deuda,
  formatMoney,
}) => {
  return (
    <div className="detail-card">
      <h2>Detalles Financieros</h2>
      <div className="financial-info">
        <div className="financial-item">
          <span className="label">Monto Original:</span>
          <span className="value">{formatMoney(deuda.amount)}</span>
        </div>
        <div className="financial-item">
          <span className="label">Total Pagado:</span>
          <span className="value">{formatMoney(deuda.totalPaid)}</span>
        </div>
        <div className="financial-item">
          <span className="label">Monto Restante:</span>
          <span className="value remaining">
            {formatMoney(deuda.remainingAmount)}
          </span>
        </div>
        <div className="financial-item">
          <span className="label">¿Vencida?:</span>
          <span
            className={`value ${deuda.isOverdue ? "overdue" : "current"}`}
          >
            {deuda.isOverdue ? "Sí" : "No"}
          </span>
        </div>
      </div>
    </div>
  );
};

export default FinancialInfoCard;
