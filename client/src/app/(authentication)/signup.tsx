import { zodResolver } from "@hookform/resolvers/zod"
import { useRouter } from "expo-router"
import React, { useState } from "react"
import { Controller, useForm } from "react-hook-form"
import {
    Keyboard,
    KeyboardAvoidingView,
    Platform,
    ScrollView,
    StyleSheet,
    Text,
    TextInput,
    TouchableOpacity,
    TouchableWithoutFeedback,
    View
} from "react-native"
import { SafeAreaView } from "react-native-safe-area-context"
import * as zod from "zod"
import { useAuth } from "../providers/auth-providers"

const signupSchema = zod
    .object({
        firstname: zod
            .string()
            .min(2, { message: "Firstname must be at least 2 characters long" })
            .max(50, { message: "Firstname must not exceed 50 characters" }),
        lastname: zod
            .string()
            .min(2, { message: "Lastname must be at least 2 characters long" })
            .max(50, { message: "Lastname must not exceed 50 characters" }),
        email: zod.string().email({ message: "Invalid email address" }),
        username: zod
            .string()
            .min(4, { message: "Username must be at least 4 characters long" })
            .regex(/^[a-zA-Z0-9]*$/, { message: "Username must contain only letters and numbers" }),
        phone: zod.string().optional(),
        type: zod.string().default("USER"),
        avatarUrl: zod.string().optional(),
        password: zod
            .string()
            .min(8, { message: "Password must be at least 8 characters long" })
            .regex(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/, {
                message:
                    "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character"
            }),
        confirmPassword: zod.string()
    })
    .refine((data) => data.password === data.confirmPassword, {
        message: "Passwords do not match",
        path: ["confirmPassword"]
    })

export default function Signup() {
    const router = useRouter()
    const { signUp } = useAuth()
    const [error, setError] = useState<string>("")

    const { control, handleSubmit, formState } = useForm({
        resolver: zodResolver(signupSchema),
        defaultValues: {
            firstname: "",
            lastname: "",
            email: "",
            username: "",
            phone: "",
            type: "USER",
            avatarUrl: "",
            password: "",
            confirmPassword: ""
        },
        mode: "onChange"
    })
    const onSignup = async (data: zod.infer<typeof signupSchema>) => {
        try {
            console.log("Starting signup process...")
            console.log("Form data:", data)

            setError("")
            console.log("Calling signUp function...")
            await signUp(data)
            console.log("Signup completed successfully")
        } catch (err) {
            console.error("Detailed signup error:", err)
            console.error("Error stack:", err instanceof Error ? err.stack : "No stack trace")
            setError(err instanceof Error ? err.message : "Failed to sign up")
        }
    }

    return (
        <SafeAreaView style={styles.safeArea}>
            <KeyboardAvoidingView
                behavior={Platform.OS === "ios" ? "padding" : "height"}
                style={styles.container}
            >
                <TouchableWithoutFeedback onPress={Keyboard.dismiss}>
                    <ScrollView
                        contentContainerStyle={styles.scrollContainer}
                        keyboardShouldPersistTaps="handled"
                    >
                        <View style={styles.formContainer}>
                            <Text style={styles.title}>Create Account</Text>

                            {error ? <Text style={styles.errorText}>{error}</Text> : null}

                            {/* First Name Input */}
                            <Controller
                                control={control}
                                name="firstname"
                                render={({
                                    field: { value, onChange, onBlur },
                                    fieldState: { error }
                                }) => (
                                    <View style={styles.inputWrapper}>
                                        <TextInput
                                            placeholder="First Name"
                                            style={styles.input}
                                            value={value}
                                            onChangeText={onChange}
                                            onBlur={onBlur}
                                            placeholderTextColor="#888"
                                            autoCapitalize="words"
                                            editable={!formState.isSubmitting}
                                        />
                                        {error && (
                                            <Text style={styles.errorText}>{error.message}</Text>
                                        )}
                                    </View>
                                )}
                            />

                            {/* Last Name Input */}
                            <Controller
                                control={control}
                                name="lastname"
                                render={({
                                    field: { value, onChange, onBlur },
                                    fieldState: { error }
                                }) => (
                                    <View style={styles.inputWrapper}>
                                        <TextInput
                                            placeholder="Last Name"
                                            style={styles.input}
                                            value={value}
                                            onChangeText={onChange}
                                            onBlur={onBlur}
                                            placeholderTextColor="#888"
                                            autoCapitalize="words"
                                            editable={!formState.isSubmitting}
                                        />
                                        {error && (
                                            <Text style={styles.errorText}>{error.message}</Text>
                                        )}
                                    </View>
                                )}
                            />

                            {/* Email Input */}
                            <Controller
                                control={control}
                                name="email"
                                render={({
                                    field: { value, onChange, onBlur },
                                    fieldState: { error }
                                }) => (
                                    <View style={styles.inputWrapper}>
                                        <TextInput
                                            placeholder="Email"
                                            style={styles.input}
                                            value={value}
                                            onChangeText={onChange}
                                            onBlur={onBlur}
                                            placeholderTextColor="#888"
                                            keyboardType="email-address"
                                            autoCapitalize="none"
                                            editable={!formState.isSubmitting}
                                        />
                                        {error && (
                                            <Text style={styles.errorText}>{error.message}</Text>
                                        )}
                                    </View>
                                )}
                            />
                            {/* Username Input */}
                            <Controller
                                control={control}
                                name="username"
                                render={({
                                    field: { value, onChange, onBlur },
                                    fieldState: { error }
                                }) => (
                                    <View style={styles.inputWrapper}>
                                        <TextInput
                                            placeholder="Username"
                                            style={styles.input}
                                            value={value}
                                            onChangeText={onChange}
                                            onBlur={onBlur}
                                            placeholderTextColor="#888"
                                            autoCapitalize="none"
                                            editable={!formState.isSubmitting}
                                        />
                                        {error && (
                                            <Text style={styles.errorText}>{error.message}</Text>
                                        )}
                                    </View>
                                )}
                            />

                            {/* Phone Number Input (Optional) */}
                            <Controller
                                control={control}
                                name="phone"
                                render={({
                                    field: { value, onChange, onBlur },
                                    fieldState: { error }
                                }) => (
                                    <View style={styles.inputWrapper}>
                                        <TextInput
                                            placeholder="Phone Number (Optional)"
                                            style={styles.input}
                                            value={value}
                                            onChangeText={onChange}
                                            onBlur={onBlur}
                                            placeholderTextColor="#888"
                                            keyboardType="phone-pad"
                                            editable={!formState.isSubmitting}
                                        />
                                        {error && (
                                            <Text style={styles.errorText}>{error.message}</Text>
                                        )}
                                    </View>
                                )}
                            />

                            {/* Password Input */}
                            <Controller
                                control={control}
                                name="password"
                                render={({
                                    field: { value, onChange, onBlur },
                                    fieldState: { error }
                                }) => (
                                    <View style={styles.inputWrapper}>
                                        <TextInput
                                            placeholder="Password"
                                            style={styles.input}
                                            value={value}
                                            onChangeText={onChange}
                                            onBlur={onBlur}
                                            placeholderTextColor="#888"
                                            secureTextEntry
                                            autoCapitalize="none"
                                            editable={!formState.isSubmitting}
                                        />
                                        {error && (
                                            <Text style={styles.errorText}>{error.message}</Text>
                                        )}
                                    </View>
                                )}
                            />

                            {/* Confirm Password Input */}
                            <Controller
                                control={control}
                                name="confirmPassword"
                                render={({
                                    field: { value, onChange, onBlur },
                                    fieldState: { error }
                                }) => (
                                    <View style={styles.inputWrapper}>
                                        <TextInput
                                            placeholder="Confirm Password"
                                            style={styles.input}
                                            value={value}
                                            onChangeText={onChange}
                                            onBlur={onBlur}
                                            placeholderTextColor="#888"
                                            secureTextEntry
                                            autoCapitalize="none"
                                            editable={!formState.isSubmitting}
                                        />
                                        {error && (
                                            <Text style={styles.errorText}>{error.message}</Text>
                                        )}
                                    </View>
                                )}
                            />

                            {/* Signup Button */}
                            <TouchableOpacity
                                style={styles.signupButton}
                                onPress={handleSubmit(onSignup)}
                                disabled={formState.isSubmitting}
                            >
                                <Text style={styles.signupButtonText}>
                                    {formState.isSubmitting ? "Creating Account..." : "Sign Up"}
                                </Text>
                            </TouchableOpacity>

                            {/* Login Navigation */}
                            <View style={styles.loginContainer}>
                                <Text style={styles.loginText}>Already have an account? </Text>
                                <TouchableOpacity onPress={() => router.push("/")}>
                                    <Text style={styles.loginLink}>Log In</Text>
                                </TouchableOpacity>
                            </View>
                        </View>
                    </ScrollView>
                </TouchableWithoutFeedback>
            </KeyboardAvoidingView>
        </SafeAreaView>
    )
}

const styles = StyleSheet.create({
    safeArea: {
        flex: 1,
        backgroundColor: "white"
    },
    container: {
        flex: 1
    },
    scrollContainer: {
        flexGrow: 1,
        justifyContent: "center",
        paddingHorizontal: 20,
        backgroundColor: "white"
    },
    formContainer: {
        width: "100%",
        alignItems: "center"
    },
    title: {
        fontSize: 24,
        fontWeight: "bold",
        marginBottom: 30,
        color: "#333"
    },
    inputWrapper: {
        width: "100%",
        marginBottom: 20
    },
    input: {
        width: "100%",
        borderBottomWidth: 1,
        borderBottomColor: "#888",
        paddingVertical: 10,
        fontSize: 16,
        color: "#333"
    },
    errorText: {
        color: "red",
        fontSize: 12,
        marginTop: 5
    },
    signupButton: {
        width: "100%",
        backgroundColor: "#0096FF",
        paddingVertical: 15,
        borderRadius: 8,
        alignItems: "center",
        marginTop: 20
    },
    signupButtonText: {
        color: "white",
        fontSize: 16,
        fontWeight: "bold"
    },
    loginContainer: {
        flexDirection: "row",
        marginTop: 20,
        justifyContent: "center"
    },
    loginText: {
        color: "#888"
    },
    loginLink: {
        color: "#0096FF",
        fontWeight: "bold"
    }
})
