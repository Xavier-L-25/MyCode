import axios from "axios";
import * as helper from "./serviceHelpers";

const resetPasswordService = {
  emailEndpoint: `${helper.API_HOST_PREFIX}/api/emails`,
  userEndpoint: `${helper.API_HOST_PREFIX}/api/users`,
};

resetPasswordService.resetPassword = (payload) => {
  const config = {
    method: "POST",
    url: resetPasswordService.emailEndpoint + "/resetpassword",
    data: payload,
    withCredentials: false,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(helper.onGlobalSuccess).catch(helper.onGlobalError);
};

resetPasswordService.changePassword = (payload) => {
  const config = {
    method: "PUT",
    url: resetPasswordService.userEndpoint + "/updatepassword",
    data: payload,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(helper.onGlobalSuccess).catch(helper.onGlobalError);
};

export default resetPasswordService;
