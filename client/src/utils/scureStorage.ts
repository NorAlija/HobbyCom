import AsyncStorage from "@react-native-async-storage/async-storage"
import * as aesjs from "aes-js"
import * as SecureStore from "expo-secure-store"
import "react-native-get-random-values"

/**
 * As Expo's SecureStore does not support values larger than 2048
 * bytes, an AES-256 key is generated and stored in SecureStore, while
 *  it is used to encrypt/decrypt values stored in AsyncStorage.
 */
async function _encrypt(key: string, value: string) {
    const encryptionKey = crypto.getRandomValues(new Uint8Array(256 / 8))

    const cipher = new aesjs.ModeOfOperation.ctr(encryptionKey, new aesjs.Counter(1))
    const encryptedBytes = cipher.encrypt(aesjs.utils.utf8.toBytes(value))

    await SecureStore.setItemAsync(key, aesjs.utils.hex.fromBytes(encryptionKey))

    return aesjs.utils.hex.fromBytes(encryptedBytes)
}

async function _decrypt(key: string, value: string) {
    const encryptionKeyHex = await SecureStore.getItemAsync(key)
    if (!encryptionKeyHex) {
        return encryptionKeyHex
    }

    const cipher = new aesjs.ModeOfOperation.ctr(
        aesjs.utils.hex.toBytes(encryptionKeyHex),
        new aesjs.Counter(1)
    )
    const decryptedBytes = cipher.decrypt(aesjs.utils.hex.toBytes(value))

    return aesjs.utils.utf8.fromBytes(decryptedBytes)
}

export async function getItem(key: string) {
    const encrypted = await AsyncStorage.getItem(key)
    if (!encrypted) {
        return encrypted
    }

    return await _decrypt(key, encrypted)
}

export async function removeItem(key: string) {
    await AsyncStorage.removeItem(key)
    await SecureStore.deleteItemAsync(key)
}

export async function setItem(key: string, value: string) {
    const encrypted = await _encrypt(key, value)

    await AsyncStorage.setItem(key, encrypted)
}
