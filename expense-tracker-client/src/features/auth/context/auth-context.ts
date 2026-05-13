import { createContext } from "react";
import type { LoginRequest, RegisterRequest, User } from "../types/auth.types";

export type AuthContextValue = {
  user: User | null;
  isAuthenticated: boolean;
  isInitializing: boolean;
  login: (request: LoginRequest) => Promise<void>;
  register: (request: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
  setUser: (user: User | null) => void;
  clearSession: () => void;
};

export const AuthContext = createContext<AuthContextValue | undefined>(
  undefined,
);
