import React, { useEffect, useState } from "react";
import debug from "sabio-debug";
import { Table } from "react-bootstrap";
import Order from "./Order";
import orderService from "services/orderService";
import Pagination from "rc-pagination";
import "rc-pagination/assets/index.css";
import "./orderhistory.css";

const _logger = debug.extend("OrderHistory");

function OrderHistory() {
  const [orders, setOrders] = useState({
    orderRows: [],
    totalcount: 0,
    current: 1,
    pageIndex: 0,
    pageSize: 15,
    totalPages: 0,
  });

  useEffect(() => {
    orderService
      .getOrders(orders.pageIndex, orders.pageSize)
      .then(onGetOrdersSuccess)
      .catch(onGetOrdersError);
  }, [orders.current]);
  const onGetOrdersSuccess = (response) => {
    let ordersArray = response.item.pagedItems.map((order) => {
      return <Order key={order.id} order={order} />;
    });

    setOrders({
      ...orders,
      orderRows: ordersArray,
      totalcount: response.item.totalCount,
      pageSize: response.item.pageSize,
      totalPages: response.item.totalPages,
    });
  };
  const onGetOrdersError = (error) => {
    _logger(error);
  };

  const onChange = (page) => {
    setOrders({
      ...orders,
      current: page,
      pageIndex: page - 1,
    });
  };

  return (
    <div className="main-orderHistory-container">
      <section className="orderHistory-table-header">
        <h1 className="text-center">Order History</h1>
      </section>
      <div className="orderHistory-table-container">
        <Table className="table-hover mt-3 shadow-lg">
          <thead>
            <tr className="bg-headers">
              <th scope="col">Name</th>
              <th scope="col">Price</th>
              <th scope="col">Venue</th>
              <th scope="col">Date</th>
            </tr>
          </thead>
          <tbody className="orderHistory-table-body">{orders.orderRows}</tbody>
        </Table>
      </div>
      <Pagination
        className="text-center mt-4"
        onChange={onChange}
        current={orders.current}
        defaultPageSize={orders.pageSize}
        total={orders.totalcount}
      />
    </div>
  );
}

export default OrderHistory;
