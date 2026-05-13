import type { PropsWithChildren } from "react";

type AuthFormCardProps = PropsWithChildren<{
  title: string;
  subtitle: string;
}>;

export function AuthFormCard({ title, subtitle, children }: AuthFormCardProps) {
  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-100 px-4">
      <div className="w-full max-w-md rounded-2xl border bg-white p-8 shadow-sm">
        <div className="mb-6">
          <h1 className="text-2xl font-semibold text-gray-900">{title}</h1>
          <p className="mt-2 text-sm text-gray-600">{subtitle}</p>
        </div>

        {children}
      </div>
    </div>
  );
}
