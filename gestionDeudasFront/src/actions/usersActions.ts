import { appclient } from "../appClient/appclient";
import type { User } from "../types/users";

export const fetchUsers = async (): Promise<User[]> => {
  const response = await appclient.get<User[]>("/Users");
  return response.data;
};

export const fetchUserById = async (id: string): Promise<User> => {
  const response = await appclient.get<User>(`/Users/${id}`);
  return response.data;
};
