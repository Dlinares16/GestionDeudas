import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import type { AppDispatch, RootState } from "../store/store";
import { setUser, clearAuth } from "../slices/authSlice";
import Cookies from "js-cookie";

export const useAuth = () => {
  const dispatch = useDispatch<AppDispatch>();
  const auth = useSelector((state: RootState) => state.auth);

  useEffect(() => {
    // Solo verificar tokens si no está autenticado ya
    if (auth.isAuthenticated) {
      console.log(
        "useAuth - Usuario ya autenticado, saltando verificación de tokens"
      );
      return;
    }

    // Verificar si hay tokens en cookies al cargar la aplicación
    const accessToken = Cookies.get("accessToken");
    const userId = localStorage.getItem("userId");
    const userData = localStorage.getItem("userData");

    console.log("useAuth - Verificando tokens:", {
      accessToken: !!accessToken,
      userId: !!userId,
      userData: !!userData,
      isAuthenticated: auth.isAuthenticated,
    });

    if (accessToken && userId && userData) {
      try {
        const user = JSON.parse(userData);
        console.log("useAuth - Restaurando usuario:", user);
        dispatch(setUser(user));
      } catch (error) {
        console.error("useAuth - Error al parsear userData:", error);
        // Si hay error al parsear, limpiar todo
        dispatch(clearAuth());
      }
    } else if (accessToken && userId) {
      // Si tenemos token y userId pero no userData, limpiar todo
      console.log("useAuth - Tokens incompletos, limpiando...");
      dispatch(clearAuth());
    }
  }, [dispatch, auth.isAuthenticated]);

  useEffect(() => {
    // Guardar datos del usuario cuando cambie el estado
    console.log("useAuth - Estado de autenticación cambió:", {
      isAuthenticated: auth.isAuthenticated,
      hasUser: !!auth.user,
    });

    if (auth.isAuthenticated && auth.user) {
      localStorage.setItem("userData", JSON.stringify(auth.user));
      localStorage.setItem("userId", auth.user.userId);
      console.log("useAuth - Datos guardados en localStorage");
    } else {
      localStorage.removeItem("userData");
      localStorage.removeItem("userId");
      console.log("useAuth - Datos removidos del localStorage");
    }
  }, [auth.isAuthenticated, auth.user]);

  return {
    ...auth,
    logout: () => dispatch(clearAuth()),
  };
};
