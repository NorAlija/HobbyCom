import userServices from "@/api/services/user-services"
import { UserData, UserResponse } from "@/types"
import { LoggedInEmailKey } from "@/utils/constants"
import { ApiError } from "@/utils/errors"
import { setItem } from "@/utils/scureStorage"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { useToast } from "react-native-toast-notifications"
import { useToken } from "../token-management/tokens"

export function useCreateUser() {
    const { clearTokens, setTokens } = useToken()
    const queryClient = useQueryClient()
    const toast = useToast()

    return useMutation<UserResponse, ApiError, UserData>({
        mutationFn: userServices.AddOne,

        // Don't retry for specific error conditions
        retry: (failureCount: number, error: ApiError) => {
            const noRetryConditions = [
                error.details?.includes("already taken"),
                error.details?.includes("already registered"),
                error.statusCode && [400, 401, 422, 429, 500, 404].includes(error.statusCode),
                error.type?.includes("validation-error"), // Check error type
                error.type?.includes("ValidationError") // Check error type
            ]
            if (noRetryConditions.some(Boolean)) {
                return false
            }

            return failureCount < 3
        },
        onSuccess: async (data) => {
            await clearTokens()

            const AccessToken = data.data.sessions[0].tokens[0].access_token
            const RefreshToken = data.data.sessions[0].tokens[0].token
            const email = data.data.email

            await setTokens(AccessToken, RefreshToken)
            await setItem(LoggedInEmailKey, email)

            // Invalidate auth state to reflect new login
            await queryClient.invalidateQueries({ queryKey: ["auth"] })

            toast.show("Registration successful", {
                type: "success",
                placement: "top",
                duration: 2000
            })
        }
    })
}
