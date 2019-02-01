import React, { Component, Fragment } from "react";
import axios from "axios";
import qs from 'qs';
import { FindUserControls } from "../../components/FindUserControls";
import { MyGrid } from "../../components/Grid";
import { Container } from "./components";

export class FindUser extends Component {
  constructor(props) {
    super(props);
  }

  state = {
    rowData: null,
    filters: {
      userName: undefined,
      role: undefined
    }
  };

  callApi(){
    const url = `/data.json?${qs.stringify(this.state.filters)}`
    axios.get(url).then(res => {
      this.setState({
        rowData: res.data.data
      });
    });
  }

  componentDidMount() {
    this.callApi();
  }

  handleChange = event => {
    console.log(event.target.name);
    const value = event.target.value;
    this.setState({
      filters: { ...this.state.filters, [event.target.name]: value }
    }, this.callApi());
  }

  render() {
    console.log(this.state.filters)
    const { rowData } = this.state;
    const inputs = [
      {
        label: "User Name",
        name: "userName",
        onChange: this.handleChange,
      },
      {
        label: "Role",
        name: "role",
        onChange:this.handleChange,
      }
    ];
    return (
      <Fragment>
        <FindUserControls inputs={inputs} />
        <Container>
          <MyGrid rowData={rowData} />
        </Container>
      </Fragment>
    );
  }
}
