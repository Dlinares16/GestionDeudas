import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { Deuda, DeudasState } from "../types/deudas";
import {
  fetchDeudas,
  fetchDeudaById,
  createDeuda,
  updateDeuda,
  deleteDeuda,
  type CreateDeudaRequest,
  type UpdateDeudaRequest,
} from "../actions/deudasActions";

const initialState: DeudasState = {
  deudas: [],
  loading: false,
  error: null,
  operationLoading: {
    fetching: false,
    creating: false,
    updating: false,
    deleting: false,
  },
  operationErrors: {
    fetch: null,
    create: null,
    update: null,
    delete: null,
  },
};

export const fetchDeudasAsync = createAsyncThunk(
  "deudas/fetchDeudas",
  async () => {
    return await fetchDeudas();
  }
);

export const fetchDeudaByIdAsync = createAsyncThunk(
  "deudas/fetchDeudaById",
  async (id: string) => {
    return await fetchDeudaById(id);
  }
);

export const createDeudaAsync = createAsyncThunk(
  "deudas/createDeuda",
  async (deuda: CreateDeudaRequest, { rejectWithValue }) => {
    try {
      console.log("Creating deuda with data:", deuda);
      const result = await createDeuda(deuda);
      console.log("Deuda created successfully:", result);
      return result;
    } catch (error: any) {
      console.error("Error creating deuda:", error);
      return rejectWithValue(
        error.response?.data?.message ||
          error.message ||
          "Error al crear la deuda"
      );
    }
  }
);

export const updateDeudaAsync = createAsyncThunk(
  "deudas/updateDeuda",
  async (
    { id, deuda }: { id: string; deuda: UpdateDeudaRequest },
    { rejectWithValue }
  ) => {
    try {
      console.log("Updating deuda with id:", id, "data:", deuda);
      const result = await updateDeuda(id, deuda);
      console.log("Deuda updated successfully:", result);
      return result;
    } catch (error: any) {
      console.error("Error updating deuda:", error);
      return rejectWithValue(
        error.response?.data?.message ||
          error.message ||
          "Error al actualizar la deuda"
      );
    }
  }
);

export const deleteDeudaAsync = createAsyncThunk(
  "deudas/deleteDeuda",
  async (id: string, { rejectWithValue }) => {
    try {
      console.log("Deleting deuda with id:", id);
      await deleteDeuda(id);
      console.log("Deuda deleted successfully");
      return id;
    } catch (error: any) {
      console.error("Error deleting deuda:", error);
      return rejectWithValue(
        error.response?.data?.message ||
          error.message ||
          "Error al eliminar la deuda"
      );
    }
  }
);

const deudasSlice = createSlice({
  name: "deudas",
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      // Fetch deudas
      .addCase(fetchDeudasAsync.pending, (state) => {
        state.loading = true;
        state.operationLoading.fetching = true;
        state.error = null;
        state.operationErrors.fetch = null;
      })
      .addCase(
        fetchDeudasAsync.fulfilled,
        (state, action: PayloadAction<Deuda[]>) => {
          state.loading = false;
          state.operationLoading.fetching = false;
          state.deudas = action.payload;
        }
      )
      .addCase(fetchDeudasAsync.rejected, (state, action) => {
        state.loading = false;
        state.operationLoading.fetching = false;
        state.error = action.error.message || "Error al cargar las deudas";
        state.operationErrors.fetch =
          action.error.message || "Error al cargar las deudas";
      })

      // Fetch deuda by ID
      .addCase(fetchDeudaByIdAsync.pending, (state) => {
        state.operationLoading.fetching = true;
        state.operationErrors.fetch = null;
      })
      .addCase(fetchDeudaByIdAsync.fulfilled, (state) => {
        state.operationLoading.fetching = false;
      })
      .addCase(fetchDeudaByIdAsync.rejected, (state, action) => {
        state.operationLoading.fetching = false;
        state.operationErrors.fetch =
          action.error.message || "Error al cargar la deuda";
      })

      // Create deuda
      .addCase(createDeudaAsync.pending, (state) => {
        state.operationLoading.creating = true;
        state.operationErrors.create = null;
      })
      .addCase(
        createDeudaAsync.fulfilled,
        (state, action: PayloadAction<Deuda>) => {
          state.operationLoading.creating = false;
          state.deudas.push(action.payload);
        }
      )
      .addCase(createDeudaAsync.rejected, (state, action) => {
        state.operationLoading.creating = false;
        state.operationErrors.create =
          (action.payload as string) ||
          action.error.message ||
          "Error al crear la deuda";
      })

      // Update deuda
      .addCase(updateDeudaAsync.pending, (state) => {
        state.operationLoading.updating = true;
        state.operationErrors.update = null;
      })
      .addCase(
        updateDeudaAsync.fulfilled,
        (state, action: PayloadAction<Deuda>) => {
          state.operationLoading.updating = false;
          const index = state.deudas.findIndex(
            (deuda) => deuda.debtId === action.payload.debtId
          );
          if (index !== -1) {
            state.deudas[index] = action.payload;
          }
        }
      )
      .addCase(updateDeudaAsync.rejected, (state, action) => {
        state.operationLoading.updating = false;
        state.operationErrors.update =
          (action.payload as string) ||
          action.error.message ||
          "Error al actualizar la deuda";
      })

      // Delete deuda
      .addCase(deleteDeudaAsync.pending, (state) => {
        state.operationLoading.deleting = true;
        state.operationErrors.delete = null;
      })
      .addCase(
        deleteDeudaAsync.fulfilled,
        (state, action: PayloadAction<string>) => {
          state.operationLoading.deleting = false;
          state.deudas = state.deudas.filter(
            (deuda) => deuda.debtId !== action.payload
          );
        }
      )
      .addCase(deleteDeudaAsync.rejected, (state, action) => {
        state.operationLoading.deleting = false;
        state.operationErrors.delete =
          (action.payload as string) ||
          action.error.message ||
          "Error al eliminar la deuda";
      });
  },
});

export default deudasSlice.reducer;
