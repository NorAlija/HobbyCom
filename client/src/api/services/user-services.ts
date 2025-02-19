import { userResponseSchema } from "@/schemas/user-schemas"
import { UserService } from "@/types"

import { ApiError } from "@/utils/errors"
import api from ".."

const userServices: UserService = {
    AddOne: async (userData) => {
        try {
            const response = await api.post("/authentication/signup", userData)
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

            return validatedResponseData.data
        } catch (error) {
            // Re-throw ApiError instances (from the interceptor)
            if (error instanceof ApiError) {
                throw error
            }
            // Transform unexpected errors into ApiError
            throw new ApiError("An unexpected error occurred", 500)
        }
    }
}

export default userServices
