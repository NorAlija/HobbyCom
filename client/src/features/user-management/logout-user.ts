import authService from "@/api/services/auth-service"
import { ApiError } from "@/utils/errors"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { useRouter } from "expo-router"
import { useToast } from "react-native-toast-notifications"
import { useToken } from "../token-management/tokens"

export function useLogout() {
    const router = useRouter()
    const { clearTokens, getLoggedInEmail } = useToken()
    const toast = useToast()
    const queryClient = useQueryClient()

    return useMutation({
        mutationKey: ["logout"],
        mutationFn: async () => {
            const email = await getLoggedInEmail()
            console.log("Logging out", email)
            if (!email) {
                throw new Error("No logged in email found")
            }
            return authService.logout(email)
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
            // Force logout on error anyway
            await clearTokens()

            await queryClient.invalidateQueries({ queryKey: ["auth"] })

            toast.show(error.details || "Logout failed", {
                type: "danger",
                placement: "top",
                duration: 2000
            })
        }
    })
}
