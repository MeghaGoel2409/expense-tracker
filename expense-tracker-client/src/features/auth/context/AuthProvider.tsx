import {
  useCallback,
  useEffect,
  useMemo,
  useState,
  type PropsWithChildren,
} from "react";
import { authApi } from "../api/authApi";
import type {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  User,
} from "../types/auth.types";
import { tokenStore } from "../utils/tokenStore";
import { AuthContext, type AuthContextValue } from "./auth-context";
import {
  registerAuthFailureHandler,
  resetSessionExpiredNotification,
  unregisterAuthFailureHandler,
} from "@/lib/api/refreshManager";

export function AuthProvider({ children }: PropsWithChildren) {
  const [user, setUser] = useState<User | null>(null);
  const [isInitializing, setIsInitializing] = useState(true);

  const applyAuthenticatedSession = useCallback(
    (data: AuthResponse["data"]) => {
      if (!data?.accessToken || !data.user) {
        throw new Error("Invalid authentication response.");
      }

      tokenStore.setAccessToken(data.accessToken);
      tokenStore.setSessionHint();
      resetSessionExpiredNotification();
      setUser(data.user);
    },
    [],
  );

  const clearSession = useCallback(() => {
    tokenStore.clear();
    setUser(null);
  }, []);

  const register = useCallback(
    async (request: RegisterRequest) => {
      const result = await authApi.register(request);

      if (!result.isSuccess) {
        throw new Error(result.errors?.[0]?.message ?? "Registration failed.");
      }

      applyAuthenticatedSession(result.data);
    },
    [applyAuthenticatedSession],
  );

  const login = useCallback(
    async (request: LoginRequest) => {
      const result = await authApi.login(request);

      if (!result.isSuccess) {
        throw new Error(result.errors?.[0]?.message ?? "Login failed.");
      }

      applyAuthenticatedSession(result.data);
    },
    [applyAuthenticatedSession],
  );

  const logout = useCallback(async () => {
    try {
      await authApi.logout();
    } finally {
      clearSession();
    }
  }, [clearSession]);

  useEffect(() => {
    registerAuthFailureHandler(clearSession);

    return () => {
      unregisterAuthFailureHandler();
    };
  }, [clearSession]);

  useEffect(() => {
    const initializeAuth = async () => {
      if (!tokenStore.hasSessionHint()) {
        setIsInitializing(false);
        return;
      }

      try {
        const result = await authApi.refresh();

        if (!result.isSuccess) {
          clearSession();
          return;
        }

        applyAuthenticatedSession(result.data);
      } catch {
        clearSession();
      } finally {
        setIsInitializing(false);
      }
    };

    void initializeAuth();
  }, [applyAuthenticatedSession, clearSession]);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isAuthenticated: !!user,
      isInitializing,
      login,
      register,
      logout,
      setUser,
      clearSession,
    }),
    [user, isInitializing, login, register, logout, clearSession],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
