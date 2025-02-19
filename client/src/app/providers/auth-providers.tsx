import { useCreateUser } from "@/features/user-management/useCreateUser"
import { SignUpData } from "@/types"
import { ApiError } from "@/utils/errors"
import { Session, User } from "@supabase/supabase-js"
import { useRouter, useSegments } from "expo-router"
import { createContext, useContext, useEffect, useState } from "react"
import { useToast } from "react-native-toast-notifications"
import { supabase } from "../../lib/supabase"

type AuthContextType = {
    user: User | null
    session: Session | null
    loading: boolean
    isPending: boolean
    signUp: (data: SignUpData) => Promise<void>
    signIn: (email: string, password: string) => Promise<void>
    signOut: () => Promise<void>
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

function AuthProvider({ children }: { children: React.ReactNode }) {
    const [user, setUser] = useState<User | null>(null)
    const [session, setSession] = useState<Session | null>(null)
    const [loading, setLoading] = useState(true)
    const router = useRouter()
    const segments = useSegments()
    const toast = useToast()

    const { isPending, mutate: registerUser } = useCreateUser()

    useEffect(() => {
        supabase.auth.getSession().then(({ data: { session } }) => {
            setSession(session)
            setUser(session?.user ?? null)
            setLoading(false)
        })

        const {
            data: { subscription }
        } = supabase.auth.onAuthStateChange((_event, session) => {
            setSession(session)
            setUser(session?.user ?? null)
            setLoading(false)
        })

        return () => subscription.unsubscribe()
    }, [])

    useEffect(() => {
        if (loading) return

        const inAuthGroup = segments[0] === "(authentication)"

        if (!user && !inAuthGroup) {
            router.replace("/(authentication)")
        } else if (user && inAuthGroup) {
            router.replace("/(community)")
        }
    }, [user, loading, segments])

    const signUp = async (data: SignUpData) => {
        return new Promise<void>((resolve, reject) => {
            registerUser(data, {
                onSuccess: () => {
                    toast.show("You have been successfully registered", {
                        type: "success",
                        placement: "top",
                        duration: 5000
                    })
                    resolve()
                },
                onError: (error: ApiError) => {
                    reject(error)
                }
            })
        })
    }

    const signIn = async (email: string, password: string) => {
        const { error } = await supabase.auth.signInWithPassword({
            email,
            password
        })
        if (error) throw error
    }

    const signOut = async () => {
        const { error } = await supabase.auth.signOut()
        if (error) throw error
    }

    return (
        <AuthContext.Provider
            value={{
                user,
                session,
                loading,
                isPending,
                signUp,
                signIn,
                signOut
            }}
        >
            {children}
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
