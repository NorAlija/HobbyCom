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
    phone: z
        .string()
        .min(5, { message: "Phone number can't be less than 5 digits" })
        .max(15, { message: "Phone number can't exceed 15 digits" })
        .regex(/^[0-9]*$/, { message: "Phone number must contain only numbers" })
        .nullable()
        .optional()
        .or(z.literal(null)),
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

const userSchema = z.object({
    id: z.string(),
    firstName: z.string().nullable(),
    lastName: z.string().nullable(),
    email: z.string().email(),
    username: z.string().nullable(),
    phone: z.string().nullable(),
    type: z.string().nullable(),
    avatarUrl: z.string().url().nullable(),
    createdAt: z.string().datetime()
})

export const userResponseSchema = z.object({
    success: z.boolean(),
    data: z.object({
        access_token: z.string(),
        token_type: z.string(),
        expires_in: z.number(),
        expires_at: z.string().datetime({ offset: true }),
        expired: z.boolean(),
        refresh_token: z.string(),
        user: userSchema
    })
})
