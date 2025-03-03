import { userResponseSchema } from "@/schemas/user-schemas"
import { LoginInput, RefreshTokenRequest, TokenResponse, UserResponse } from "@/types"
import { ApiError } from "@/utils/errors"
import api from ".."

export default {
    refresh: async (tokenResponse: RefreshTokenRequest): Promise<TokenResponse> => {
        const response = await api.post("/authentications/refresh", tokenResponse)
        console.log(response)
        return response.data
    },
    logout: async (email: string): Promise<boolean> => {
        const response = await api.post("/authentications/logout", {
            email
        })
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
