import axios from "axios";
import * as helper from "./serviceHelpers";

const orderService = {
  endpoint: `${helper.API_HOST_PREFIX}/api/order`,
};

orderService.addOrder = (payload) => {
  const config = {
    method: "POST",
    url: orderService.endpoint,
    data: payload,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(helper.onGlobalSuccess).catch(helper.onGlobalError);
};

orderService.getOrders = (pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url:
      orderService.endpoint +
      `/orderhistory/?pageindex=${pageIndex}&pagesize=${pageSize}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(helper.onGlobalSuccess).catch(helper.onGlobalError);
};

export default orderService;
