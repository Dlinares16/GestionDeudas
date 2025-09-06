import React, { useEffect } from "react";
import { Navigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import { useAuth } from "../hooks/useAuth";
import { setUser } from "../slices/authSlice";
import Cookies from "js-cookie";
import type { AppDispatch } from "../store/store";

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ children }) => {
  const { isAuthenticated, user } = useAuth();
  const dispatch = useDispatch<AppDispatch>();

  useEffect(() => {
    // Verificar si hay tokens y datos de usuario guardados
    const token = Cookies.get("accessToken");
    const userDataString = localStorage.getItem("userData");

    if (token && userDataString && !user) {
      try {
        const userData = JSON.parse(userDataString);
        dispatch(setUser(userData));
      } catch (error) {
        console.error("Error al parsear datos de usuario:", error);
        // Limpiar datos corruptos
        Cookies.remove("accessToken");
        Cookies.remove("refreshToken");
        localStorage.removeItem("userId");
        localStorage.removeItem("userData");
      }
    }
  }, [dispatch, user]);

  // Si no está autenticado, redirigir al login
  if (!isAuthenticated && !Cookies.get("accessToken")) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
};

export default ProtectedRoute;