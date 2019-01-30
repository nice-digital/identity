import React, { Component } from "react";
import { BrowserRouter, Route } from "react-router-dom";
import { Layout } from "./components/Layout";
import { FindUser } from "./Pages/FindUser";

export default class App extends Component {
  render() {
    return (
      <BrowserRouter>
        <Layout>
          <Route exact path="/" component={FindUser} />
          <Route exact path="/roles" component={FindUser} />
          <Route exact path="/realms" component={FindUser} />
        </Layout>
      </BrowserRouter>
    );
  }
}
