import { tokenResponseSchema } from "@/schemas/auth-schema"
import { userResponseSchema } from "@/schemas/user-schemas"
import { LoginInput, LogoutBody, RefreshTokenRequest, TokenResponse, UserResponse } from "@/types"
import { ApiError } from "@/utils/errors"
import api from ".."

export default {
    refresh: async (tokenResponse: RefreshTokenRequest): Promise<TokenResponse> => {
        const response = await api.post("/authentications/refresh", tokenResponse)
        const validatedResponseData = tokenResponseSchema.safeParse(response.data)
        if (!validatedResponseData.success) {
            throw new ApiError(
                "Invalid response format",
                500,
                "ValidationError",
                undefined,
                undefined,
                undefined,
                undefined,
                validatedResponseData.error.errors.map((error) => error.message)
            )
        }
        return response.data
    },
    logout: async (logoutBody: LogoutBody): Promise<boolean> => {
        const response = await api.post("/authentications/logout", logoutBody)
        return response.data
    },
    login: async (credentials: LoginInput): Promise<UserResponse> => {
        const response = await api.post("/authentications/login", credentials)

        const validatedResponseData = userResponseSchema.safeParse(response.data)

        if (!validatedResponseData.success) {
            throw new ApiError(
                "Invalid response format",
                500,
                "ValidationError",
                undefined,
                undefined,
                undefined,
                undefined,
                validatedResponseData.error.errors.map((error) => error.message)
            )
        }

        return response.data
    }
}
