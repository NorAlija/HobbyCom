import { StyleSheet, Text, View } from "react-native"
import React from "react"

const Map = () => {
    return (
        <View style={styles.container}>
            <Text>Push this shit to production</Text>
        </View>
    )
}

export default Map

const styles = StyleSheet.create({
    container: {
        flex: 1, // Make the container take up the full height
        justifyContent: "center", // Centers the content vertically
        alignItems: "center"
    }
})
