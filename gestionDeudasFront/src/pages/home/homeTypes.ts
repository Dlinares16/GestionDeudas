export type StatusFilter = "all" | "pending" | "paid" | "overdue";

export interface CreateDeudaForm {
  debtorId: string;
  amount: string | number;
  description: string;
  dueDate: string;
}

export interface DeudaStats {
  paid: number;
  pending: number;
  overdue: number;
  totalOwed: number;
}

export interface UseHomeReturn {
  // Estados de datos
  filteredDeudas: any[];
  stats: DeudaStats;
  users: any[];
  totalDeudas: number;
  
  // Estados de UI
  searchTerm: string;
  statusFilter: StatusFilter;
  showCreateModal: boolean;
  showDeleteModal: string | null;
  createForm: CreateDeudaForm;
  
  // Estados de carga
  loading: boolean;
  usersLoading: boolean;
  isCreating: boolean;
  isDeleting: boolean;
  
  // Errores
  error: string | null;
  
  // Funciones de manejo
  setSearchTerm: (term: string) => void;
  setStatusFilter: (filter: StatusFilter) => void;
  setShowCreateModal: (show: boolean) => void;
  setShowDeleteModal: (id: string | null) => void;
  updateCreateForm: (field: keyof CreateDeudaForm, value: string | number) => void;
  
  // Acciones
  handleEdit: (deuda: any) => void;
  handleView: (deuda: any) => void;
  handleDelete: (id: string) => Promise<void>;
  handleCreateDeuda: (e: React.FormEvent) => Promise<void>;
  clearFilters: () => void;
  resetCreateForm: () => void;
}
