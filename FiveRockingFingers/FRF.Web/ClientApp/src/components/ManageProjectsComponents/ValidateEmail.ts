import * as yup from "yup";

const emailSchema = yup.object().shape({
    email: yup.string()
        .trim()
        .email('Debe ser un email valido.')
        .required(),
});

export function ValidateEmail(email: string, emailField: Function, openSnackbar: Function ) {
    let userEmail: string | null = "";
    if (typeof email === "string") userEmail = email;
    
    if (!emailSchema.isValidSync({ email: userEmail })) {
        openSnackbar("Formato de email invalido!", "warning");
        emailField("");
        return null;
    }
    else return userEmail;
}
