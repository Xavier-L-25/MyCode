import React from "react";
import PropTypes from "prop-types";

function Order({ order }) {
  const javaDate = new Date(order.dateCreated);

  return (
    <tr>
      <td className="py-3">{order.name}</td>
      <td className="py-3">{order.price}</td>
      <td className="py-3">{order.venue.name}</td>
      <td className="py-3">{javaDate.toDateString()}</td>
    </tr>
  );
}

Order.propTypes = {
  order: PropTypes.shape({
    name: PropTypes.string,
    price: PropTypes.number,
    venue: PropTypes.shape({
      name: PropTypes.string,
    }),
    dateCreated: PropTypes.string,
  }),
};

export default Order;
