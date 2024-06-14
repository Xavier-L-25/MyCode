import * as Yup from "yup";

const loginSchema = Yup.object().shape({
  email: Yup.string().min(2).max(255).required("Required"),
  password: Yup.string().min(8).max(100).required("Required"),
  rememberPassword: Yup.bool(),
});

const registerSchema = Yup.object().shape({
  email: Yup.string().min(2).max(255).required("Required"),
  firstName: Yup.string().min(2).max(100).required("Required"),
  lastName: Yup.string().min(2).max(100).required("Required"),
  mi: Yup.string().min(1).max(2),
  avatarUrl: Yup.string().min(2).max(255),
  password: Yup.string().min(2).max(100).required("Required"),
  passwordConfirm: Yup.string().min(2).max(100).required("Required"),
  isConfirmed: Yup.bool().required("Required"),
  statusId: Yup.number().min(1).max(5).required("Required"),
  roleId: Yup.number().min(1).max(2).required("Required"),
});

const resetPasswordSchema = Yup.object().shape({
  email: Yup.string().email().min(2).max(255).required("Required"),
});

const changePasswordSchema = Yup.object().shape({
  password: Yup.string()
    .min(8)
    .matches(
      /^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[#?!@$%^&*-]).{8,}$/,
      "Password must be at least 8 characters long and contain an uppercase letter, a lowercase letter, and a special character"
    )
    .required("Required"),
  confirmPassword: Yup.string()
    .oneOf([Yup.ref("password")], "Passwords do not match")
    .required("Required"),
});

export {
  registerSchema,
  loginSchema,
  resetPasswordSchema,
  changePasswordSchema,
};
