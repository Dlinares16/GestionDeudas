import React from "react";

interface LoginHeaderProps {
  isRegisterMode: boolean;
}

const LoginHeader: React.FC<LoginHeaderProps> = ({ isRegisterMode }) => {
  return (
    <>
      <h1>{isRegisterMode ? "Crear Cuenta" : "Bienvenido"}</h1>
      <p className="login-subtitle">
        {isRegisterMode ? "Regístrate para comenzar" : "Ingresa a tu cuenta"}
      </p>
    </>
  );
};

export default LoginHeader;
