type Estado = "pending" | "paid" | "overdue";

interface User {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  phone: string | null;
  isActive: boolean;
  emailVerified: boolean;
  createdAt: string;
  updatedAt: string;
  fullName: string;
}

interface Payment {
  // Estructura de pagos si es necesaria
  paymentId?: string;
  amount?: number;
  date?: string;
}

interface Deuda {
  debtId: string;
  creditorId: string;
  debtorId: string;
  amount: number;
  description: string;
  status: Estado;
  dueDate: string;
  createdAt: string;
  updatedAt: string;
  creditor: User;
  debtor: User;
  payments: Payment[];
  totalPaid: number;
  remainingAmount: number;
  isOverdue: boolean;
}

interface DeudasState {
  deudas: Deuda[];
  loading: boolean;
  error: string | null;
  operationLoading: {
    fetching: boolean;
    creating: boolean;
    updating: boolean;
    deleting: boolean;
  };
  operationErrors: {
    fetch: string | null;
    create: string | null;
    update: string | null;
    delete: string | null;
  };
}

export type { Deuda, DeudasState, Estado, User, Payment };
