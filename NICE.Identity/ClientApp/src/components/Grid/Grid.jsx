import React, { Component } from "react";
import { AgGridReact } from "ag-grid-react";
import "ag-grid-community/dist/styles/ag-grid.css";
import "ag-grid-community/dist/styles/ag-theme-balham.css";
import { GridComponent } from "./components";

export class MyGrid extends Component {
  displayName = GridComponent.name;

  constructor(props) {
    super(props);

    this.state = {
      columnDefs: [
        { headerName: "Make", field: "make", sortable: true, filter: true },
        { headerName: "Model", field: "model", sortable: true, filter: true },
        { headerName: "Price", field: "price", sortable: true, filter: true }
      ]
    };
  }

  render() {
    return (
      <GridComponent className="ag-theme-balham">
        <AgGridReact
          columnDefs={this.state.columnDefs}
          rowData={this.props.rowData}
        />
      </GridComponent>
    );
  }
}
