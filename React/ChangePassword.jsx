import React, { useState } from "react";
import { useSearchParams } from "react-router-dom";
import { Formik, Form, Field, ErrorMessage } from "formik";
import { changePasswordSchema } from "./userSchema";
import PasswordService from "services/passwordService";
import toastr from "toastr";
import "../users/loginform.css";

import debug from "sabio-debug";

const _logger = debug.extend("ChangePassword");

function ChangePassword() {
  const [queryParams] = useSearchParams();
  const token = queryParams.get("token");
  const [changePasswordForm] = useState({
    token: token,
    password: "",
    confirmPassword: "",
  });

  const onPasswordSubmit = (value) => {
    _logger(value);

    PasswordService.changePassword(value)
      .then(onChangePasswordSuccess)
      .catch(onChangePasswordError);
  };

  const onChangePasswordSuccess = () => {
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
      "The password for your account has been updated.",
      "Password Updated"
    );

    setTimeout(() => {
      window.location.replace("/login");
    }, 3500);
  };

  const onChangePasswordError = () => {
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
      "There was an issue updating your password.",
      "Password Update Error"
    );
  };

  return (
    <div className="login-form card">
      <div className="card-body">
        <Formik
          initialValues={changePasswordForm}
          validationSchema={changePasswordSchema}
          onSubmit={onPasswordSubmit}
        >
          <Form>
            <div>
              <h3 className="login-form-header mb-4">Change Password</h3>
            </div>
            <div className="form-group mt-3">
              <label htmlFor="password">Password</label>
              <Field type="password" className="form-control" name="password" />
              <ErrorMessage
                name="password"
                component="div"
                className="text-center loginform-err-msg"
              />
            </div>
            <div className="form-group mt-3">
              <label htmlFor="confirmPassword">Confirm Password</label>
              <Field
                type="password"
                className="form-control"
                name="confirmPassword"
              />
              <ErrorMessage
                name="confirmPassword"
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

export default ChangePassword;
