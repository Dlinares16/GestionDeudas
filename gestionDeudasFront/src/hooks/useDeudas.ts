import { useSelector } from "react-redux";
import type { RootState } from "../store/store";

export const useDeudas = () => {
  const deudas = useSelector((state: RootState) => state.deudas.deudas);
  const loading = useSelector((state: RootState) => state.deudas.loading);
  const error = useSelector((state: RootState) => state.deudas.error);
  const operationLoading = useSelector(
    (state: RootState) => state.deudas.operationLoading
  );
  const operationErrors = useSelector(
    (state: RootState) => state.deudas.operationErrors
  );

  return {
    deudas,
    loading,
    error,
    // Estados específicos de operaciones
    isCreating: operationLoading.creating,
    isUpdating: operationLoading.updating,
    isDeleting: operationLoading.deleting,
    isFetching: operationLoading.fetching,
    // Errores específicos
    createError: operationErrors.create,
    updateError: operationErrors.update,
    deleteError: operationErrors.delete,
    fetchError: operationErrors.fetch,
    // Helpers
    isAnyOperationLoading: Object.values(operationLoading).some(
      (loading) => loading
    ),
    hasAnyError: Object.values(operationErrors).some((error) => error !== null),
  };
};
