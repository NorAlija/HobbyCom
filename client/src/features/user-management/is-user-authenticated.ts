import authService from "@/api/services/auth-service"
import { RefreshTokenRequest } from "@/types"
import { AccessTokenKey, LoggedInEmailKey, RefreshTokenKey } from "@/utils/constants"
import { ApiError } from "@/utils/errors"
import { isTokenExpired } from "@/utils/jwt-decode"
import { removeItem } from "@/utils/scureStorage"
import { useQuery } from "@tanstack/react-query"
import { useCallback } from "react"
import { useToast } from "react-native-toast-notifications"
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
    const toast = useToast()

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
                toast.show("Failed to refresh token", {
                    type: "danger",
                    placement: "top",
                    duration: 2000
                })
                //TODO: if there is no access token, clear the tokens and set isAuthenticated to false
                //TODO: check if the token has a session associated with it, if not, clear the tokens and set isAuthenticated to false
                //TODO: if the token has a session try to inavalidate the auth state to try and refresh the token again
                //TODO: if the refresh token is invalid, clear the tokens and set isAuthenticated to false
                //Todo: if thhere is no session associated with the token, clear the tokens and set isAuthenticated to false

                // await logout()
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
                error.statusCode && [403, 401, 500, 404].includes(error.statusCode)
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
