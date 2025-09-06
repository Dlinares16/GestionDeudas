import React from "react";
import Table from "../components/Table";
import { useAuth } from "../hooks/useAuth";
import {
  HomeHeader,
  StatsGrid,
  Filters,
  CreateDeudaModal,
  DeleteConfirmationModal,
  useHome,
} from "./home/index";
import "../styles/Home.css";

const Home: React.FC = () => {
  const { user } = useAuth();
  const {
    // Estados de datos
    filteredDeudas,
    stats,
    users,
    totalDeudas,
    
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
  } = useHome();

  return (
    <div className="page-container">
      <HomeHeader
        userName={user?.fullName}
        onCreateClick={() => setShowCreateModal(true)}
      />

      <StatsGrid
        paid={stats.paid}
        pending={stats.pending}
        overdue={stats.overdue}
        totalOwed={stats.totalOwed}
      />

      <Filters
        searchTerm={searchTerm}
        statusFilter={statusFilter}
        filteredCount={filteredDeudas.length}
        totalCount={totalDeudas}
        onSearchChange={setSearchTerm}
        onStatusFilterChange={setStatusFilter}
        onClearFilters={clearFilters}
      />

      <div className="table-section">
        <Table
          deudas={filteredDeudas}
          onEdit={handleEdit}
          onView={handleView}
          onDelete={(id) => setShowDeleteModal(id)}
          loading={loading}
          error={error}
        />
      </div>

      <CreateDeudaModal
        isOpen={showCreateModal}
        createForm={createForm}
        users={users}
        usersLoading={usersLoading}
        isCreating={isCreating}
        onClose={() => setShowCreateModal(false)}
        onSubmit={handleCreateDeuda}
        onFormChange={updateCreateForm}
      />

      <DeleteConfirmationModal
        isOpen={!!showDeleteModal}
        isDeleting={isDeleting}
        onClose={() => setShowDeleteModal(null)}
        onConfirm={async () => {
          if (showDeleteModal) {
            await handleDelete(showDeleteModal);
          }
        }}
      />
    </div>
  );
};

export default Home;
