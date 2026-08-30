export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface RegisteredUser {
  id: string;
  name: string;
  email: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResult {
  expiresAt: string;
}
