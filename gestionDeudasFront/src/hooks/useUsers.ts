import { useSelector } from "react-redux";
import type { RootState } from "../store/store";

export const useUsers = () => {
  const {
    users,
    loading,
    error,
    operationLoading,
    operationErrors,
  } = useSelector((state: RootState) => state.users);

  return {
    users,
    loading,
    error,
    isFetching: operationLoading.fetching,
    fetchError: operationErrors.fetch,
  };
};
