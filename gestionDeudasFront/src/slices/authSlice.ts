import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { AuthState, LoginCredentials, RegisterCredentials, User } from "../types/auth";
import { login, register, logout } from "../actions/authActions";
import toast from "react-hot-toast";
import Cookies from "js-cookie";

const initialState: AuthState = {
  user: null,
  accessToken: null,
  refreshToken: null,
  expiresAt: null,
  loading: false,
  error: null,
  isAuthenticated: false,
};

export const loginAsync = createAsyncThunk(
  "auth/login",
  async (credentials: LoginCredentials, { rejectWithValue }) => {
    try {
      const response = await login(credentials);

      // Guardar tokens en cookies de forma segura
      const isProduction = window.location.protocol === "https:";
      const isDevelopment =
        window.location.hostname === "localhost" ||
        window.location.hostname === "127.0.0.1";

      console.log("Configuración de entorno:", { isProduction, isDevelopment });

      Cookies.set("accessToken", response.accessToken, {
        secure: isProduction, // Solo en HTTPS en producción
        sameSite: isDevelopment ? "lax" : "strict", // Más permisivo en desarrollo
        expires: new Date(response.expiresAt),
      });

      Cookies.set("refreshToken", response.refreshToken, {
        secure: isProduction, // Solo en HTTPS en producción
        sameSite: isDevelopment ? "lax" : "strict",
        expires: 30, // 30 días
      });

      // Guardar userId en localStorage
      localStorage.setItem("userId", response.user.userId);

      // También guardar los datos del usuario para verificación
      localStorage.setItem("userData", JSON.stringify(response.user));

      // Verificar que se guardaron correctamente
      const tokenVerification = Cookies.get("accessToken");
      const userIdVerification = localStorage.getItem("userId");

      console.log("Verificación después de guardar:", {
        accessToken: response.accessToken?.substring(0, 50) + "...",
        userId: response.user.userId,
        cookieSet: !!tokenVerification,
        localStorageSet: !!userIdVerification,
        cookieValue: tokenVerification?.substring(0, 50) + "...",
      });

      toast.success("¡Inicio de sesión exitoso!");
      return response;
    } catch (error: any) {
      const errorMessage =
        error.response?.data?.message || "Error en el inicio de sesión";
      toast.error(errorMessage);
      return rejectWithValue(errorMessage);
    }
  }
);

export const registerAsync = createAsyncThunk(
  "auth/register",
  async (credentials: RegisterCredentials, { rejectWithValue }) => {
    try {
      const response = await register(credentials);

      // Guardar tokens en cookies de forma segura (igual que en login)
      const isProduction = window.location.protocol === "https:";
      const isDevelopment =
        window.location.hostname === "localhost" ||
        window.location.hostname === "127.0.0.1";

      Cookies.set("accessToken", response.accessToken, {
        secure: isProduction,
        sameSite: isDevelopment ? "lax" : "strict",
        expires: new Date(response.expiresAt),
      });

      Cookies.set("refreshToken", response.refreshToken, {
        secure: isProduction,
        sameSite: isDevelopment ? "lax" : "strict",
        expires: 30, // 30 días
      });

      // Guardar userId en localStorage
      localStorage.setItem("userId", response.user.userId);
      localStorage.setItem("userData", JSON.stringify(response.user));

      toast.success("¡Registro exitoso! Bienvenido!");
      return response;
    } catch (error: any) {
      const errorMessage =
        error.response?.data?.message || "Error en el registro";
      toast.error(errorMessage);
      return rejectWithValue(errorMessage);
    }
  }
);

export const logoutAsync = createAsyncThunk(
  "auth/logout",
  async (_, { rejectWithValue }) => {
    try {
      // Obtener el refreshToken de las cookies antes de limpiarlo
      const refreshToken = Cookies.get("refreshToken");
      
      if (refreshToken) {
        // Llamar al endpoint de logout con el refreshToken
        await logout(refreshToken);
      }

      // Limpiar cookies y localStorage
      Cookies.remove("accessToken");
      Cookies.remove("refreshToken");
      localStorage.removeItem("userId");
      localStorage.removeItem("userData");

      toast.success("Sesión cerrada correctamente");
    } catch (error: any) {
      // Aunque falle el logout en el servidor, limpiamos localmente
      Cookies.remove("accessToken");
      Cookies.remove("refreshToken");
      localStorage.removeItem("userId");
      localStorage.removeItem("userData");
      
      const errorMessage =
        error.response?.data?.message || "Error al cerrar sesión";
      toast.error(errorMessage);
      return rejectWithValue(errorMessage);
    }
  }
);

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    clearError: (state) => {
      state.error = null;
    },
    setUser: (state, action: PayloadAction<User>) => {
      state.user = action.payload;
      state.isAuthenticated = true;
    },
    clearAuth: (state) => {
      state.user = null;
      state.accessToken = null;
      state.refreshToken = null;
      state.expiresAt = null;
      state.isAuthenticated = false;
      state.error = null;
      // Limpiar cookies y localStorage
      Cookies.remove("accessToken");
      Cookies.remove("refreshToken");
      localStorage.removeItem("userId");
      localStorage.removeItem("userData");
    },
  },
  extraReducers: (builder) => {
    builder
      // Login
      .addCase(loginAsync.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(loginAsync.fulfilled, (state, action) => {
        state.loading = false;
        state.user = action.payload.user;
        state.accessToken = action.payload.accessToken;
        state.refreshToken = action.payload.refreshToken;
        state.expiresAt = action.payload.expiresAt;
        state.isAuthenticated = true;
        state.error = null;
      })
      .addCase(loginAsync.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
        state.isAuthenticated = false;
        state.user = null;
        state.accessToken = null;
        state.refreshToken = null;
        state.expiresAt = null;
      })
      // Register
      .addCase(registerAsync.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(registerAsync.fulfilled, (state, action) => {
        state.loading = false;
        state.user = action.payload.user;
        state.accessToken = action.payload.accessToken;
        state.refreshToken = action.payload.refreshToken;
        state.expiresAt = action.payload.expiresAt;
        state.isAuthenticated = true;
        state.error = null;
      })
      .addCase(registerAsync.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
        state.isAuthenticated = false;
        state.user = null;
        state.accessToken = null;
        state.refreshToken = null;
        state.expiresAt = null;
      })
      // Logout
      .addCase(logoutAsync.pending, (state) => {
        state.loading = true;
      })
      .addCase(logoutAsync.fulfilled, (state) => {
        state.loading = false;
        state.user = null;
        state.accessToken = null;
        state.refreshToken = null;
        state.expiresAt = null;
        state.isAuthenticated = false;
        state.error = null;
      })
      .addCase(logoutAsync.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const { clearError, setUser, clearAuth } = authSlice.actions;
export default authSlice.reducer;
