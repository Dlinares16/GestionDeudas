import React from "react";

interface DetailHeaderProps {
  editing: boolean;
  isUpdating: boolean;
  isDeleting: boolean;
  deudaStatus: "pending" | "paid" | "overdue";
  onBack: () => void;
  onEdit: () => void;
  onSave: () => void;
  onCancelEdit: () => void;
  onDelete: () => void;
}

const DetailHeader: React.FC<DetailHeaderProps> = ({
  editing,
  isUpdating,
  isDeleting,
  deudaStatus,
  onBack,
  onEdit,
  onSave,
  onCancelEdit,
  onDelete,
}) => {
  const canEdit = deudaStatus !== "paid";

  return (
    <div className="detail-header">
      <button onClick={onBack} className="back-button">
        <svg
          width="20"
          height="20"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          strokeWidth="2"
        >
          <path d="M19 12H5" />
          <path d="M12 19l-7-7 7-7" />
        </svg>
        Volver
      </button>
      <h1 className="page-title">Detalles de la Deuda</h1>
      <div className="actions">
        {!editing ? (
          canEdit && (
            <button onClick={onEdit} className="edit-button">
              <svg
                width="20"
                height="20"
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
          )
        ) : (
          <div className="edit-actions">
            <button
              onClick={onSave}
              className="save-button"
              disabled={isUpdating}
            >
              {isUpdating ? (
                <>
                  <div className="loading-spinner-small"></div>
                  Guardando...
                </>
              ) : (
                "Guardar"
              )}
            </button>
            <button
              onClick={onCancelEdit}
              className="cancel-button"
              disabled={isUpdating}
            >
              Cancelar
            </button>
          </div>
        )}
        {canEdit && (
          <button
            onClick={onDelete}
            className="delete-button"
            disabled={isDeleting || isUpdating}
          >
            <svg
              width="20"
              height="20"
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
        )}
      </div>
    </div>
  );
};

export default DetailHeader;
