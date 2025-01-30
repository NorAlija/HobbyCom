// import { createContext, useContext, useEffect, useState } from 'react';
// import { supabase } from '../../lib/supabase';
// import { Session, User } from '@supabase/supabase-js';
// import { useRouter, useSegments } from 'expo-router';

// type AuthContextType = {
//   user: User | null;
//   session: Session | null;
//   loading: boolean;
//   signUp: (email: string, password: string) => Promise<void>;
//   signIn: (email: string, password: string) => Promise<void>;
//   signOut: () => Promise<void>;
// };

// const AuthContext = createContext<AuthContextType | undefined>(undefined);
// type SignUpData = {
//   email: string;
//   password: string;
//   firstName: string;
//   lastName: string;
//   phoneNumber?: string;
// };

// export function AuthProvider({ children }: { children: React.ReactNode }) {
//   const [user, setUser] = useState<User | null>(null);
//   const [session, setSession] = useState<Session | null>(null);
//   const [loading, setLoading] = useState(true);
//   const router = useRouter();
//   const segments = useSegments();

//   useEffect(() => {
//     supabase.auth.getSession().then(({ data: { session } }) => {
//       setSession(session);
//       setUser(session?.user ?? null);
//       setLoading(false);
//     });

//     const { data: { subscription } } = supabase.auth.onAuthStateChange((_event, session) => {
//       setSession(session);
//       setUser(session?.user ?? null);
//       setLoading(false);
//     });

//     return () => subscription.unsubscribe();
//   }, []);

//   useEffect(() => {
//     if (loading) return;

//     const inAuthGroup = segments[0] === "(authentication)";

//     if (!user && !inAuthGroup) {
//       router.replace("/(authentication)");
//     } else if (user && inAuthGroup) {
//       router.replace("/(community)");
//     }
//   }, [user, loading, segments]);

//   // const signUp = async (email: string, password: string) => {
//   //   const { error } = await supabase.auth.signUp({
//   //     email,
//   //     password,
//   //   });
//   //   if (error) throw error;
//   // };
//   const signUp = async (email: string, password: string) => {
//     try {
//       // First sign up the user
//       const { data: authData, error: authError } = await supabase.auth.signUp({
//         email,
//         password,
//       });
//       if (authError) throw authError;

//       if (authData.user) {
//         // Create a profile for the user
//         const { error: profileError } = await supabase
//           .from('profiles')
//           .insert([
//             {
//               user_id: authData.user.id,
//               email: email,
//               first_name: '',  // Default empty values
//               last_name: '',
//               created_at: new Date().toISOString(),
//             }
//           ]);

//         if (profileError) throw profileError;
//       }
//     } catch (error) {
//       throw error;
//     }
//   };

//   const signIn = async (email: string, password: string) => {
//     const { error } = await supabase.auth.signInWithPassword({
//       email,
//       password,
//     });
//     if (error) throw error;
//   };

//   const signOut = async () => {
//     const { error } = await supabase.auth.signOut();
//     if (error) throw error;
//   };

//   return (
//     <AuthContext.Provider value={{
//       user,
//       session,
//       loading,
//       signUp,
//       signIn,
//       signOut,
//     }}>
//       {children}
//     </AuthContext.Provider>
//   );
// }

// export function useAuth() {
//   const context = useContext(AuthContext);
//   if (context === undefined) {
//     throw new Error('useAuth must be used within an AuthProvider');
//   }
//   return context;
// }

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
    try {
      const { data: authData, error: authError } = await supabase.auth.signUp({
        email: data.email,
        password: data.password
      })
      if (authError) throw authError

      if (authData.user) {
        const { error: profileError } = await supabase.from("profiles").insert([
          {
            user_id: authData.user.id,
            email: data.email,
            first_name: data.firstName,
            last_name: data.lastName,
            phone_number: data.phoneNumber,
            created_at: new Date().toISOString()
          }
        ])

        if (profileError) throw profileError
      }
    } catch (error) {
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
