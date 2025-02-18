import userServices from "@/api/services/user-services"
import { UserData, UserResponse } from "@/schemas/user-schemas"
import { ApiError } from "@/utils/errors"
import { useMutation } from "@tanstack/react-query"

export function useCreateUser() {
    return useMutation<UserResponse, ApiError, UserData>({
        mutationFn: userServices.AddOne,

        // Don't retry for specific error conditions
        retry: (failureCount: number, error: ApiError) => {
            const noRetryConditions = [
                error.details?.includes("already exists"),
                error.details?.includes("already registered"),
                error.statusCode && [400, 422, 429].includes(error.statusCode),
                error.type?.includes("validation-error") // Check error type
            ]
            if (noRetryConditions.some(Boolean)) {
                return false
            }

            return failureCount < 3
        }
    })
}
