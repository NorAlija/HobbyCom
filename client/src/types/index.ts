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
    firstname: string | null
    lastname: string | null
    email: string
    username: string | null
    phone?: string | null
    type?: string | null
    avatarUrl?: string | null
    created_at: string
}

export type SignupUser = {
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

export interface SignUpData extends Omit<SignupUser, "id" | "type" | "avatarUrl" | "created_at"> {
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
export type UserResponse = z.infer<typeof userResponseSchema>

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
    iss: string // Issuer ("https://your-supabase-url.auth/v1")
    sub: string // User ID (UUID)
    aud: string // Audience ("authenticated")
    exp: number // Expiration timestamp (seconds since epoch)
    iat: number // Issued at timestamp

    // Supabase-specific Claims
    email: string
    phone?: string // Optional as it can be empty string
    app_metadata: {
        provider: string
        providers: string[]
    }
    user_metadata: {
        avatar_url: string
        email: string
        email_verified: boolean
        first_name: string
        last_name: string
        phone: string
        phone_verified: boolean
        type: string
        username: string
    }
    role: string // "authenticated"
    aal: string // Authenticator Assurance Level ("aal1")
    amr: Array<{
        method: string // "password", "otp", etc.
        timestamp: number
    }>
    session_id: string
    is_anonymous: boolean
}
