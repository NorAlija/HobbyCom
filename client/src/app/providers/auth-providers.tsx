

import { createContext, useContext, useEffect, useState } from "react"
import { supabase } from "../../lib/supabase"
import { Session, User } from "@supabase/supabase-js"
import { useRouter, useSegments } from "expo-router"



// Add this type
type SignUpData = {
    email: string
    password: string
    firstName: string
    lastName: string
    phoneNumber?: string
    username: string
    type: string
    avatarUrl: string
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
            const url = process.env.DEVELOPMENT_URL +"/authentication/signup";
        
            try {
                const response = await fetch(url, {
                    method: "POST", // Use POST to send data
                    headers: {
                        "Content-Type": "application/json", // Specify JSON content type
                    },
                    body: JSON.stringify(data), // Convert data to JSON string
                });
                console.log(response)
        
                if (!response.ok) {
                    throw new Error(`Failed to sign up: ${response.statusText}`);
                }
        
                const json = await response.json();
                console.log("Signup successful:", json);
                return json;
            } catch (error) {
                console.error("Signup error:", error);
                throw new Error("Something went wrong during signup!");
            }
        };
        
    

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
