// import React, { memo, useCallback } from "react";
// import {
//   View,
//   Text,
//   StyleSheet,
//   Keyboard,
//   TouchableOpacity,
//   TextInput,
//   TouchableWithoutFeedback,
//   KeyboardAvoidingView,
//   Platform,
//   ScrollView
// } from "react-native";
// import { SafeAreaView } from "react-native-safe-area-context";
// import { useForm, Controller } from "react-hook-form";
// import * as zod from "zod";
// import { useRouter } from "expo-router";
// import { zodResolver } from "@hookform/resolvers/zod";

// // Signup validation schema
// const signupSchema = zod.object({
//   firstName: zod.string().min(2, { message: "First name must be at least 2 characters" }),
//   lastName: zod.string().min(2, { message: "Last name must be at least 2 characters" }),
//   email: zod.string().email({ message: "Invalid email address" }),
//   phoneNumber: zod.string().optional(),
//   password: zod.string().min(6, { message: "Password must be at least 6 characters long" }),
//   confirmPassword: zod.string()
// }).refine((data) => data.password === data.confirmPassword, {
//   message: "Passwords do not match",
//   path: ["confirmPassword"]
// });

// export default function Signup() {
//   const router = useRouter();
//   const { control, handleSubmit, formState } = useForm({
//     resolver: zodResolver(signupSchema),
//     defaultValues: {
//       firstName: "",
//       lastName: "",
//       email: "",
//       phoneNumber: "",
//       password: "",
//       confirmPassword: ""
//     },
//     mode: 'onChange'
//   });

//   const onSignup = useCallback((data: zod.infer<typeof signupSchema>) => {
//     console.log(data);
//     router.replace("../(community)")
//     // Implement your signup logic here
//     // For example, send data to backend, navigate to next screen, etc.
//   }, []);

//   return (
//     <SafeAreaView style={styles.safeArea}>
//       <KeyboardAvoidingView
//         behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
//         style={styles.container}
//       >
//         <TouchableWithoutFeedback onPress={Keyboard.dismiss}>
//           <ScrollView
//             contentContainerStyle={styles.scrollContainer}
//             keyboardShouldPersistTaps="handled"
//           >
//             <View style={styles.formContainer}>
//               <Text style={styles.title}>Create Account</Text>

//               {/* First Name Input */}
//               <Controller
//                 control={control}
//                 name="firstName"
//                 render={({
//                   field: { value, onChange, onBlur },
//                   fieldState: { error }
//                 }) => (
//                   <View style={styles.inputWrapper}>
//                     <TextInput
//                       placeholder="First Name"
//                       style={styles.input}
//                       value={value}
//                       onChangeText={onChange}
//                       onBlur={onBlur}
//                       placeholderTextColor="#888"
//                       autoCapitalize="words"
//                     />
//                     {error && <Text style={styles.errorText}>{error.message}</Text>}
//                   </View>
//                 )}
//               />

//               {/* Last Name Input */}
//               <Controller
//                 control={control}
//                 name="lastName"
//                 render={({
//                   field: { value, onChange, onBlur },
//                   fieldState: { error }
//                 }) => (
//                   <View style={styles.inputWrapper}>
//                     <TextInput
//                       placeholder="Last Name"
//                       style={styles.input}
//                       value={value}
//                       onChangeText={onChange}
//                       onBlur={onBlur}
//                       placeholderTextColor="#888"
//                       autoCapitalize="words"
//                     />
//                     {error && <Text style={styles.errorText}>{error.message}</Text>}
//                   </View>
//                 )}
//               />

//               {/* Email Input */}
//               <Controller
//                 control={control}
//                 name="email"
//                 render={({
//                   field: { value, onChange, onBlur },
//                   fieldState: { error }
//                 }) => (
//                   <View style={styles.inputWrapper}>
//                     <TextInput
//                       placeholder="Email"
//                       style={styles.input}
//                       value={value}
//                       onChangeText={onChange}
//                       onBlur={onBlur}
//                       placeholderTextColor="#888"
//                       keyboardType="email-address"
//                       autoCapitalize="none"
//                     />
//                     {error && <Text style={styles.errorText}>{error.message}</Text>}
//                   </View>
//                 )}
//               />

//               {/* Phone Number Input (Optional) */}
//               <Controller
//                 control={control}
//                 name="phoneNumber"
//                 render={({
//                   field: { value, onChange, onBlur },
//                   fieldState: { error }
//                 }) => (
//                   <View style={styles.inputWrapper}>
//                     <TextInput
//                       placeholder="Phone Number (Optional)"
//                       style={styles.input}
//                       value={value}
//                       onChangeText={onChange}
//                       onBlur={onBlur}
//                       placeholderTextColor="#888"
//                       keyboardType="phone-pad"
//                     />
//                     {error && <Text style={styles.errorText}>{error.message}</Text>}
//                   </View>
//                 )}
//               />

//               {/* Password Input */}
//               <Controller
//                 control={control}
//                 name="password"
//                 render={({
//                   field: { value, onChange, onBlur },
//                   fieldState: { error }
//                 }) => (
//                   <View style={styles.inputWrapper}>
//                     <TextInput
//                       placeholder="Password"
//                       style={styles.input}
//                       value={value}
//                       onChangeText={onChange}
//                       onBlur={onBlur}
//                       placeholderTextColor="#888"
//                       secureTextEntry
//                       autoCapitalize="none"
//                     />
//                     {error && <Text style={styles.errorText}>{error.message}</Text>}
//                   </View>
//                 )}
//               />

//               {/* Confirm Password Input */}
//               <Controller
//                 control={control}
//                 name="confirmPassword"
//                 render={({
//                   field: { value, onChange, onBlur },
//                   fieldState: { error }
//                 }) => (
//                   <View style={styles.inputWrapper}>
//                     <TextInput
//                       placeholder="Confirm Password"
//                       style={styles.input}
//                       value={value}
//                       onChangeText={onChange}
//                       onBlur={onBlur}
//                       placeholderTextColor="#888"
//                       secureTextEntry
//                       autoCapitalize="none"
//                     />
//                     {error && <Text style={styles.errorText}>{error.message}</Text>}
//                   </View>
//                 )}
//               />

//               {/* Signup Button */}
//               <TouchableOpacity
//                 style={styles.signupButton}
//                 onPress={handleSubmit(onSignup)}
//                 disabled={formState.isSubmitting}
//               >
//                 <Text style={styles.signupButtonText}>Sign Up</Text>
//               </TouchableOpacity>

//               {/* Login Navigation */}
//               <View style={styles.loginContainer}>
//                 <Text style={styles.loginText}>Already have an account? </Text>
//                 <TouchableOpacity onPress={() => router.push("/")}>
//                   <Text style={styles.loginLink}>Log In</Text>
//                 </TouchableOpacity>
//               </View>
//             </View>
//           </ScrollView>
//         </TouchableWithoutFeedback>
//       </KeyboardAvoidingView>
//     </SafeAreaView>
//   );
// }
import React, { memo, useState } from "react"
import {
    View,
    Text,
    StyleSheet,
    Keyboard,
    TouchableOpacity,
    TextInput,
    TouchableWithoutFeedback,
    KeyboardAvoidingView,
    Platform,
    ScrollView
} from "react-native"
import { SafeAreaView } from "react-native-safe-area-context"
import { useForm, Controller } from "react-hook-form"
import * as zod from "zod"
import { useRouter } from "expo-router"
import { zodResolver } from "@hookform/resolvers/zod"
import { useAuth } from "../providers/auth-providers"

// Signup validation schema
const signupSchema = zod
    .object({
        firstName: zod.string().min(2, { message: "First name must be at least 2 characters" }),
        lastName: zod.string().min(2, { message: "Last name must be at least 2 characters" }),
        email: zod.string().email({ message: "Invalid email address" }),
        phoneNumber: zod.string().optional(),
        password: zod.string().min(6, { message: "Password must be at least 6 characters long" }),
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
            firstName: "",
            lastName: "",
            email: "",
            phoneNumber: "",
            password: "",
            confirmPassword: ""
        },
        mode: "onChange"
    })
    const onSignup = async (data: zod.infer<typeof signupSchema>) => {
        try {
            setError("")
            await signUp({
                email: data.email,
                password: data.password,
                firstName: data.firstName,
                lastName: data.lastName,
                phoneNumber: data.phoneNumber
            })
            // The auth provider will handle navigation after successful signup
        } catch (err) {
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
                                        {error && (
                                            <Text style={styles.errorText}>{error.message}</Text>
                                        )}
                                    </View>
                                )}
                            />

                            {/* Phone Number Input (Optional) */}
                            <Controller
                                control={control}
                                name="phoneNumber"
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
