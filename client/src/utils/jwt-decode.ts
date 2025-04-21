import { TokenPayload } from "@/types"
import { jwtDecode } from "jwt-decode"

// Combined validation and decoding function
export function validateToken(
    token: string | null,
    bufferMs = 0
): {
    isValid: boolean
    isExpired: boolean
    payload?: TokenPayload
} {
    if (!token) return { isValid: false, isExpired: true }

    try {
        const payload = jwtDecode<TokenPayload>(token)
        const currentTime = Math.floor(Date.now() / 1000)
        const bufferSeconds = Math.floor(bufferMs / 1000)

        // Validate required claims
        const isValid = !!payload.nameid && !!payload.exp && typeof payload.exp === "number"

        // Check expiration with proactive buffer
        const isExpired = isValid && payload.exp < currentTime + bufferSeconds

        return { isValid, isExpired, payload }
    } catch (error) {
        return { isValid: false, isExpired: true }
    }
}

export function isTokenExpired(token: string | null): boolean {
    return validateToken(token).isExpired
}
