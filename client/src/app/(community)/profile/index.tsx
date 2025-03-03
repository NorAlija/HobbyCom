import React from "react"
import { StyleSheet, Text, TouchableOpacity, View } from "react-native"
import { useAuth } from "../../providers/auth-providers"

const Profile = () => {
    const { signOut /*user*/ } = useAuth()
    // const [firstName, setFirstName] = useState("")
    // const [loading, setLoading] = useState(true)

    // useEffect(() => {
    //     fetchProfile()
    // }, [user])

    // const fetchProfile = async () => {
    //     try {
    //         if (!user) return

    //         const { data, error } = await supabase
    //             .from("profiles")
    //             .select("first_name")
    //             .eq("user_id", user.id)
    //             .single()

    //         if (error) {
    //             console.error("Error fetching profile:", error)
    //         } else if (data) {
    //             setFirstName(data.first_name || "")
    //         }
    //     } catch (error) {
    //         console.error("Error:", error)
    //     } finally {
    //         setLoading(false)
    //     }
    // }

    // if (loading) {
    //     return (
    //         <View style={styles.container}>
    //             <Text>Loading...</Text>
    //         </View>
    //     )
    // }

    return (
        <View style={styles.container}>
            <Text style={styles.name}>{/*user?.firstname ||*/ "User"}</Text>
            <TouchableOpacity onPress={signOut} style={styles.button}>
                <Text style={styles.buttonText}>Sign Out</Text>
            </TouchableOpacity>
        </View>
    )
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
        padding: 20
    },
    button: {
        marginTop: 20,
        padding: 10,
        backgroundColor: "#007bff",
        borderRadius: 5,
        width: "100%",
        maxWidth: 200,
        alignItems: "center"
    },
    buttonText: {
        color: "white",
        fontSize: 16
    },
    name: {
        fontSize: 24,
        marginBottom: 20
    }
})

export default Profile

//CHATGPT VERSIO
// import { StyleSheet, Text, View, TouchableOpacity } from 'react-native';
// import React, { useState, useEffect } from 'react';
// import { useAuth } from '../../providers/auth-providers'; // Import the useAuth hook
// import { supabase } from "../../../lib/supabase";

// const Profile = () => {
//   const { signOut, user } = useAuth(); // Access the user object from useAuth
//   const [userData, setUserData] = useState<{ first_name: string; last_name: string } | null>(null);

//   useEffect(() => {
//     const fetchUserData = async () => {
//       if (user) {
//         try {
//           const { data, error } = await supabase
//             .from('profiles') // Replace 'users' with your table name
//             .select('first_name, last_name')
//             .eq('user_id', user.id) // Filter by the logged-in user's ID
//             .single(); // Fetch a single record

//           if (error) {
//             console.error('Error fetching user data:', error);
//           } else {
//             setUserData(data); // Set the fetched user data
//           }
//         } catch (error) {
//           console.error('Unexpected error fetching user data:', error);
//         }
//       }
//     };

//     fetchUserData();
//   }, [user]);

//   const handleSignOut = async () => {
//     try {
//       await signOut(); // Call the signOut function
//     } catch (error) {
//       console.error('Error signing out:', error);
//     }
//   };

//   return (
//     <View style={styles.container}>
//       {userData ? (
//         <Text>{`${userData.first_name} ${userData.last_name}`}</Text>
//       ) : (
//         <Text>Loading...</Text>
//       )}
//       <TouchableOpacity onPress={handleSignOut} style={styles.button}>
//         <Text style={styles.buttonText}>Sign Out</Text>
//       </TouchableOpacity>
//     </View>
//   );
// };

// export default Profile;

// const styles = StyleSheet.create({
//   container: {
//     flex: 1,
//     justifyContent: 'center',
//     alignItems: 'center',
//   },
//   button: {
//     marginTop: 20,
//     padding: 10,
//     backgroundColor: '#007bff', // Button color
//     borderRadius: 5, // Rounded corners
//   },
//   buttonText: {
//     color: 'white', // Text color
//     fontSize: 16,
//   },
// });

//TÄLLÄ HETKELLÄ TOIMIVA
// const Profile = () => {
//   const { signOut } = useAuth(); // Destructure the signOut function from the useAuth hook

//   const handleSignOut = async () => {
//     try {
//       await signOut(); // Call the signOut function when the button is pressed
//     } catch (error) {
//       console.error("Error signing out: ", error);
//     }
//   };

//   return (
//     <View style={styles.container}>
//       <Text>Niko Pajavuori</Text>
//       {/* Custom Sign Out Button using TouchableOpacity */}
//       <TouchableOpacity onPress={handleSignOut} style={styles.button}>
//         <Text style={styles.buttonText}>Sign Out</Text>
//       </TouchableOpacity>
//     </View>
//   );
// };
