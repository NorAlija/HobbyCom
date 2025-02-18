export class ApiError extends Error {
    constructor(
        message: string,
        public statusCode?: number,
        public type?: string,
        public title?: string,
        public instance?: string,
        public traceId?: string,
        public timestamp?: string,
        public errors?: Record<string, string[]> | string[],
        public details?: string
    ) {
        super(message)
        this.name = "ApiError"
    }
}
