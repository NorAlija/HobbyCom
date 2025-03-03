import authService from "@/api/services/auth-service"
import { LoginInput, UserResponse } from "@/types"
import { LoggedInEmailKey } from "@/utils/constants"
import { ApiError } from "@/utils/errors"
import { setItem } from "@/utils/scureStorage"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { useToast } from "react-native-toast-notifications"
import { useToken } from "../token-management/tokens"

export function useLogin() {
    const { clearTokens, setTokens } = useToken()
    const queryClient = useQueryClient()
    const toast = useToast()

    return useMutation<UserResponse, ApiError, LoginInput>({
        mutationKey: ["login"],
        mutationFn: authService.login,

        retry: (failureCount: number, error: ApiError) => {
            const noRetryConditions = [
                error.statusCode && [400, 401, 422, 429, 500].includes(error.statusCode),
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

            const AccessToken = data.data.access_token
            const RefreshToken = data.data.refresh_token
            const email = data.data.user.email

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
