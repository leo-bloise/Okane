import {
  randomBytes,
  createCipheriv,
  createDecipheriv,
} from "crypto";
import { ZodObject } from "zod";
import { $ZodType, $ZodTypeInternals } from "zod/v4/core";

const ALGORITHM = "aes-256-gcm";
const KEY = Buffer.from(process.env.ENCRYPTION_KEY ?? 'change-me', "hex");

export function encrypt(text: string): string {
  const iv = randomBytes(12);

  const cipher = createCipheriv(
    ALGORITHM,
    KEY,
    iv
  );

  const encrypted = Buffer.concat([
    cipher.update(text, "utf8"),
    cipher.final(),
  ]);

  const tag = cipher.getAuthTag();

  return Buffer.concat([
    iv,
    tag,
    encrypted,
  ]).toString("base64url");
}

export function decrypt(token: string): string {
  const data = Buffer.from(
    token,
    "base64url"
  );

  const iv = data.subarray(0, 12);
  const tag = data.subarray(12, 28);
  const encrypted = data.subarray(28);

  const decipher = createDecipheriv(
    ALGORITHM,
    KEY,
    iv
  );

  decipher.setAuthTag(tag);

  const decrypted = Buffer.concat([
    decipher.update(encrypted),
    decipher.final(),
  ]);

  return decrypted.toString("utf8");
}

export async function parseSafe<T extends Readonly<{ [k: string]: $ZodType<unknown, unknown, $ZodTypeInternals<unknown, unknown>>; }>>(request: Request, schema: ZodObject<T>)  {
  const body = await request.json();

  return schema.safeParse(body);
}