import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useLogin } from "../hooks/useLogin";
import { loginSchema, type LoginFormValues } from "../schemas/authSchemas";
import { mapLoginToRequest } from "../mappers/authMappers";
import { AuthFormCard } from "../components/AuthFormCard";
import { AuthTextField } from "../components/AuthTextField";
import { AuthSubmitButton } from "../components/AuthSubmitButton";
import { AuthFormFooter } from "../components/AuthFormFooter";
import { AuthErrorBanner } from "../components/AuthErrorBanner";

export function LoginPage() {
  const loginMutation = useLogin();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: "",
      password: "",
    },
    mode: "onSubmit",
  });

  const onSubmit = (values: LoginFormValues) => {
    loginMutation.mutate(mapLoginToRequest(values));
  };

  return (
    <AuthFormCard title="Sign in" subtitle="Welcome back to Expense Tracker.">
      <form onSubmit={handleSubmit(onSubmit)} noValidate className="space-y-4">
        <AuthTextField
          id="email"
          label="Email"
          type="email"
          autoComplete="email"
          placeholder="you@example.com"
          disabled={loginMutation.isPending}
          error={errors.email?.message}
          {...register("email")}
        />

        <AuthTextField
          id="password"
          label="Password"
          type="password"
          autoComplete="current-password"
          placeholder="Enter your password"
          disabled={loginMutation.isPending}
          error={errors.password?.message}
          {...register("password")}
        />

        {loginMutation.isError && (
          <AuthErrorBanner message={loginMutation.error.message} />
        )}

        <AuthSubmitButton
          isLoading={loginMutation.isPending}
          idleText="Sign in"
          loadingText="Signing in..."
        />
      </form>

      <AuthFormFooter
        text="Don't have an account?"
        linkText="Create one"
        to="/register"
      />
    </AuthFormCard>
  );
}
