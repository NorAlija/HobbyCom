import { useQueryClient } from "@tanstack/react-query"
import React from "react"
import { RefreshControl, ScrollView, StyleSheet, Text, View } from "react-native"

const Home = () => {
    const queryClient = useQueryClient()
    const [refreshing, setRefreshing] = React.useState(false)

    const onRefresh = React.useCallback(async () => {
        await queryClient.invalidateQueries({ queryKey: ["auth"] })

        setRefreshing(true)
        setTimeout(() => {
            setRefreshing(false)
        }, 300)
    }, [])

    return (
        <ScrollView
            contentContainerStyle={styles.scrollContainer}
            keyboardShouldPersistTaps="handled"
            refreshControl={<RefreshControl refreshing={refreshing} onRefresh={onRefresh} />}
        >
            <View style={styles.container}>
                <Text>Vittu mun kryptot on nousussa!</Text>
            </View>
        </ScrollView>
    )
}

const styles = StyleSheet.create({
    container: {
        flex: 1, // Make the container take up the full height
        justifyContent: "center", // Centers the content vertically
        alignItems: "center"
    },
    scrollContainer: {
        flexGrow: 1,
        justifyContent: "center",
        paddingHorizontal: 20,
        backgroundColor: "white"
    }
})

export default Home
