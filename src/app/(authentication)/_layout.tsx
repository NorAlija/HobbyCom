// import { Stack } from "expo-router";

// export default function AuthLayout() {
//     return (
//         <Stack>
//             <Stack.Screen 
//                 name="index" 
//                 options={{ headerShown: false }} 
//             />
//              <Stack.Screen 
//                 name="signup" 
//                 options={{ headerShown: false }} 
//             />
//         </Stack>
//     );
// }

import { Stack } from "expo-router";
import { useAuth } from "../providers/auth-providers";

export default function AuthLayout() {
  const { loading } = useAuth();

  if (loading) return null;

  return (
    <Stack screenOptions={{ headerShown: false }}>
      <Stack.Screen name="index" />
      <Stack.Screen name="signup" />
    </Stack>
  );
}