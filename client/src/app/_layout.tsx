// import { Stack } from "expo-router";

// // This is the root layout that shows how the app works
// export default function RootLayout() {
//     return (
//         <Stack>
//             {/* Auth screen as the initial route */}
//             <Stack.Screen
//                 name="(authentication)"
//                 options={{ headerShown: false, title: "Welcome" }}
//             />

//             {/* Community screen for authenticated users */}
//             <Stack.Screen
//                 name="(community)"
//                 options={{ headerShown: false, title: "Community" }}
//             />
//         </Stack>
//     );
// }

import { Stack } from "expo-router"
import { AuthProvider } from "./providers/auth-providers"

export default function RootLayout() {
    return (
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
    )
}
