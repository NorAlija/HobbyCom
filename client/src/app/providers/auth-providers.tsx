import { Session, User } from "@supabase/supabase-js"
import { useRouter, useSegments } from "expo-router"
import { createContext, useContext, useEffect, useState } from "react"
import { supabase } from "../../lib/supabase"

type SignUpData = {
    firstname: string
    lastname: string
    email: string
    username: string
    phone?: string
    type?: string
    avatarUrl?: string
    password: string
    confirmPassword: string
}

type AuthContextType = {
    user: User | null
    session: Session | null
    loading: boolean
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
        const url = process.env.DEVELOPMENT_URL + "/authentication/signup"

        const requestData = {
            firstname: data.firstname,
            lastname: data.lastname,
            email: data.email,
            username: data.username,
            phone: data.phone || null,
            password: data.password,
            confirmPassword: data.confirmPassword,
            type: data.type || null,
            avatarUrl: data.avatarUrl || null
        }

        console.log("Attempting signup with data:", requestData)

        try {
            const response = await fetch(url, {
                method: "POST",
                headers: {
                    Accept: "application/json",
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(requestData)
            })

            console.log("Response status:", response.status)
            const responseText = await response.text()
            console.log("Response text:", responseText)

            if (!response.ok) {
                throw new Error(`Signup failed: ${response.status} ${responseText}`)
            }

            const json = JSON.parse(responseText)
            return json.data
        } catch (error) {
            console.error("Signup error:", error)
            throw error
        }
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
