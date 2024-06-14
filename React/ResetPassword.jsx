import React, { useState } from "react";
import { Formik, Form, Field, ErrorMessage } from "formik";
import { resetPasswordSchema } from "./userSchema";
import PasswordService from "services/passwordService";
import toastr from "toastr";
import "../users/loginform.css";

import debug from "sabio-debug";
const _logger = debug.extend("ResetPassword");

function ResetPassword() {
  const [resetPasswordForm] = useState({
    email: "",
  });

  const onEmailSubmit = (value) => {
    //value is equal to user input on email field
    _logger(value);

    PasswordService.resetPassword(value)
      .then(onResetPasswordSuccess)
      .catch(onResetPasswordError);
  };

  const onResetPasswordSuccess = () => {
    toastr.options = {
      closeButton: false,
      debug: false,
      newestOnTop: false,
      progressBar: false,
      positionClass: "toast-top-right",
      preventDuplicates: false,
      onclick: null,
      showDuration: "300",
      hideDuration: "1000",
      timeOut: "5000",
      extendedTimeOut: "1000",
      showEasing: "swing",
      hideEasing: "linear",
      showMethod: "fadeIn",
      hideMethod: "fadeOut",
    };

    toastr["success"](
      "An email to reset your password has been sent to your inbox.",
      "Reset Email Sent"
    );

    setTimeout(() => {
      window.location.replace("/");
    }, 3500);
  };

  const onResetPasswordError = () => {
    toastr.options = {
      closeButton: false,
      debug: false,
      newestOnTop: false,
      progressBar: false,
      positionClass: "toast-top-right",
      preventDuplicates: false,
      onclick: null,
      showDuration: "300",
      hideDuration: "1000",
      timeOut: "5000",
      extendedTimeOut: "1000",
      showEasing: "swing",
      hideEasing: "linear",
      showMethod: "fadeIn",
      hideMethod: "fadeOut",
    };

    toastr["error"](
      "The email entered does not have an account associated with it.",
      "Email Error"
    );
  };

  _logger("test");

  return (
    <div className="login-form card">
      <div className="card-body">
        <Formik
          initialValues={resetPasswordForm}
          onSubmit={onEmailSubmit}
          validationSchema={resetPasswordSchema}
        >
          <Form>
            <div>
              <h3 className="login-form-header mb-4">Reset Password</h3>
            </div>
            <div className="form-group mt-3">
              <label htmlFor="email">Email Address</label>
              <Field
                type="email"
                className="form-control"
                name="email"
                placeholder="johnExample@gmail.com"
              />
              <ErrorMessage
                name="email"
                component="div"
                className="text-center loginform-err-msg"
              />
            </div>
            <div className="text-center">
              <button type="submit" className="login-button btn">
                Submit
              </button>
            </div>
          </Form>
        </Formik>
      </div>
    </div>
  );
}

export default ResetPassword;
