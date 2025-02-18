import { UserData, UserResponse } from "@/schemas/user-schemas"

// -------------------------
// EXPO Configuration Related Types
// -------------------------
export type ExtraConfig = {
    apiUrl: {
        development: {
            local: string
            network: string
        }
        production: string
    }
    mode: string
}

// -------------------------
// ID Types
// -------------------------
export type UserId = string

// -------------------------
// User-related Types
// -------------------------
export interface User {
    id: UserId
    firstname: string
    lastname: string
    email: string
    username: string
    phone?: string | null
    type?: string
    avatarUrl?: string | null
    created_at: string
}

export interface SignUpData extends Omit<User, "id" | "type" | "avatarUrl" | "created_at"> {
    password: string
    confirmPassword: string
}

export interface LoginResult {
    user: User
}

export interface UserService {
    AddOne: (userData: UserData) => Promise<UserResponse>
}

// -------------------------
// From-related Types
// -------------------------
export type FormErrors = {
    [key: string]: string
}
export type FormData = UserData
