"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var react_pro_sidebar_1 = require("react-pro-sidebar");
var react_router_dom_1 = require("react-router-dom");
var handleChange = function () {
};
var NavMenuItem = function (prop) {
    return (React.createElement("div", null,
        React.createElement(react_pro_sidebar_1.SubMenu, { title: "Proyecto: " + prop.project.name },
            React.createElement(react_pro_sidebar_1.SubMenu, { title: "Infraestructura" },
                React.createElement(react_pro_sidebar_1.MenuItem, null,
                    React.createElement(react_router_dom_1.Link, { to: "/preview/" + prop.project.id }, "Preview")),
                React.createElement(react_pro_sidebar_1.MenuItem, null, "Entornos")),
            React.createElement(react_pro_sidebar_1.MenuItem, null, "Equipo"),
            React.createElement(react_pro_sidebar_1.MenuItem, null, "Presupuesto"))));
};
exports.default = NavMenuItem;
//# sourceMappingURL=NavMenuItem.js.map