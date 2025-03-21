import { ConfigContext, ExpoConfig } from "expo/config"

declare const process: {
    env: {
        [key: string]: string | undefined
    }
}

export default ({ config }: ConfigContext): ExpoConfig => {
    const expoConfig: ExpoConfig = {
        ...config,
        name: config.name || "h-com", // These fields are required by ExpoConfig type
        slug: config.slug || "h-com", // ..
        version: config.version || "1.0.0", // ..
        extra: {
            ...config.extra,
            apiUrl: {
                development: {
                    // local: process.env.EXPO_PUBLIC_DEVELOPMENT_URL_LOCAL,
                    network: process.env.EXPO_PUBLIC_DEVELOPMENT_URL_NETWORK
                },
                production: process.env.EXPO_PUBLIC_API_URL
            },
            mode: process.env.EXPO_PUBLIC_PUBLIC_MODE || "development"
        }
    }

    return expoConfig
}
