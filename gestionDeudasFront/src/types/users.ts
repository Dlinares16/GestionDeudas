export interface User {
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

export interface UsersState {
  users: User[];
  loading: boolean;
  error: string | null;
  operationLoading: {
    fetching: boolean;
  };
  operationErrors: {
    fetch: string | null;
  };
}
