"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var react_pro_sidebar_1 = require("react-pro-sidebar");
var NavMenuItem = function (props) {
    return (React.createElement("div", null,
        React.createElement(react_pro_sidebar_1.SubMenu, { title: "Proyecto: " + props.project.name },
            React.createElement(react_pro_sidebar_1.MenuItem, null, "Artefactos"),
            React.createElement(react_pro_sidebar_1.MenuItem, null, "Equipo"),
            React.createElement(react_pro_sidebar_1.MenuItem, null, "Presupuesto"))));
};
exports.default = NavMenuItem;
//# sourceMappingURL=NavMenuItem.js.map