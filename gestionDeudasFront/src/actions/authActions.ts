import { appclient } from "../appClient/appclient";
import type { LoginCredentials, RegisterCredentials, AuthResponse } from "../types/auth";

export const login = async (
  credentials: LoginCredentials
): Promise<AuthResponse> => {
  const response = await appclient.post("/Auth/login", credentials);
  return response.data;
};

export const register = async (
  credentials: RegisterCredentials
): Promise<AuthResponse> => {
  const response = await appclient.post("/Auth/register", credentials);
  return response.data;
};

export const logout = async (refreshToken: string): Promise<void> => {
  await appclient.post("/Auth/logout", { refreshToken });
};
