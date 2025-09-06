import React from "react";
import RegisterFields from "./RegisterFields";
import CommonFields from "./CommonFields";
import SubmitButton from "./SubmitButton";

interface LoginFormProps {
  isRegisterMode: boolean;
  loading: boolean;
  registerField: any;
  handleSubmit: any;
  onSubmit: any;
  errors: any;
  password: string;
}

const LoginForm: React.FC<LoginFormProps> = ({
  isRegisterMode,
  loading,
  registerField,
  handleSubmit,
  onSubmit,
  errors,
  password,
}) => {
  return (
    <form onSubmit={handleSubmit(onSubmit)} className="login-form">
      {isRegisterMode && (
        <RegisterFields registerField={registerField} errors={errors} />
      )}

      <CommonFields
        registerField={registerField}
        errors={errors}
        isRegisterMode={isRegisterMode}
        password={password}
      />

      <SubmitButton isRegisterMode={isRegisterMode} loading={loading} />
    </form>
  );
};

export default LoginForm;
