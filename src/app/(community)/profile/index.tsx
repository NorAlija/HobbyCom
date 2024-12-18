import { StyleSheet, Text, View } from 'react-native'
import React from 'react'

const Profile = () => {
  return (
    <View style={styles.container}>
      <Text>Niko Pajavuori</Text>
    </View>
  )
}

export default Profile

const styles = StyleSheet.create({
  container:{
    flex: 1, // Make the container take up the full height
    justifyContent: "center", // Centers the content vertically
    alignItems: "center"
}
})