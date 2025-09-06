import React from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { logoutAsync } from "../slices/authSlice";
import { useDispatch, useSelector } from "react-redux";
import type { AppDispatch, RootState } from "../store/store";
import "../styles/Header.css";

const Header: React.FC = () => {
  const { user, isAuthenticated } = useAuth();
  const { loading } = useSelector((state: RootState) => state.auth);
  const dispatch = useDispatch<AppDispatch>();
  const navigate = useNavigate();

  const handleLogout = async () => {
    const result = await dispatch(logoutAsync());
    
    // Redirigir al login después del logout (exitoso o no)
    if (logoutAsync.fulfilled.match(result) || logoutAsync.rejected.match(result)) {
      navigate("/login");
    }
  };

  if (!isAuthenticated || !user) {
    return null;
  }

  return (
    <header className="header">
      <div className="header-container">
        <div className="header-left">
          <h1 className="header-title">Gestión de Deudas</h1>
        </div>
        <div className="header-right">
          <div className="user-info">
            <div className="user-avatar">
              {user.firstName.charAt(0).toUpperCase()}
              {user.lastName.charAt(0).toUpperCase()}
            </div>
            <div className="user-details">
              <span className="user-name">{user.fullName}</span>
              <span className="user-email">{user.email}</span>
            </div>
          </div>
          <button
            className="logout-button"
            onClick={handleLogout}
            disabled={loading}
            title="Cerrar sesión"
          >
            {loading ? (
              <div className="loading-spinner-small"></div>
            ) : (
              <svg
                width="20"
                height="20"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
              >
                <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
                <polyline points="16,17 21,12 16,7" />
                <line x1="21" y1="12" x2="9" y2="12" />
              </svg>
            )}
          </button>
        </div>
      </div>
    </header>
  );
};

export default Header;
