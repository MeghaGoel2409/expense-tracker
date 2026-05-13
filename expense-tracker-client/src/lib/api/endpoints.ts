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
  categories: {
    base: "/categories",
  },
} as const;
