import React from "react";

interface ModeToggleProps {
  isRegisterMode: boolean;
  loading: boolean;
  onToggle: () => void;
}

const ModeToggle: React.FC<ModeToggleProps> = ({
  isRegisterMode,
  loading,
  onToggle,
}) => {
  return (
    <div className="toggle-mode">
      <p>
        {isRegisterMode ? "¿Ya tienes cuenta? " : "¿No tienes cuenta? "}
        <button
          type="button"
          className="toggle-link"
          onClick={onToggle}
          disabled={loading}
        >
          {isRegisterMode ? "Inicia Sesión" : "Regístrate"}
        </button>
      </p>
    </div>
  );
};

export default ModeToggle;
