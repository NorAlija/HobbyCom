import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { Stack } from "expo-router"
import { ToastProvider } from "react-native-toast-notifications"
import { AuthProvider } from "./providers/auth-providers"

export default function RootLayout() {
    // Default options for react-query with default retry delay of max 30 seconds
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: {
                retryDelay: (attemptIndex) => Math.min(1000 * 2 ** attemptIndex, 30000)
            }
        }
    })
    return (
        <ToastProvider>
            <QueryClientProvider client={queryClient}>
                <AuthProvider>
                    <Stack>
                        <Stack.Screen
                            name="(authentication)"
                            options={{ headerShown: false, title: "Welcome" }}
                        />
                        <Stack.Screen
                            name="(community)"
                            options={{ headerShown: false, title: "Community" }}
                        />
                    </Stack>
                </AuthProvider>
            </QueryClientProvider>
        </ToastProvider>
    )
}
