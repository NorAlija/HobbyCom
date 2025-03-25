import { signupSchema } from "@/schemas/user-schemas"
import { UserData } from "@/types"
import { ApiError } from "@/utils/errors"
import { zodResolver } from "@hookform/resolvers/zod"
import { useQueryClient } from "@tanstack/react-query"
import { useRouter } from "expo-router"
import React from "react"
import { Controller, useForm } from "react-hook-form"
import {
    ActivityIndicator,
    Keyboard,
    KeyboardAvoidingView,
    Platform,
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

export default function Signup() {
    const router = useRouter()
    const { signUp, isSigningUp } = useAuth()
    const [refreshing, setRefreshing] = React.useState(false)
    const queryClient = useQueryClient()

    const { control, handleSubmit, formState, setError, reset } = useForm({
        resolver: zodResolver(signupSchema),
        defaultValues: {
            firstName: "",
            lastName: "",
            email: "",
            username: "",
            phone: "",
            password: "",
            confirmPassword: ""
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

    const onSignup = async (data: UserData) => {
        try {
            // Clear all form errors
            setError("root", { type: "manual", message: "" })

            await signUp(data)
        } catch (err) {
            if (err instanceof ApiError) {
                setError("root", {
                    type: "manual",
                    message: err.message || "An error occurred. Please try again."
                })
            } else {
                setError("root", {
                    type: "manual",
                    message: "An unexpected error occurred. Please try again."
                })
            }
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
                        refreshControl={
                            <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
                        }
                    >
                        <View style={styles.formContainer}>
                            <Text style={styles.title}>Create Account</Text>

                            {/* First Name Input */}
                            <Controller
                                control={control}
                                name="firstName"
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
                                name="lastName"
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
                                        {error ? (
                                            <Text style={styles.errorText}>{error.message}</Text>
                                        ) : formState.errors.root &&
                                          formState.errors.root.message &&
                                          formState.errors.root.message.includes(
                                              "already registered"
                                          ) ? (
                                            <Text style={styles.errorText}>
                                                {formState.errors.root.message}
                                            </Text>
                                        ) : null}
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
                                        {error ? (
                                            <Text style={styles.errorText}>{error.message}</Text>
                                        ) : formState.errors.root &&
                                          formState.errors.root.message &&
                                          formState.errors.root.message.includes(
                                              "already taken"
                                          ) ? (
                                            <Text style={styles.errorText}>
                                                {formState.errors.root.message}
                                            </Text>
                                        ) : null}
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
                                        {error ? (
                                            <Text style={styles.errorText}>{error.message}</Text>
                                        ) : formState.errors.root &&
                                          formState.errors.root.message &&
                                          formState.errors.root.message.includes("Phone") ? (
                                            <Text style={styles.errorText}>
                                                {formState.errors.root.message}
                                            </Text>
                                        ) : null}
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
                                disabled={isSigningUp}
                            >
                                <Text style={styles.signupButtonText}>
                                    {isSigningUp ? (
                                        <>
                                            {/* Creating Account... */}
                                            <ActivityIndicator size="small" color="#0000ff" />
                                        </>
                                    ) : (
                                        "Sign Up"
                                    )}
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
        backgroundColor: "#f0f0f0"
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
