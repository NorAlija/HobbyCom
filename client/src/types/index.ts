import { loginSchema, refreshTokenSchema, tokenResponseSchema } from "@/schemas/auth-schema"
import { signupSchema, userResponseSchema } from "@/schemas/user-schemas"
import { z } from "zod"

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
export type User = {
    id: UserId
    firstName: string
    lastName: string
    email: string
    username: string
    phone?: string | null
    avatarUrl?: string | null
    role: string
    createdAt: string
    updatedAt: string
}

export type Session = {
    userId: string
    createdAt: string
    updatedAt: string
    refreshedAt: string
    tokens: Token[]
    id: string
}

export type Token = {
    userId: string
    token: string
    createdAt: string
    tokenRevoked: boolean
    sessionId: string
    access_token: string
    id: string
}

export interface SignUpData
    extends Omit<User, "id" | "role" | "avatarUrl" | "createdAt" | "updatedAt"> {
    password: string
    confirmPassword: string
}

export interface UserService {
    AddOne: (userData: UserData) => Promise<UserResponse>
}

export type TokenResponse = z.infer<typeof tokenResponseSchema>
export type RefreshTokenRequest = z.infer<typeof refreshTokenSchema>
export type LoginInput = z.infer<typeof loginSchema>
export type UserData = z.infer<typeof signupSchema>
// export type UserResponse = z.infer<typeof userResponseSchema>
export type UserResponse = z.infer<typeof userResponseSchema> & {
    data: {
        sessions: Session[]
    }
}
export type LogoutBody = {
    email: string
    refresh_token: string | null
}

// -------------------------
// From-related Types
// -------------------------
export type FormErrors = {
    [key: string]: string
}
export type FormData = UserData

// -------------------------
// Token related Type
// -------------------------
export type TokenPayload = {
    // Standard JWT Claims
    nameid: string // User ID (UUID)
    SessionId: string
    email: string
    nbf: number // Not Before timestamp
    exp: number // Expiration timestamp
    iat: number // Issued at timestamp
    iss: string // Issuer
    aud: string // Audience
}
