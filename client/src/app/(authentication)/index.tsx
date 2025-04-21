import { loginSchema } from "@/schemas/auth-schema"
import { LoginInput } from "@/types"
import { ApiError } from "@/utils/errors"
import { zodResolver } from "@hookform/resolvers/zod"
import { useQueryClient } from "@tanstack/react-query"
import { useRouter } from "expo-router"
import React from "react"
import { Controller, useForm } from "react-hook-form"
import {
    ActivityIndicator,
    ImageBackground,
    Keyboard,
    RefreshControl,
    ScrollView,
    StyleSheet,
    Text,
    TextInput,
    TouchableOpacity,
    TouchableWithoutFeedback,
    View
} from "react-native"
import { SafeAreaView } from "react-native-safe-area-context"
import { useAuth } from "../providers/auth-providers"

export default function Auth() {
    const { signIn, isSigningIn } = useAuth()
    const router = useRouter()
    const [refreshing, setRefreshing] = React.useState(false)
    const queryClient = useQueryClient()

    const { control, handleSubmit, formState, setError, reset } = useForm({
        resolver: zodResolver(loginSchema),
        defaultValues: {
            email: "",
            password: ""
        },
        mode: "onChange"
    })

    const onRefresh = React.useCallback(async () => {
        await queryClient.invalidateQueries({ queryKey: ["auth"] })
        setRefreshing(true)
        setTimeout(() => {
            setRefreshing(false)
            reset()
            setError("root", { type: "manual", message: "" })
        }, 300)
    }, [reset, setError])

    const handleSignIn = async (data: LoginInput) => {
        try {
            setError("root", { type: "manual", message: "" })
            await signIn(data)
            // The auth provider will handle navigation
        } catch (err) {
            if (err instanceof ApiError) {
                setError("root", { type: "manual", message: err.message ?? "Failed to sign in" })
            } else {
                setError("root", {
                    type: "manual",
                    message: err instanceof Error ? err.message : "Failed to sign in"
                })
            }
        }
    }

    return (
        <SafeAreaView
            edges={["left", "right"]}
            style={{ flex: 1, backgroundColor: "rgba(0,0,0,0.7)" }}
        >
            <TouchableWithoutFeedback onPress={Keyboard.dismiss} style={{ flex: 1 }}>
                <ScrollView
                    contentContainerStyle={styles.scrollContainer}
                    keyboardShouldPersistTaps="handled"
                    refreshControl={
                        <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
                    }
                >
                    <ImageBackground
                        source={require("../assets/hml.jpg")}
                        style={styles.backGroundImage}
                    >
                        <View style={styles.overlay} />

                        <View style={styles.container}>
                            <Text style={styles.title}>HobbyCom</Text>
                            <Text style={styles.subtitle}>
                                Find free sports activities to do near you
                            </Text>

                            {formState.errors.root?.message ? (
                                <Text style={styles.error}>{formState.errors.root.message}</Text>
                            ) : null}

                            <Controller
                                control={control}
                                name="email"
                                render={({
                                    field: { value, onChange, onBlur },
                                    fieldState: { error }
                                }) => (
                                    <>
                                        <TextInput
                                            placeholder="Email"
                                            style={styles.input}
                                            value={value}
                                            onChangeText={onChange}
                                            onBlur={onBlur}
                                            placeholderTextColor="#aaa"
                                            autoCapitalize="none"
                                            editable={!formState.isSubmitting}
                                            keyboardType="email-address"
                                        />
                                        {error && <Text style={styles.error}>{error.message}</Text>}
                                    </>
                                )}
                            />

                            <Controller
                                control={control}
                                name="password"
                                render={({
                                    field: { value, onChange, onBlur },
                                    fieldState: { error }
                                }) => (
                                    <>
                                        <TextInput
                                            placeholder="Password"
                                            style={styles.input}
                                            value={value}
                                            onChangeText={onChange}
                                            onBlur={onBlur}
                                            secureTextEntry
                                            placeholderTextColor="#aaa"
                                            autoCapitalize="none"
                                            editable={!formState.isSubmitting}
                                        />
                                        {error && <Text style={styles.error}>{error.message}</Text>}
                                    </>
                                )}
                            />

                            <TouchableOpacity
                                style={styles.button}
                                onPress={handleSubmit(handleSignIn)}
                                disabled={isSigningIn}
                            >
                                <Text style={styles.buttonText}>
                                    {isSigningIn ? (
                                        <>
                                            {/* Signing in... */}
                                            <ActivityIndicator size="small" color="#0000ff" />
                                        </>
                                    ) : (
                                        "Sign In"
                                    )}
                                </Text>
                            </TouchableOpacity>

                            <TouchableOpacity
                                style={[styles.button, styles.signUpButton]}
                                onPress={() => router.push("/signup")}
                            >
                                <Text style={styles.buttonText}>Register</Text>
                            </TouchableOpacity>
                        </View>
                    </ImageBackground>
                </ScrollView>
            </TouchableWithoutFeedback>
        </SafeAreaView>
    )
}

const styles = StyleSheet.create({
    backGroundImage: {
        flex: 1,
        resizeMode: "cover",
        justifyContent: "center",
        alignItems: "center"
    },
    overlay: {
        ...StyleSheet.absoluteFillObject,
        backgroundColor: "rgba(0,0,0,0.7)"
    },
    container: {
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
        padding: 16,
        width: "100%"
    },
    scrollContainer: {
        flexGrow: 1,
        backgroundColor: "red",
        width: "100%"
    },
    title: {
        fontSize: 36,
        fontWeight: "bold",
        color: "#fff",
        marginBottom: 8
    },
    subtitle: {
        fontSize: 18,
        color: "#ddd",
        marginBottom: 32
    },
    input: {
        width: "90%",
        padding: 10,
        marginBottom: 16,
        backgroundColor: "transparent",
        borderBottomWidth: 2,
        borderBottomColor: "#fff",
        fontSize: 16,
        color: "#fff",
        shadowColor: "#000", // Shadow color
        shadowOffset: { width: 0, height: 2 }, // Shadow positioning
        shadowOpacity: 0.1, // Opacity of the shadow
        shadowRadius: 2, // Radius of the shadow
        elevation: 3 // For Android
    },

    button: {
        backgroundColor: "#0096FF",
        padding: 13,
        borderRadius: 8,
        marginBottom: 16,
        width: "90%",
        alignItems: "center"
    },
    signUpButton: {
        backgroundColor: "transparent",
        borderColor: "#fff",
        borderWidth: 1
    },
    signUpButtonText: {
        color: "#fff"
    },
    buttonText: {
        fontSize: 16,
        fontWeight: "bold",
        color: "#fff"
    },
    error: {
        color: "red",
        fontSize: 12,
        marginBottom: 16,
        textAlign: "left",
        width: "90%"
    }
})
