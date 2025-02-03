import { StatusBar } from "expo-status-bar"
import React from "react"

import { SafeAreaView, StyleSheet, Text, View } from "react-native"

export default function App() {
    return (
        <SafeAreaView>
            <View style={styles.container}>
                <Text>Hello olivia</Text>
                <StatusBar style="auto" />
            </View>
        </SafeAreaView>
    )
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: "#fff",
        alignItems: "center",
        justifyContent: "center"
    }
})
