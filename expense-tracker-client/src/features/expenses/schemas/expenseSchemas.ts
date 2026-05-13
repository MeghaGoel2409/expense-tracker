import { z } from "zod";

export const expenseSchema = z.object({
  amount: z.coerce.number().positive("Amount must be greater than 0."),
  expenseDate: z.string().min(1, "Expense date is required."),
  categoryId: z.coerce.number().int().positive("Category is required."),
  //currency: z.string().trim().optional(),
  notes: z.string().trim().optional(),
  merchant: z.string().trim().optional(),
  paymentMethod: z.string().trim().optional(),
  expenseType: z.enum(["Essential", "NonEssential"], {
    message: "Expense type is required.",
  }),
  isRecurring: z.boolean(),
});

export type ExpenseFormInput = z.input<typeof expenseSchema>;
export type ExpenseFormValues = z.output<typeof expenseSchema>;
