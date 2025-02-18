import { ExtraConfig } from "@/types"
import Constants from "expo-constants"
import { Platform } from "react-native"

export default function getBaseURL() {
    const extra = Constants.expoConfig?.extra as ExtraConfig
    if (!extra || !extra.apiUrl) {
        throw new Error("ExtraConfig is missing or not loaded.")
    }

    const mode = extra.mode

    if (mode === "development") {
        if (Platform.OS === "android") {
            return __DEV__ ? "http://10.0.2.2:PORT" : extra.apiUrl.development.network
        }
        // return __DEV__ ? extra.apiUrl.development.local : extra.apiUrl.development.network
        return __DEV__ ? extra.apiUrl.development.network : undefined
    }
    return extra.apiUrl.production
}
