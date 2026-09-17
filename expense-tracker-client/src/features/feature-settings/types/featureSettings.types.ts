export type FeatureSettings = {
  features: Record<string, boolean>;
};

export const FeatureKeys = {
  expenseExport: "expenseExport",
} as const;

export type FeatureKey = keyof typeof FeatureKeys;
