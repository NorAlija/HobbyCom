import {
    View,
    Text,
    StyleSheet,
    Keyboard,
    ImageBackground,
    TouchableOpacity,
    TextInput,
    TouchableWithoutFeedback
} from "react-native"
import { SafeAreaView } from "react-native-safe-area-context"
import { useForm, Controller } from "react-hook-form"
import * as zod from "zod"
import { useRouter } from "expo-router"
import { zodResolver } from "@hookform/resolvers/zod"
import React, { useState } from "react"
import { useAuth } from "../providers/auth-providers"

const authSchema = zod.object({
    email: zod.string().email({ message: "Invalid email address" }),
    password: zod.string().min(6, { message: "Password must be at least 6 characters long" })
})

export default function Auth() {
    const { signIn } = useAuth()
    const [error, setError] = useState<string>("")
    const router = useRouter()

    const { control, handleSubmit, formState } = useForm({
        resolver: zodResolver(authSchema),
        defaultValues: {
            email: "",
            password: ""
        }
    })

    const handleSignIn = async (data: zod.infer<typeof authSchema>) => {
        try {
            setError("")
            await signIn(data.email, data.password)
            // The auth provider will handle navigation
        } catch (err) {
            setError(err instanceof Error ? err.message : "Failed to sign in")
        }
    }

    return (
        <SafeAreaView edges={["left", "right"]} style={{ flex: 1 }}>
            <TouchableWithoutFeedback onPress={Keyboard.dismiss} style={{ flex: 1 }}>
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

                        {error ? <Text style={styles.error}>{error}</Text> : null}

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
                            disabled={formState.isSubmitting}
                        >
                            <Text style={styles.buttonText}>
                                {formState.isSubmitting ? "Signing in..." : "Sign In"}
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
