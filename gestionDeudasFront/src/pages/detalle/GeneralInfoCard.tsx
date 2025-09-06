import React from "react";
import type { Deuda } from "../../types/deudas";
import type { DeudaFormData } from "./detalleTypes";

interface GeneralInfoCardProps {
  deuda: Deuda;
  formData: DeudaFormData;
  editing: boolean;
  onFieldChange: (field: keyof DeudaFormData, value: any) => void;
  formatMoney: (amount: number) => string;
  formatDate: (dateString: string) => string;
}

const GeneralInfoCard: React.FC<GeneralInfoCardProps> = ({
  deuda,
  formData,
  editing,
  onFieldChange,
  formatMoney,
  formatDate,
}) => {
  const getStatusLabel = (status: string) => {
    switch (status) {
      case "pending": return "Pendiente";
      case "paid": return "Pagada";
      case "overdue": return "Vencida";
      default: return status;
    }
  };

  // No permitir edición si la deuda está pagada
  const canEdit = editing && deuda.status !== "paid";

  return (
    <div className="detail-card">
      <div className="card-header">
        <h2>Información General</h2>
        <span className={`status-badge status-${deuda.status}`}>
          {getStatusLabel(deuda.status)}
        </span>
        {deuda.status === "paid" && editing && (
          <span className="info-message">Las deudas pagadas no se pueden modificar</span>
        )}
      </div>

      <div className="detail-grid">
        <div className="detail-field">
          <label>Descripción</label>
          {canEdit ? (
            <textarea
              value={formData.description}
              onChange={(e) => onFieldChange("description", e.target.value)}
              className="form-input"
              rows={3}
            />
          ) : (
            <p>{deuda.description}</p>
          )}
        </div>

        <div className="detail-field">
          <label>Monto</label>
          {canEdit ? (
            <input
              type="number"
              step="0.01"
              value={formData.amount}
              onChange={(e) => onFieldChange("amount", parseFloat(e.target.value))}
              className="form-input"
            />
          ) : (
            <p className="amount">{formatMoney(deuda.amount)}</p>
          )}
        </div>

        <div className="detail-field">
          <label>Fecha Límite</label>
          {canEdit ? (
            <input
              type="date"
              value={formData.dueDate}
              onChange={(e) => onFieldChange("dueDate", e.target.value)}
              className="form-input"
            />
          ) : (
            <p>{formatDate(deuda.dueDate)}</p>
          )}
        </div>

        <div className="detail-field">
          <label>Estado</label>
          {canEdit ? (
            <select
              value={formData.status}
              onChange={(e) => onFieldChange("status", e.target.value as any)}
              className="form-input"
            >
              <option value="pending">Pendiente</option>
              <option value="paid">Pagada</option>
            </select>
          ) : (
            <p>{getStatusLabel(deuda.status)}</p>
          )}
        </div>
      </div>
    </div>
  );
};

export default GeneralInfoCard;
