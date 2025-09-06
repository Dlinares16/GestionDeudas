import React from "react";
import Header from "../components/Header";
import Aside from "../components/Aside";
import { useAuth } from "../hooks/useAuth";
import "../styles/Layout.css";

interface LayoutProps {
  children: React.ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <>{children}</>;
  }

  return (
    <div className="layout">
      <Header />
      <Aside />
      <main className="main-content">{children}</main>
    </div>
  );
};

export default Layout;
