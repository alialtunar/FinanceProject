import { jwtVerify } from "jose";

export function getJwtSecretKey() {
  const secret = "your-secret-key5555555sasaaxxxxxxxxx258222222222222222222222222";

  if (!secret) {
    throw new Error("JWT Secret key is not matched");
  }

  return new TextEncoder().encode(secret);
}

export async function verifyJwtToken(token) {
  try {
    const { payload } = await jwtVerify(token, getJwtSecretKey());

    return payload;
  } catch (error) {
    return null;
  }
}