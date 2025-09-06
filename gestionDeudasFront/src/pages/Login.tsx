import React from "react";
import { 
  useLogin, 
  LoginHeader, 
  LoginForm, 
  ModeToggle 
} from "./login/index";
import "../styles/Login.css";

const Login: React.FC = () => {
  const {
    // Estados
    isRegisterMode,
    loading,
    
    // Formulario
    registerField,
    handleSubmit,
    errors,
    
    // Acciones
    onSubmit,
    toggleMode,
    
    // Datos calculados
    password,
  } = useLogin();

  return (
    <div className="login-container">
      <div className="login-card">
        <LoginHeader isRegisterMode={isRegisterMode} />

        <LoginForm
          isRegisterMode={isRegisterMode}
          loading={loading}
          registerField={registerField}
          handleSubmit={handleSubmit}
          onSubmit={onSubmit}
          errors={errors}
          password={password}
        />

        <ModeToggle
          isRegisterMode={isRegisterMode}
          loading={loading}
          onToggle={toggleMode}
        />
      </div>
    </div>
  );
};

export default Login;
