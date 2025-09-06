import React from "react";

interface ErrorStateProps {
  message?: string;
  onBack: () => void;
}

const ErrorState: React.FC<ErrorStateProps> = ({ 
  message = "Deuda no encontrada",
  onBack 
}) => {
  return (
    <div className="page-container">
      <div className="error-container">
        <h2>{message}</h2>
        <button onClick={onBack} className="back-button">
          Volver al inicio
        </button>
      </div>
    </div>
  );
};

export default ErrorState;
