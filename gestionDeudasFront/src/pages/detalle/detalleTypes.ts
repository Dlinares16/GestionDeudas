import type { Deuda } from "../../types/deudas";

export interface DeudaFormData {
  amount: number;
  description: string;
  dueDate: string;
  status: "pending" | "paid" | "overdue";
}

export interface PersonInfo {
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone: string | null;
}

export interface UseDetalleReturn {
  // Estados de datos
  deuda: Deuda | null;
  formData: DeudaFormData;
  
  // Estados de UI
  loading: boolean;
  editing: boolean;
  showDeleteModal: boolean;
  
  // Estados de operaciones
  isUpdating: boolean;
  isDeleting: boolean;
  
  // Errores
  updateError: string | null;
  deleteError: string | null;
  
  // Funciones de manejo
  setEditing: (editing: boolean) => void;
  setShowDeleteModal: (show: boolean) => void;
  updateFormData: (field: keyof DeudaFormData, value: any) => void;
  
  // Acciones
  handleSave: () => Promise<void>;
  handleDelete: () => Promise<void>;
  handleBack: () => void;
  
  // Funciones de utilidad
  formatMoney: (amount: number) => string;
  formatDate: (dateString: string) => string;
}
