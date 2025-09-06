import React from "react";

interface SubmitButtonProps {
  isRegisterMode: boolean;
  loading: boolean;
}

const SubmitButton: React.FC<SubmitButtonProps> = ({
  isRegisterMode,
  loading,
}) => {
  const getButtonText = () => {
    if (loading) {
      return isRegisterMode ? "Registrando..." : "Iniciando sesión...";
    }
    return isRegisterMode ? "Crear Cuenta" : "Iniciar Sesión";
  };

  return (
    <button type="submit" className="login-button" disabled={loading}>
      {getButtonText()}
    </button>
  );
};

export default SubmitButton;
