import React from "react";

interface FormFieldProps {
  type: "text" | "email" | "password";
  placeholder: string;
  registerField: any;
  fieldName: string;
  rules: any;
  error?: string;
}

const FormField: React.FC<FormFieldProps> = ({
  type,
  placeholder,
  registerField,
  fieldName,
  rules,
  error,
}) => {
  return (
    <div className="form-group">
      <input
        type={type}
        placeholder={placeholder}
        {...registerField(fieldName, rules)}
      />
      {error && <span className="error-message">{error}</span>}
    </div>
  );
};

export default FormField;
