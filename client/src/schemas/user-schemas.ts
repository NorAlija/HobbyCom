import { z } from "zod"

const userBaseSchema = z.object({
    firstname: z
        .string()
        .min(2, { message: "Firstname must be at least 2 characters long" })
        .max(50, { message: "Firstname must not exceed 50 characters" }),
    lastname: z
        .string()
        .min(2, { message: "Lastname must be at least 2 characters long" })
        .max(50, { message: "Lastname must not exceed 50 characters" }),
    email: z.string().email({ message: "Invalid email address" }),
    username: z
        .string()
        .min(4, { message: "Username must be at least 4 characters long" })
        .regex(/^[a-zA-Z0-9]*$/, { message: "Username must contain only letters and numbers" }),
    phone: z.string().nullable().optional(),
    type: z.string().default("USER"),
    avatarUrl: z.string().nullable().optional()
})

export const signupSchema = userBaseSchema
    .omit({ type: true, avatarUrl: true })
    .extend({
        password: z
            .string()
            .min(8, { message: "Password must be at least 8 characters long" })
            .regex(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/, {
                message:
                    "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character"
            }),
        confirmPassword: z.string()
    })
    .refine((data) => data.password === data.confirmPassword, {
        message: "Passwords do not match",
        path: ["confirmPassword"]
    })

export const userResponseSchema = z.object({
    id: z.string(),
    firstname: z.string(),
    lastname: z.string(),
    username: z.string(),
    email: z.string(),
    phone: z.string(),
    type: z.string(),
    avatarUrl: z.string(),
    created_at: z.string()
})

export type UserData = z.infer<typeof signupSchema>
export type UserResponse = z.infer<typeof userResponseSchema>
