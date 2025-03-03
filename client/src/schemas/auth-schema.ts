import { z } from "zod"

export const loginSchema = z.object({
    email: z
        .string()
        .min(1, "Email is required")
        .nonempty("Email is required")
        .email("Invalid email address"),
    password: z.string().min(1, "Password is required")
})

export const tokenResponseSchema = z.object({
    success: z.boolean(),
    data: z.object({
        accessToken: z.string(),
        refreshToken: z.string()
    })
})

export const refreshTokenSchema = z.object({
    email: z.string().email(),
    accessToken: z.string(),
    refreshToken: z.string()
})
