import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { User, UsersState } from "../types/users";
import { fetchUsers, fetchUserById } from "../actions/usersActions";

const initialState: UsersState = {
  users: [],
  loading: false,
  error: null,
  operationLoading: {
    fetching: false,
  },
  operationErrors: {
    fetch: null,
  },
};

export const fetchUsersAsync = createAsyncThunk(
  "users/fetchUsers",
  async () => {
    return await fetchUsers();
  }
);

export const fetchUserByIdAsync = createAsyncThunk(
  "users/fetchUserById",
  async (id: string) => {
    return await fetchUserById(id);
  }
);

const usersSlice = createSlice({
  name: "users",
  initialState,
  reducers: {
    clearUsersError: (state) => {
      state.error = null;
      state.operationErrors.fetch = null;
    },
  },
  extraReducers: (builder) => {
    builder
      // Fetch users
      .addCase(fetchUsersAsync.pending, (state) => {
        state.loading = true;
        state.operationLoading.fetching = true;
        state.error = null;
        state.operationErrors.fetch = null;
      })
      .addCase(
        fetchUsersAsync.fulfilled,
        (state, action: PayloadAction<User[]>) => {
          state.loading = false;
          state.operationLoading.fetching = false;
          state.users = action.payload;
        }
      )
      .addCase(fetchUsersAsync.rejected, (state, action) => {
        state.loading = false;
        state.operationLoading.fetching = false;
        state.error = action.error.message || "Error al cargar los usuarios";
        state.operationErrors.fetch =
          action.error.message || "Error al cargar los usuarios";
      })

      // Fetch user by ID
      .addCase(fetchUserByIdAsync.pending, (state) => {
        state.operationLoading.fetching = true;
        state.operationErrors.fetch = null;
      })
      .addCase(fetchUserByIdAsync.fulfilled, (state) => {
        state.operationLoading.fetching = false;
      })
      .addCase(fetchUserByIdAsync.rejected, (state, action) => {
        state.operationLoading.fetching = false;
        state.operationErrors.fetch =
          action.error.message || "Error al cargar el usuario";
      });
  },
});

export const { clearUsersError } = usersSlice.actions;
export default usersSlice.reducer;
