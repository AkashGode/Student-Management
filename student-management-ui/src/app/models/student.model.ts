export interface Student {
  id: number;
  name: string;
  email: string;
  age: number;
  course: string;
  createdDate: string;
}

export interface StudentCreateDto {
  name: string;
  email: string;
  age: number;
  course: string;
}

export interface StudentUpdateDto {
  name: string;
  email: string;
  age: number;
  course: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors?: string[];
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  username: string;
}
