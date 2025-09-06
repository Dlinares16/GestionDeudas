import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import toast from "react-hot-toast";
import type { AppDispatch } from "../../store/store";
import { updateDeudaAsync, deleteDeudaAsync } from "../../slices/deudasSlice";
import type { Deuda } from "../../types/deudas";
import { appclient } from "../../appClient/appclient";
import { useDeudas } from "../../hooks/useDeudas";
import type { DeudaFormData, UseDetalleReturn } from "./detalleTypes";

export const useDetalle = (): UseDetalleReturn => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const dispatch = useDispatch<AppDispatch>();
  const { isUpdating, isDeleting, updateError, deleteError } = useDeudas();

  const [deuda, setDeuda] = useState<Deuda | null>(null);
  const [loading, setLoading] = useState(true);
  const [editing, setEditing] = useState(false);
  const [showDeleteModal, setShowDeleteModal] = useState(false);

  const [formData, setFormData] = useState<DeudaFormData>({
    amount: 0,
    description: "",
    dueDate: "",
    status: "pending",
  });

  // Cargar datos de la deuda
  useEffect(() => {
    const fetchDeuda = async () => {
      if (!id) return;

      try {
        setLoading(true);
        const response = await appclient.get<Deuda>(`/Debts/${id}`);
        setDeuda(response.data);
        setFormData({
          amount: response.data.amount,
          description: response.data.description,
          dueDate: response.data.dueDate,
          status: response.data.status,
        });
      } catch (error: any) {
        toast.error("Error al cargar los detalles de la deuda");
        console.error("Error fetching debt details:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchDeuda();
  }, [id]);

  // Actualizar campo del formulario
  const updateFormData = (field: keyof DeudaFormData, value: any) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
  };

  // Guardar cambios
  const handleSave = async () => {
    if (!deuda || !id) return;

    try {
      const updateData = {
        amount: formData.amount,
        description: formData.description,
        dueDate: formData.dueDate,
        status: formData.status,
      };

      const resultAction = await dispatch(
        updateDeudaAsync({ id, deuda: updateData })
      );
      
      if (updateDeudaAsync.fulfilled.match(resultAction)) {
        setDeuda(resultAction.payload);
        setEditing(false);
        toast.success("Deuda actualizada exitosamente");
      } else {
        toast.error(updateError || "Error al actualizar la deuda");
      }
    } catch (error) {
      toast.error("Error al actualizar la deuda");
    }
  };

  // Eliminar deuda
  const handleDelete = async () => {
    if (!id) return;

    try {
      const resultAction = await dispatch(deleteDeudaAsync(id));
      if (deleteDeudaAsync.fulfilled.match(resultAction)) {
        toast.success("Deuda eliminada exitosamente");
        navigate("/home");
      } else {
        toast.error(deleteError || "Error al eliminar la deuda");
      }
    } catch (error) {
      toast.error("Error al eliminar la deuda");
    }
  };

  // Navegar hacia atrás
  const handleBack = () => {
    navigate("/home");
  };

  // Formatear dinero
  const formatMoney = (amount: number) => {
    return new Intl.NumberFormat("es-ES", {
      style: "currency",
      currency: "USD",
    }).format(amount);
  };

  // Formatear fecha
  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString("es-ES", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  };

  return {
    // Estados de datos
    deuda,
    formData,
    
    // Estados de UI
    loading,
    editing,
    showDeleteModal,
    
    // Estados de operaciones
    isUpdating,
    isDeleting,
    
    // Errores
    updateError,
    deleteError,
    
    // Funciones de manejo
    setEditing,
    setShowDeleteModal,
    updateFormData,
    
    // Acciones
    handleSave,
    handleDelete,
    handleBack,
    
    // Funciones de utilidad
    formatMoney,
    formatDate,
  };
};
