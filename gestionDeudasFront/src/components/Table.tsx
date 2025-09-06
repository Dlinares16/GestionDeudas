import React from "react";
import "../styles/Table.css";
import type { Deuda } from "../types/deudas";

interface TableProps {
  deudas: Deuda[];
  onEdit: (deuda: Deuda) => void;
  onDelete: (id: string) => void;
  onView?: (deuda: Deuda) => void;
  loading?: boolean;
  error?: string | null;
}
export const Table: React.FC<TableProps> = ({
  deudas,
  onEdit,
  onDelete,
  onView,
  loading,
  error,
}) => {
  const formatMoney = (amount: number) => {
    return new Intl.NumberFormat("es-ES", {
      style: "currency",
      currency: "EUR",
    }).format(amount);
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString("es-ES", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  };

  const renderEmptyState = () => {
    if (loading) {
      return (
        <tr>
          <td colSpan={6} className="empty-state">
            <div className="loading-container">
              <div className="loading-spinner"></div>
              <p className="loading-text">Cargando deudas...</p>
            </div>
          </td>
        </tr>
      );
    }

    if (error) {
      return (
        <tr>
          <td colSpan={6} className="empty-state">
            <div className="error-container">
              <div className="error-icon">
                <svg
                  width="48"
                  height="48"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  strokeWidth="2"
                >
                  <circle cx="12" cy="12" r="10" />
                  <line x1="15" y1="9" x2="9" y2="15" />
                  <line x1="9" y1="9" x2="15" y2="15" />
                </svg>
              </div>
              <h3 className="error-title">Error al cargar las deudas</h3>
              <button
                className="retry-button"
                onClick={() => window.location.reload()}
              >
                Intentar nuevamente
              </button>
            </div>
          </td>
        </tr>
      );
    }

    if (!deudas || deudas.length === 0) {
      return (
        <tr>
          <td colSpan={6} className="empty-state">
            <div className="empty-container">
              <div className="empty-icon">
                <svg
                  width="64"
                  height="64"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  strokeWidth="1.5"
                >
                  <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
                  <polyline points="14,2 14,8 20,8" />
                  <line x1="12" y1="18" x2="12" y2="12" />
                  <line x1="9" y1="15" x2="15" y2="15" />
                </svg>
              </div>
              <h3 className="empty-title">No tienes deudas registradas</h3>
              <p className="empty-message">
                Cuando agregues nuevas deudas, aparecerán aquí para que puedas
                gestionarlas fácilmente.
              </p>
              <button className="add-debt-button">
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
                Agregar primera deuda
              </button>
            </div>
          </td>
        </tr>
      );
    }

    return null;
  };

  return (
    <div className="table-container">
      <table className="modern-table">
        <thead>
          <tr>
            <th>Deudor</th>
            <th>Descripción</th>
            <th>Monto</th>
            <th>Fecha Límite</th>
            <th>Estado</th>
            <th>Acciones</th>
          </tr>
        </thead>
        <tbody>
          {renderEmptyState() ||
            deudas.map((deuda) => (
              <tr key={deuda.debtId}>
                <td>{deuda.debtor.fullName}</td>
                <td>{deuda.description}</td>
                <td>{formatMoney(deuda.amount)}</td>
                <td>{formatDate(deuda.dueDate)}</td>
                <td>
                  <span className={`status-badge status-${deuda.status}`}>
                    {deuda.status === "pending"
                      ? "Pendiente"
                      : deuda.status === "paid"
                      ? "Pagada"
                      : "Vencida"}
                  </span>
                </td>
                <td>
                  {onView && (
                    <button
                      className="action-button view-button"
                      onClick={() => onView(deuda)}
                      title="Ver detalles"
                    >
                      <svg
                        width="16"
                        height="16"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        strokeWidth="2"
                      >
                        <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" />
                        <circle cx="12" cy="12" r="3" />
                      </svg>
                      Ver
                    </button>
                  )}
                  {deuda.status !== "paid" && (
                    <>
                      <button
                        className="action-button edit-button"
                        onClick={() => onEdit(deuda)}
                        title="Editar deuda"
                      >
                        <svg
                          width="16"
                          height="16"
                          viewBox="0 0 24 24"
                          fill="none"
                          stroke="currentColor"
                          strokeWidth="2"
                        >
                          <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7" />
                          <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z" />
                        </svg>
                        Editar
                      </button>
                      <button
                        className="action-button delete-button"
                        onClick={() => onDelete(deuda.debtId)}
                        title="Eliminar deuda"
                      >
                        <svg
                          width="16"
                          height="16"
                          viewBox="0 0 24 24"
                          fill="none"
                          stroke="currentColor"
                          strokeWidth="2"
                        >
                          <polyline points="3,6 5,6 21,6" />
                          <path d="M19,6v14a2,2,0,0,1-2,2H7a2,2,0,0,1-2-2V6m3,0V4a2,2,0,0,1,2-2h4a2,2,0,0,1,2,2V6" />
                        </svg>
                        Eliminar
                      </button>
                    </>
                  )}
                </td>
              </tr>
            ))}
        </tbody>
      </table>
    </div>
  );
};

export default Table;
