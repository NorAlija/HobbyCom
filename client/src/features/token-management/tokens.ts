import { AccessTokenKey, LoggedInEmailKey, RefreshTokenKey } from "@/utils/constants"
import { getItem, removeItem, setItem } from "@/utils/scureStorage"

export const useToken = () => {
    const setTokens = async (access_token: string | null, refresh_token: string | null) => {
        if (access_token) {
            await setItem(AccessTokenKey, access_token)
        }
        if (refresh_token) {
            await setItem(RefreshTokenKey, refresh_token)
        }
    }

    const clearTokens = async () => {
        await removeItem(AccessTokenKey)
        await removeItem(RefreshTokenKey)
        await removeItem(LoggedInEmailKey)
    }

    const getAccessToken = async (): Promise<string | null> => await getItem(AccessTokenKey)
    const getRefreshToken = async (): Promise<string | null> => await getItem(RefreshTokenKey)
    const getLoggedInEmail = async (): Promise<string | null> => await getItem(LoggedInEmailKey)

    return {
        setTokens,
        clearTokens,
        getAccessToken,
        getRefreshToken,
        getLoggedInEmail
    }
}
