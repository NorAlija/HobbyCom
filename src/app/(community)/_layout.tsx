import { Tabs } from "expo-router";
import { SafeAreaView } from "react-native-safe-area-context";
import { StyleSheet } from "react-native";
import { FontAwesome } from "@expo/vector-icons"

function TabBarIcon(props: {
    name: React.ComponentProps<typeof FontAwesome>["name"];
    color: string;
}){
    return <FontAwesome size={30} color={props.color} name={props.name} />;
}

const TabsLayout = () => {
    return (
        <SafeAreaView edges={["top"]} style={styles.safeArea}>
            <Tabs screenOptions={{
                tabBarActiveTintColor:"#1591ea",
                tabBarInactiveTintColor: "gray",
                tabBarLabelStyle: { fontSize: 16},
                tabBarStyle: {
                    borderTopLeftRadius: 20,
                    borderTopRightRadius: 20,
                    paddingTop: 10,
                },
                headerShown: false, 
            }}>
                <Tabs.Screen name="index" options={{ title: "", tabBarIcon(props) {
                    return <TabBarIcon {...props} name="home" />
                } }} />
                <Tabs.Screen name="map" options={{title: "", tabBarIcon(props) {
                    return <TabBarIcon {...props} name="map" />
                }}} />
                <Tabs.Screen name="profile" options={{title: "", tabBarIcon(props) {
                    return <TabBarIcon {...props} name="user"/>
                }}} />
            </Tabs>
        </SafeAreaView>
    )
}

export default TabsLayout;

const styles = StyleSheet.create({
    safeArea:{
        flex: 1,
    }
});