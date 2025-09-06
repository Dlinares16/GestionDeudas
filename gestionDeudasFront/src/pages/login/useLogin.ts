import { useState } from "react";
import { useForm } from "react-hook-form";
import type { SubmitHandler } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import type { AppDispatch, RootState } from "../../store/store";
import { loginAsync, registerAsync } from "../../slices/authSlice";
import type { LoginCredentials, RegisterCredentials } from "../../types/auth";
import type { 
  ILoginInputs, 
  IRegisterInputs, 
  UseLoginReturn 
} from "./loginTypes";

export const useLogin = (): UseLoginReturn => {
  const navigate = useNavigate();
  const dispatch = useDispatch<AppDispatch>();
  const { loading } = useSelector((state: RootState) => state.auth);
  const [isRegisterMode, setIsRegisterMode] = useState(false);

  const {
    register: registerField,
    handleSubmit,
    formState: { errors },
    watch,
    reset,
  } = useForm<IRegisterInputs>();

  const onSubmitLogin: SubmitHandler<ILoginInputs> = async (data) => {
    const credentials: LoginCredentials = {
      email: data.email,
      password: data.password,
    };

    const result = await dispatch(loginAsync(credentials));

    if (loginAsync.fulfilled.match(result)) {
      navigate("/home");
    }
  };

  const onSubmitRegister: SubmitHandler<IRegisterInputs> = async (data) => {
    const credentials: RegisterCredentials = {
      email: data.email,
      firstName: data.firstName,
      lastName: data.lastName,
      password: data.password,
    };

    const result = await dispatch(registerAsync(credentials));

    if (registerAsync.fulfilled.match(result)) {
      navigate("/home");
    }
  };

  const onSubmit: SubmitHandler<IRegisterInputs> = async (data) => {
    if (isRegisterMode) {
      await onSubmitRegister(data);
    } else {
      await onSubmitLogin(data as ILoginInputs);
    }
  };

  const toggleMode = () => {
    setIsRegisterMode(!isRegisterMode);
    reset(); // Limpiar el formulario al cambiar de modo
  };

  const password = watch("password");

  return {
    // Estados
    isRegisterMode,
    loading,
    
    // Formulario
    registerField,
    handleSubmit,
    errors,
    watch,
    
    // Acciones
    onSubmit,
    toggleMode,
    
    // Datos calculados
    password,
  };
};
