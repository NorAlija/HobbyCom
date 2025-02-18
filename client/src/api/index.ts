import { ApiError } from "@/utils/errors"
import getBaseURL from "@/utils/getBaseURL"
import axios, { AxiosError } from "axios"

const baseURL = getBaseURL()

const api = axios.create({
    baseURL,
    headers: {
        "Content-Type": "application/json"
    },
    timeout: 5000
})

// interscept errors from the backend
api.interceptors.response.use(
    (response) => response,
    (error: AxiosError) => {
        interface ResponseData {
            detail?: string
            errors?: Record<string, string[]> | string[]
            title?: string
            type?: string
            instance?: string
            traceId?: string
            timestamp?: string
        }

        const responseData: ResponseData = error.response?.data || {}
        const statusCode = error.response?.status

        const detail = responseData.detail
        const validationErrors = responseData.errors
        const title = responseData.title
        const type = responseData.type

        let message = detail || title || "An error occurred"

        if (validationErrors) {
            if (Array.isArray(validationErrors)) {
                message = validationErrors.join(", ")
            } else {
                message = Object.entries(validationErrors)
                    .flatMap(([field, errors]) => `${field}: ${(errors as string[]).join(", ")}`)
                    .join("; ")
            }
        }

        // Create enhanced error
        const apiError = new ApiError(
            message,
            statusCode,
            type,
            title,
            responseData.instance,
            responseData.traceId,
            responseData.timestamp,
            validationErrors,
            responseData.detail
        )

        return Promise.reject(apiError)
    }
)

export default api
