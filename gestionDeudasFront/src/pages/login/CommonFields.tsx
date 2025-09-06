import React from "react";
import FormField from "./FormField";

interface CommonFieldsProps {
  registerField: any;
  errors: any;
  isRegisterMode: boolean;
  password: string;
}

const CommonFields: React.FC<CommonFieldsProps> = ({
  registerField,
  errors,
  isRegisterMode,
  password,
}) => {
  return (
    <>
      <FormField
        type="email"
        placeholder="Email"
        registerField={registerField}
        fieldName="email"
        rules={{
          required: "El email es requerido",
          pattern: {
            value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
            message: "Email inválido",
          },
        }}
        error={errors.email?.message}
      />

      <FormField
        type="password"
        placeholder="Contraseña"
        registerField={registerField}
        fieldName="password"
        rules={{
          required: "La contraseña es requerida",
          minLength: {
            value: 6,
            message: "La contraseña debe tener al menos 6 caracteres",
          },
        }}
        error={errors.password?.message}
      />

      {isRegisterMode && (
        <FormField
          type="password"
          placeholder="Confirmar Contraseña"
          registerField={registerField}
          fieldName="confirmPassword"
          rules={{
            required: "Debes confirmar la contraseña",
            validate: (value: string) =>
              value === password || "Las contraseñas no coinciden",
          }}
          error={errors.confirmPassword?.message}
        />
      )}
    </>
  );
};

export default CommonFields;
