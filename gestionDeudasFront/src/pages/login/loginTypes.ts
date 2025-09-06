import type { SubmitHandler } from "react-hook-form";

export interface ILoginInputs {
  email: string;
  password: string;
}

export interface IRegisterInputs {
  email: string;
  firstName: string;
  lastName: string;
  password: string;
  confirmPassword: string;
}

export interface UseLoginReturn {
  // Estados
  isRegisterMode: boolean;
  loading: boolean;
  
  // Formulario
  registerField: any;
  handleSubmit: any;
  errors: any;
  watch: any;
  
  // Acciones
  onSubmit: SubmitHandler<IRegisterInputs>;
  toggleMode: () => void;
  
  // Datos calculados
  password: string;
}
