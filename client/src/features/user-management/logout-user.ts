import authService from "@/api/services/auth-service"
import { ApiError } from "@/utils/errors"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { useToast } from "react-native-toast-notifications"
import { useToken } from "../token-management/tokens"

export function useLogout() {
    const { clearTokens, getLoggedInEmail, getRefreshToken } = useToken()
    const toast = useToast()
    const queryClient = useQueryClient()

    return useMutation({
        mutationKey: ["logout"],
        mutationFn: async () => {
            await queryClient.invalidateQueries({ queryKey: ["auth"] })
            const email = await getLoggedInEmail()
            const refreshToken = await getRefreshToken()

            if (!email) {
                throw new Error("No logged in email found")
            }

            const LogoutBody = {
                email: email,
                refresh_token: refreshToken
            }
            return await authService.logout(LogoutBody)
        },
        retry: (failureCount: number, error: ApiError) => {
            const noRetryConditions = [
                error.statusCode && [400, 422, 429, 401, 500].includes(error.statusCode)
            ]
            if (noRetryConditions.some(Boolean)) {
                return false
            }

            return failureCount < 3
        },
        onSuccess: async () => {
            await clearTokens()

            await queryClient.invalidateQueries({ queryKey: ["auth"] })

            toast.show("Logout successful", {
                type: "success",
                placement: "top",
                duration: 2000
            })
        },
        onError: async (error: ApiError) => {
            toast.show(error.title || "Logout failed.Try again!", {
                type: "danger",
                placement: "top",
                duration: 2000
            })
        }
    })
}
