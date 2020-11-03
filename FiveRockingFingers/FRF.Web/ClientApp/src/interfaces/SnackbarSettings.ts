export default interface SnackbarSettings {
    message: string,
    severity: "success" | "info" | "warning" | "error" | undefined,
}