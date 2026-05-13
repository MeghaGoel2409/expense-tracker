import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useRegister } from "../hooks/useRegister";
import {
  registerSchema,
  type RegisterFormValues,
} from "../schemas/authSchemas";
import { mapRegisterToRequest } from "../mappers/authMappers";
import { AuthFormCard } from "../components/AuthFormCard";
import { AuthTextField } from "../components/AuthTextField";
import { AuthSubmitButton } from "../components/AuthSubmitButton";
import { AuthFormFooter } from "../components/AuthFormFooter";
import { AuthErrorBanner } from "../components/AuthErrorBanner";

export function RegisterPage() {
  const registerMutation = useRegister();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
    mode: "onSubmit",
  });

  const onSubmit = (values: RegisterFormValues) => {
    registerMutation.mutate(mapRegisterToRequest(values));
  };

  return (
    <AuthFormCard
      title="Create account"
      subtitle="Start tracking your expenses with a secure account."
    >
      <form onSubmit={handleSubmit(onSubmit)} noValidate className="space-y-4">
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <AuthTextField
            id="firstName"
            label="First name"
            type="text"
            autoComplete="given-name"
            disabled={registerMutation.isPending}
            error={errors.firstName?.message}
            {...register("firstName")}
          />

          <AuthTextField
            id="lastName"
            label="Last name"
            type="text"
            autoComplete="family-name"
            disabled={registerMutation.isPending}
            error={errors.lastName?.message}
            {...register("lastName")}
          />
        </div>

        <AuthTextField
          id="email"
          label="Email"
          type="email"
          autoComplete="email"
          placeholder="you@example.com"
          disabled={registerMutation.isPending}
          error={errors.email?.message}
          {...register("email")}
        />

        <AuthTextField
          id="password"
          label="Password"
          type="password"
          autoComplete="new-password"
          disabled={registerMutation.isPending}
          error={errors.password?.message}
          {...register("password")}
        />

        <AuthTextField
          id="confirmPassword"
          label="Confirm password"
          type="password"
          autoComplete="new-password"
          disabled={registerMutation.isPending}
          error={errors.confirmPassword?.message}
          {...register("confirmPassword")}
        />

        {registerMutation.isError && (
          <AuthErrorBanner message={registerMutation.error.message} />
        )}

        <AuthSubmitButton
          isLoading={registerMutation.isPending}
          idleText="Create account"
          loadingText="Creating account..."
        />
      </form>

      <AuthFormFooter
        text="Already have an account?"
        linkText="Sign in"
        to="/login"
      />
    </AuthFormCard>
  );
}
