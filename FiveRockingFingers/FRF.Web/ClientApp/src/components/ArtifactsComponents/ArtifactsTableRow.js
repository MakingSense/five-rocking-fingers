"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var reactstrap_1 = require("reactstrap");
var ArtifactsTableRow = function (props) {
    var deleteButtonClick = function () {
        props.deleteArtifact(props.artifact.id);
    };
    return (React.createElement("tr", null,
        React.createElement("td", null, props.artifact.name),
        React.createElement("td", null, props.artifact.provider),
        React.createElement("td", null, props.artifact.artifactType.name),
        React.createElement("td", null,
            React.createElement(reactstrap_1.Button, { color: "danger", onClick: deleteButtonClick }, "Borrar"))));
};
exports.default = ArtifactsTableRow;
//# sourceMappingURL=ArtifactsTableRow.js.map