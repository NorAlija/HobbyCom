import { userResponseSchema } from "@/schemas/user-schemas"
import { UserService } from "@/types"

import { ApiError } from "@/utils/errors"
import api from ".."

const userServices: UserService = {
    AddOne: async (userData) => {
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
    }
}

export default userServices
