import React, { Component, Fragment } from 'react';
import  axios  from 'axios';
import { FindUserControls } from '../../components/FindUserControls';
import { MyGrid } from '../../components/Grid';
import { Container } from './components';

export class FindUser extends Component {
  state = {
    rowData: null,
  }

  componentDidMount(){
    axios.get('/data.json').then( res => {
      this.setState({
        rowData: res.data.data,
      })
    })
  }

  render() {
    const {rowData} = this.state;
    console.log('updated with:', rowData)
    return (
      <Fragment>
        <Container>
          <FindUserControls />
        </Container>
        <Container>
          <MyGrid rowData={rowData} />
        </Container>
      </Fragment>
    );
  }
}
