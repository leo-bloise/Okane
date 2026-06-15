import { ZodError } from "zod";

export function zodToDetailsAdapter<T>(
  error: ZodError<T>
): Partial<Record<keyof T, string>> {
  const { fieldErrors } = error.flatten();

  const details: Partial<Record<keyof T, string>> = {};

  for (const fieldName in fieldErrors) {
    const messages = fieldErrors[fieldName as keyof typeof fieldErrors];

    if (messages?.length) {
      details[fieldName as keyof T] = messages[0];
    }
  }

  return details;
}