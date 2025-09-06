import { appclient } from "../appClient/appclient";
import type { Deuda } from "../types/deudas";

// Interfaces específicas para las operaciones
export interface CreateDeudaRequest {
  debtorId: string;
  amount: number;
  description: string;
  dueDate: string;
}

export interface UpdateDeudaRequest {
  amount: number;
  description: string;
  dueDate: string;
  status: "pending" | "paid" | "overdue";
}

export const fetchDeudas = async () => {
  const response = await appclient.get<Deuda[]>("/Debts");
  return response.data;
};

export const fetchDeudaById = async (id: string) => {
  const response = await appclient.get<Deuda>(`/Debts/${id}`);
  return response.data;
};

export const createDeuda = async (deuda: CreateDeudaRequest) => {
  const response = await appclient.post<Deuda>("/Debts", deuda);
  return response.data;
};

export const updateDeuda = async (id: string, deuda: UpdateDeudaRequest) => {
  const response = await appclient.put<Deuda>(`/Debts/${id}`, deuda);
  return response.data;
};

export const deleteDeuda = async (id: string) => {
  await appclient.delete(`/Debts/${id}`);
  return id;
};
