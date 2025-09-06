import { useEffect, useState, useMemo } from "react";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import toast from "react-hot-toast";
import type { AppDispatch } from "../../store/store";
import {
  fetchDeudasAsync,
  deleteDeudaAsync,
  createDeudaAsync,
} from "../../slices/deudasSlice";
import { fetchUsersAsync } from "../../slices/usersSlice";
import type { CreateDeudaRequest } from "../../actions/deudasActions";
import { useDeudas } from "../../hooks/useDeudas";
import { useUsers } from "../../hooks/useUsers";
import type { 
  StatusFilter, 
  CreateDeudaForm, 
  DeudaStats, 
  UseHomeReturn 
} from "./homeTypes";

export const useHome = (): UseHomeReturn => {
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();
  
  // Hooks existentes
  const {
    deudas,
    loading,
    error,
    isCreating,
    isDeleting,
    createError,
    deleteError,
  } = useDeudas();
  
  const { users, isFetching: usersLoading } = useUsers();

  // Estados para filtros y búsqueda
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState<StatusFilter>("all");
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState<string | null>(null);

  // Estado para el formulario de creación
  const [createForm, setCreateForm] = useState<CreateDeudaForm>({
    debtorId: "",
    amount: "",
    description: "",
    dueDate: new Date().toISOString().split("T")[0],
  });

  // Inicialización de datos
  useEffect(() => {
    dispatch(fetchDeudasAsync());
    dispatch(fetchUsersAsync());
  }, [dispatch]);

  // Estadísticas calculadas
  const stats = useMemo((): DeudaStats => {
    const paid = deudas.filter((d) => d.status === "paid").length;
    const pending = deudas.filter((d) => d.status === "pending").length;
    const overdue = deudas.filter((d) => d.status === "overdue").length;
    const totalOwed = deudas
      .filter((d) => d.status !== "paid")
      .reduce((sum, d) => sum + d.remainingAmount, 0);

    return { paid, pending, overdue, totalOwed };
  }, [deudas]);

  // Filtrar deudas basado en búsqueda y filtros
  const filteredDeudas = useMemo(() => {
    let filtered = deudas;

    // Filtrar por estado
    if (statusFilter !== "all") {
      filtered = filtered.filter((deuda) => deuda.status === statusFilter);
    }

    // Filtrar por término de búsqueda
    if (searchTerm) {
      const term = searchTerm.toLowerCase();
      filtered = filtered.filter(
        (deuda) =>
          deuda.description.toLowerCase().includes(term) ||
          deuda.debtor.fullName.toLowerCase().includes(term) ||
          deuda.creditor.fullName.toLowerCase().includes(term)
      );
    }

    return filtered;
  }, [deudas, statusFilter, searchTerm]);

  // Funciones de utilidad para el formulario
  const updateCreateForm = (field: keyof CreateDeudaForm, value: string | number) => {
    setCreateForm(prev => ({
      ...prev,
      [field]: value
    }));
  };

  const resetCreateForm = () => {
    setCreateForm({
      debtorId: "",
      amount: "",
      description: "",
      dueDate: new Date().toISOString().split("T")[0],
    });
  };

  // Handlers de navegación
  const handleEdit = (deuda: any) => {
    navigate(`/detalle/${deuda.debtId}`);
  };

  const handleView = (deuda: any) => {
    navigate(`/detalle/${deuda.debtId}`);
  };

  // Handler para eliminar deuda
  const handleDelete = async (id: string) => {
    try {
      const resultAction = await dispatch(deleteDeudaAsync(id));
      if (deleteDeudaAsync.fulfilled.match(resultAction)) {
        toast.success("Deuda eliminada exitosamente");
        setShowDeleteModal(null);
      } else {
        toast.error(deleteError || "Error al eliminar la deuda");
      }
    } catch (error) {
      toast.error("Error al eliminar la deuda");
    }
  };

  // Handler para crear nueva deuda
  const handleCreateDeuda = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validación más completa
    if (!createForm.debtorId) {
      toast.error("Por favor selecciona un deudor");
      return;
    }

    if (!createForm.description.trim()) {
      toast.error("Por favor ingresa una descripción");
      return;
    }

    const amount = Number(createForm.amount);
    if (isNaN(amount) || amount <= 0) {
      toast.error("El monto debe ser un número mayor a 0");
      return;
    }

    if (!createForm.dueDate) {
      toast.error("Por favor selecciona una fecha límite");
      return;
    }

    const deudaData: CreateDeudaRequest = {
      debtorId: createForm.debtorId,
      amount: amount,
      description: createForm.description.trim(),
      dueDate: createForm.dueDate,
    };

    console.log("Enviando datos de deuda:", deudaData);

    try {
      const resultAction = await dispatch(createDeudaAsync(deudaData));
      if (createDeudaAsync.fulfilled.match(resultAction)) {
        toast.success("Deuda creada exitosamente");
        setShowCreateModal(false);
        resetCreateForm();
      } else {
        console.error("Error en createDeudaAsync:", resultAction);
        const errorMessage =
          createError ||
          (resultAction.payload as string) ||
          "Error al crear la deuda";
        toast.error(errorMessage);
      }
    } catch (error) {
      console.error("Error catch en handleCreateDeuda:", error);
      toast.error("Error inesperado al crear la deuda");
    }
  };

  // Limpiar filtros
  const clearFilters = () => {
    setSearchTerm("");
    setStatusFilter("all");
  };

  return {
    // Estados de datos
    filteredDeudas,
    stats,
    users,
    totalDeudas: deudas.length,
    
    // Estados de UI
    searchTerm,
    statusFilter,
    showCreateModal,
    showDeleteModal,
    createForm,
    
    // Estados de carga
    loading,
    usersLoading,
    isCreating,
    isDeleting,
    
    // Errores
    error,
    
    // Funciones de manejo
    setSearchTerm,
    setStatusFilter,
    setShowCreateModal,
    setShowDeleteModal,
    updateCreateForm,
    
    // Acciones
    handleEdit,
    handleView,
    handleDelete,
    handleCreateDeuda,
    clearFilters,
    resetCreateForm,
  };
};
