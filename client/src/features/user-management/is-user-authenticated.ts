import authService from "@/api/services/auth-service"
import { RefreshTokenRequest } from "@/types"
import { AccessTokenKey, LoggedInEmailKey, RefreshTokenKey } from "@/utils/constants"
import { ApiError } from "@/utils/errors"
import { isTokenExpired } from "@/utils/jwt-decode"
import { removeItem } from "@/utils/scureStorage"
import { useQuery } from "@tanstack/react-query"
import { useCallback } from "react"
import { useToken } from "../token-management/tokens"
import { useLogout } from "./logout-user"

type AuthState = {
    isAuthenticated: boolean
}

type UseIsAuthOptions = {
    onAuthStateChange?: (data: AuthState) => void
}

// const TOKEN_LIFETIME = 58 * 60 * 1000 // 58  minutes
const TOKEN_LIFETIME = 30 * 1000

// export function useIsAuth() {
export function useIsAuth(options?: UseIsAuthOptions) {
    const { getAccessToken, getRefreshToken, clearTokens, setTokens, getLoggedInEmail } = useToken()
    const { mutateAsync: logout } = useLogout()

    const checkAuthState = useCallback(async (): Promise<AuthState> => {
        const [accessToken, refreshToken, email] = await Promise.all([
            getAccessToken(),
            getRefreshToken(),
            getLoggedInEmail()
        ])

        if (!accessToken || !refreshToken || !email) {
            console.log("No tokens found")
            return { isAuthenticated: false }
        }

        const refreshObject: RefreshTokenRequest = {
            email: email,
            refreshToken: refreshToken,
            accessToken: accessToken
        }

        if (isTokenExpired(accessToken)) {
            try {
                console.log("Token is expired")
                const newTokens = await authService.refresh(refreshObject)
                await removeItem(AccessTokenKey)
                await removeItem(RefreshTokenKey)
                await setTokens(newTokens.data.accessToken, newTokens.data.refreshToken)
                return { isAuthenticated: true }
            } catch (error) {
                console.error("Failed to refresh token", error)
                await logout()
                await clearTokens()
                removeItem(LoggedInEmailKey)
                return { isAuthenticated: false }
            }
        } else {
            console.log(
                "Token is valid",
                "Refreshes in:",
                Math.floor((Date.now() + TOKEN_LIFETIME - Date.now()) / 60000) + " minutes"
            )
            return { isAuthenticated: true }
        }
    }, [getAccessToken, getRefreshToken, logout, setTokens, clearTokens])

    const result = useQuery<AuthState, ApiError, AuthState>({
        queryKey: ["auth"],
        queryFn: checkAuthState,
        staleTime: TOKEN_LIFETIME,
        refetchInterval: TOKEN_LIFETIME,
        refetchIntervalInBackground: true,
        refetchOnWindowFocus: true,
        retry: (failureCount: number, error: ApiError) => {
            const noRetryConditions = [
                error.statusCode && [403, 401, 500].includes(error.statusCode)
            ]
            if (noRetryConditions.some(Boolean)) {
                return false
            }

            return failureCount < 3
        }
    })

    if (options?.onAuthStateChange && result.data) {
        options.onAuthStateChange(result.data)
    }

    return result
}
