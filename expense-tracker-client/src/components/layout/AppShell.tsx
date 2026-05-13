import { Outlet, Link } from "react-router-dom";
import { useAuth } from "@/features/auth/hooks/useAuth";

export function AppShell() {
  const { user, logout } = useAuth();

  return (
    <div className="flex min-h-screen bg-gray-100">
      {/* Sidebar */}
      <aside className="w-64 bg-white border-r">
        <div className="p-4 text-lg font-semibold border-b">
          Expense Tracker
        </div>

        <nav className="p-4 space-y-2">
          <Link to="/" className="block rounded-md px-3 py-2 hover:bg-gray-100">
            Dashboard
          </Link>

          <Link
            to="/expenses"
            className="block rounded-md px-3 py-2 hover:bg-gray-100"
          >
            Expenses
          </Link>

          <Link
            to="/categories"
            className="block rounded-md px-3 py-2 hover:bg-gray-100"
          >
            Categories
          </Link>
        </nav>
      </aside>

      {/* Main area */}
      <div className="flex flex-1 flex-col">
        {/* Header */}
        <header className="flex items-center justify-between bg-white px-6 py-3 border-b">
          <div className="text-lg font-medium">
            Welcome {user?.firstName ?? "User"}
          </div>

          <div className="flex items-center gap-4">
            <span className="text-sm text-gray-600">{user?.email}</span>

            <button
              onClick={logout}
              className="rounded-md bg-black px-3 py-1.5 text-sm text-white hover:opacity-90"
            >
              Logout
            </button>
          </div>
        </header>

        {/* Page content */}
        <main className="flex-1 p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
