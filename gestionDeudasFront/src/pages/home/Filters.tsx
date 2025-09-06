import React from "react";
import type { StatusFilter } from "./homeTypes";

interface FiltersProps {
  searchTerm: string;
  statusFilter: StatusFilter;
  filteredCount: number;
  totalCount: number;
  onSearchChange: (term: string) => void;
  onStatusFilterChange: (filter: StatusFilter) => void;
  onClearFilters: () => void;
}

const Filters: React.FC<FiltersProps> = ({
  searchTerm,
  statusFilter,
  filteredCount,
  totalCount,
  onSearchChange,
  onStatusFilterChange,
  onClearFilters,
}) => {
  const hasActiveFilters = searchTerm || statusFilter !== "all";

  return (
    <div className="filters-section">
      <div className="filters-header">
        <h2 className="section-title">Mis Deudas</h2>
        <div className="filters-controls">
          <div className="search-box">
            <svg
              width="20"
              height="20"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              <circle cx="11" cy="11" r="8" />
              <path d="M21 21l-4.35-4.35" />
            </svg>
            <input
              type="text"
              placeholder="Buscar por descripción o nombre..."
              value={searchTerm}
              onChange={(e) => onSearchChange(e.target.value)}
              className="search-input"
            />
          </div>
          <select
            value={statusFilter}
            onChange={(e) => onStatusFilterChange(e.target.value as StatusFilter)}
            className="filter-select"
          >
            <option value="all">Todos los estados</option>
            <option value="pending">Pendientes</option>
            <option value="paid">Pagadas</option>
            <option value="overdue">Vencidas</option>
          </select>
          {hasActiveFilters && (
            <button onClick={onClearFilters} className="clear-filters-button">
              <svg
                width="16"
                height="16"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
              >
                <line x1="18" y1="6" x2="6" y2="18" />
                <line x1="6" y1="6" x2="18" y2="18" />
              </svg>
              Limpiar
            </button>
          )}
        </div>
      </div>

      {hasActiveFilters && (
        <div className="filters-summary">
          Mostrando {filteredCount} de {totalCount} deudas
          {searchTerm && <span> que contienen "{searchTerm}"</span>}
          {statusFilter !== "all" && (
            <span> con estado "{statusFilter}"</span>
          )}
        </div>
      )}
    </div>
  );
};

export default Filters;
