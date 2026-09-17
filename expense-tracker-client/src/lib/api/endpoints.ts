export const endpoints = {
  auth: {
    login: "/auth/login",
    register: "/auth/register",
    refresh: "/auth/refresh",
    logout: "/auth/logout",
    me: "/auth/me",
  },
  expenses: {
    base: "/expenses",
    byId: (id: number) => `/expenses/${id}`,
  },
  expenseExports: {
    base: "/expense-exports",
    byId: (exportJobId: number) => `/expense-exports/${exportJobId}`,
    download: (exportJobId: number) =>
      `/expense-exports/${exportJobId}/download`,
  },
  categories: {
    base: "/categories",
  },
  featureSettings: {
    base: "/feature-settings",
  },
} as const;
