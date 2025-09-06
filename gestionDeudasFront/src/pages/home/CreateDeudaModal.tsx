import React from "react";
import type { CreateDeudaForm } from "./homeTypes";

interface CreateDeudaModalProps {
  isOpen: boolean;
  createForm: CreateDeudaForm;
  users: any[];
  usersLoading: boolean;
  isCreating: boolean;
  onClose: () => void;
  onSubmit: (e: React.FormEvent) => Promise<void>;
  onFormChange: (field: keyof CreateDeudaForm, value: string | number) => void;
}

const CreateDeudaModal: React.FC<CreateDeudaModalProps> = ({
  isOpen,
  createForm,
  users,
  usersLoading,
  isCreating,
  onClose,
  onSubmit,
  onFormChange,
}) => {
  if (!isOpen) return null;

  return (
    <div className="modal-overlay">
      <div className="modal large-modal">
        <div className="modal-header">
          <h3>Crear Nueva Deuda</h3>
          <button onClick={onClose} className="close-button">
            <svg
              width="24"
              height="24"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              <line x1="18" y1="6" x2="6" y2="18" />
              <line x1="6" y1="6" x2="18" y2="18" />
            </svg>
          </button>
        </div>
        <form onSubmit={onSubmit}>
          <div className="modal-body">
            <div className="form-grid">
              <div className="form-group">
                <label htmlFor="debtorId">Deudor *</label>
                <select
                  id="debtorId"
                  value={createForm.debtorId}
                  onChange={(e) => onFormChange("debtorId", e.target.value)}
                  className="form-input"
                  required
                  disabled={usersLoading}
                >
                  <option value="">
                    {usersLoading ? "Cargando usuarios..." : "Seleccionar deudor"}
                  </option>
                  {users.map((user) => (
                    <option key={user.userId} value={user.userId}>
                      {user.fullName}
                    </option>
                  ))}
                </select>
              </div>

              <div className="form-group">
                <label htmlFor="amount">Monto *</label>
                <input
                  type="number"
                  id="amount"
                  step="0.01"
                  min="0.01"
                  value={createForm.amount}
                  onChange={(e) => onFormChange("amount", e.target.value)}
                  placeholder="0.00"
                  className="form-input"
                  required
                />
              </div>

              <div className="form-group full-width">
                <label htmlFor="description">Descripción *</label>
                <textarea
                  id="description"
                  value={createForm.description}
                  onChange={(e) => onFormChange("description", e.target.value)}
                  className="form-input"
                  rows={3}
                  placeholder="Describe el motivo de la deuda..."
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="dueDate">Fecha Límite</label>
                <input
                  type="date"
                  id="dueDate"
                  value={createForm.dueDate}
                  onChange={(e) => onFormChange("dueDate", e.target.value)}
                  className="form-input"
                />
              </div>
            </div>
          </div>
          <div className="modal-footer">
            <button
              type="button"
              onClick={onClose}
              className="cancel-button"
              disabled={isCreating}
            >
              Cancelar
            </button>
            <button
              type="submit"
              className="save-button"
              disabled={isCreating}
            >
              {isCreating ? (
                <>
                  <div className="loading-spinner-small"></div>
                  Creando...
                </>
              ) : (
                "Crear Deuda"
              )}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CreateDeudaModal;
