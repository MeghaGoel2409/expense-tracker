import { Routes, Route } from "react-router-dom";
import { ProtectedRoute } from "././ProtectedRoute";
import { PublicOnlyRoute } from "./PublicOnlyRoute";

import { LoginPage } from "@/features/auth/pages/LoginPage";
import { RegisterPage } from "@/features/auth/pages/RegisterPage";

import { DashboardPage } from "@/features/dashboard/pages/DashboardPage";
import { AppShell } from "@/components/layout/AppShell";
import { CreateExpensePage } from "@/features/expenses/pages/CreateExpensePage";
import { ExpensesPage } from "@/features/expenses/pages/ExpensesPage";
import { EditExpensePage } from "@/features/expenses/pages/EditExpensePage";

export function AppRouter() {
  return (
    <Routes>
      {/* Public routes (only when NOT logged in) */}
      <Route element={<PublicOnlyRoute />}>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
      </Route>

      {/* Protected routes */}
      <Route element={<ProtectedRoute />}>
        <Route element={<AppShell />}>
          <Route path="/" element={<DashboardPage />} />
          <Route path="expenses" element={<ExpensesPage />} />
          <Route path="expenses/new" element={<CreateExpensePage />} />
          <Route path="expenses/:id/edit" element={<EditExpensePage />} />
        </Route>
      </Route>
    </Routes>
  );
}
