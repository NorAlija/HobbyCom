import { z } from "zod"

const userBaseSchema = z.object({
    firstName: z
        .string()
        .min(2, { message: "Firstname must be at least 2 characters long" })
        .max(50, { message: "Firstname must not exceed 50 characters" }),
    lastName: z
        .string()
        .min(2, { message: "Lastname must be at least 2 characters long" })
        .max(50, { message: "Lastname must not exceed 50 characters" }),
    email: z.string().email({ message: "Invalid email address" }),
    userName: z
        .string()
        .min(4, { message: "Username must be at least 4 characters long" })
        .regex(/^[a-zA-Z0-9]*$/, { message: "Username must contain only letters and numbers" }),
    phoneNumber: z.preprocess(
        (val) => (val === "" ? null : val),
        z
            .string()
            .min(5, { message: "Phone number can't be less than 5 digits" })
            .max(15, { message: "Phone number can't exceed 15 digits" })
            .regex(/^[0-9]*$/, { message: "Phone number must contain only numbers" })
            .nullable()
            .optional()
    ),
    role: z.string().default("USER"),
    profilePicture: z.string().nullable().optional()
})

export const signupSchema = userBaseSchema
    .omit({ role: true, profilePicture: true })
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

export const userSchema = z.object({
    id: z.string(),
    firstName: z.string(),
    lastName: z.string(),
    email: z.string().email(),
    userName: z.string().nullable(),
    phoneNumber: z.string().nullable(),
    profilePicture: z.string().url().nullable(),
    role: z.string(),
    createdAt: z.string().datetime(),
    updatedAt: z.string().datetime(),
    sessions: z.array(
        z.object({
            userId: z.string(),
            createdAt: z.string().datetime(),
            updatedAt: z.string().datetime(),
            refreshedAt: z.string().datetime(),
            tokens: z.array(
                z.object({
                    userId: z.string(),
                    token: z.string(),
                    createdAt: z.string().datetime(),
                    tokenRevoked: z.boolean(),
                    sessionId: z.string(),
                    access_token: z.string(),
                    id: z.string()
                })
            ),
            id: z.string()
        })
    )
})

export const userResponseSchema = z.object({
    success: z.boolean(),
    // data: z.object({
    //     access_token: z.string(),
    //     token_type: z.string(),
    //     expires_in: z.number(),
    //     expires_at: z.string().datetime({ offset: true }),
    //     expired: z.boolean(),
    //     refresh_token: z.string(),
    //     user: userSchema
    // })
    data: userSchema
})
