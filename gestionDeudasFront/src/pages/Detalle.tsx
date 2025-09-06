import React from "react";
import {
  useDetalle,
  LoadingState,
  ErrorState,
  DetailHeader,
  GeneralInfoCard,
  PersonInfoCard,
  FinancialInfoCard,
  DeleteConfirmationModal,
} from "./detalle/index";
import "../styles/Detalle.css";

const Detalle: React.FC = () => {
  const {
    // Estados de datos
    deuda,
    formData,
    
    // Estados de UI
    loading,
    editing,
    showDeleteModal,
    
    // Estados de operaciones
    isUpdating,
    isDeleting,
    
    // Funciones de manejo
    setEditing,
    setShowDeleteModal,
    updateFormData,
    
    // Acciones
    handleSave,
    handleDelete,
    handleBack,
    
    // Funciones de utilidad
    formatMoney,
    formatDate,
  } = useDetalle();

  if (loading) {
    return <LoadingState />;
  }

  if (!deuda) {
    return <ErrorState onBack={handleBack} />;
  }

  return (
    <div className="page-container">
      <DetailHeader
        editing={editing}
        isUpdating={isUpdating}
        isDeleting={isDeleting}
        deudaStatus={deuda.status}
        onBack={handleBack}
        onEdit={() => setEditing(true)}
        onSave={handleSave}
        onCancelEdit={() => setEditing(false)}
        onDelete={() => setShowDeleteModal(true)}
      />

      <div className="detail-content">
        <GeneralInfoCard
          deuda={deuda}
          formData={formData}
          editing={editing}
          onFieldChange={updateFormData}
          formatMoney={formatMoney}
          formatDate={formatDate}
        />

        <PersonInfoCard
          title="Información del Deudor"
          person={deuda.debtor}
        />

        <PersonInfoCard
          title="Información del Acreedor"
          person={deuda.creditor}
        />

        <FinancialInfoCard
          deuda={deuda}
          formatMoney={formatMoney}
        />
      </div>

      <DeleteConfirmationModal
        isOpen={showDeleteModal}
        isDeleting={isDeleting}
        onClose={() => setShowDeleteModal(false)}
        onConfirm={handleDelete}
      />
    </div>
  );
};

export default Detalle;
