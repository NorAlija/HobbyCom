import { useCreateUser } from "@/features/user-management/create-user"
import { useIsAuth } from "@/features/user-management/is-user-authenticated"
import { useLogin } from "@/features/user-management/login-user"
import { useLogout } from "@/features/user-management/logout-user"
import { LoginInput, SignUpData, User } from "@/types"
import { ApiError } from "@/utils/errors"
import { useRouter, useSegments } from "expo-router"
import { createContext, useContext, useEffect, useState } from "react"
import { ActivityIndicator, View } from "react-native"
import { useToast } from "react-native-toast-notifications"

type AuthContextType = {
    user: User | null
    isAuthenticated: boolean
    isAuthLoading: boolean
    isSigningUp: boolean
    isSigningIn: boolean
    signUp: (data: SignUpData) => Promise<void>
    signOut: () => Promise<void>
    signIn: (data: LoginInput) => Promise<void>
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

function AuthProvider({ children }: { children: React.ReactNode }) {
    const [user, setUser] = useState<User | null>(null)
    const router = useRouter()
    const segments = useSegments()
    const toast = useToast()

    const { mutateAsync: logoutMutation } = useLogout()

    const { isPending: isSigningUp, mutate: registerUser } = useCreateUser()
    const { isPending: isSigningIn, mutate: loginUser } = useLogin()

    const { data: authState, isLoading: isAuthLoading } = useIsAuth()

    const isAuthenticated = authState?.isAuthenticated ?? false

    useEffect(() => {
        if (isAuthLoading || !authState) return

        const routeGroup = segments[0] ?? "(authentication)"
        const shouldAuth = !authState.isAuthenticated // should authenticate if isAuthenticated is false
        const isAuthRoute = routeGroup === "(authentication)"

        if (shouldAuth && !isAuthRoute) {
            router.replace("/(authentication)")
        } else if (!shouldAuth && isAuthRoute) {
            router.replace("/(community)")
        }
    }, [authState, isAuthLoading, segments, router])

    const signUp = async (data: SignUpData) => {
        return new Promise<void>((resolve, reject) => {
            registerUser(data, {
                onSuccess: () => {
                    resolve()
                },
                onError: (error: ApiError) => {
                    toast.show("Registration failed", {
                        type: "danger",
                        placement: "top",
                        duration: 2000
                    })
                    reject(error)
                }
            })
        })
    }

    const signIn = async (data: LoginInput) => {
        return new Promise<void>((resolve, reject) => {
            loginUser(data, {
                onSuccess: () => {
                    resolve()
                },
                onError: (error: ApiError) => {
                    toast.show("Login failed", {
                        type: "danger",
                        placement: "top",
                        duration: 2000
                    })
                    reject(error)
                }
            })
        })
    }

    const signOut = async () => {
        await logoutMutation()
    }

    return (
        <AuthContext.Provider
            value={{
                user,
                isAuthenticated,
                isAuthLoading,
                isSigningUp,
                isSigningIn,
                signUp,
                signOut,
                signIn
            }}
        >
            {isAuthLoading ? (
                <View style={{ flex: 1, justifyContent: "center", alignItems: "center" }}>
                    <ActivityIndicator size="large" />
                </View>
            ) : (
                children
            )}
        </AuthContext.Provider>
    )
}

function useAuth() {
    const context = useContext(AuthContext)
    if (context === undefined) {
        throw new Error("useAuth must be used within an AuthProvider")
    }
    return context
}

// Export both the provider and the hook
export { AuthProvider, useAuth }

// Add a default export of the provider
export default AuthProvider
