import { userResponseSchema } from "@/schemas/user-schemas"
import { UserService } from "@/types"

import api from ".."

const userServices: UserService = {
    AddOne: async (userData) => {
        const response = await api.post("/authentication/signup", userData)
        const validatedResponseData = userResponseSchema.safeParse(response.data)

        if (!validatedResponseData.success) {
            throw validatedResponseData.error
        }

        return validatedResponseData.data
    }
}

export default userServices
