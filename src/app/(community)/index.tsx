
import { StyleSheet, Text, View, StatusBar, SafeAreaView } from 'react-native';

const Home = () => {
  return (
    <View style={styles.container}>
      <Text>Vittu mun kryptot on nousussa!</Text>
    </View>
    
  );
};

const styles = StyleSheet.create({
  container:{
    flex: 1, // Make the container take up the full height
    justifyContent: "center", // Centers the content vertically
    alignItems: "center"
}

});

export default Home;
