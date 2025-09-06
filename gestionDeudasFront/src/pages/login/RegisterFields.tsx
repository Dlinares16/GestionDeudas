import React from "react";
import FormField from "./FormField";

interface RegisterFieldsProps {
  registerField: any;
  errors: any;
}

const RegisterFields: React.FC<RegisterFieldsProps> = ({
  registerField,
  errors,
}) => {
  return (
    <>
      <FormField
        type="text"
        placeholder="Nombre"
        registerField={registerField}
        fieldName="firstName"
        rules={{
          required: "El nombre es requerido",
          minLength: {
            value: 2,
            message: "El nombre debe tener al menos 2 caracteres",
          },
        }}
        error={errors.firstName?.message}
      />

      <FormField
        type="text"
        placeholder="Apellido"
        registerField={registerField}
        fieldName="lastName"
        rules={{
          required: "El apellido es requerido",
          minLength: {
            value: 2,
            message: "El apellido debe tener al menos 2 caracteres",
          },
        }}
        error={errors.lastName?.message}
      />
    </>
  );
};

export default RegisterFields;
