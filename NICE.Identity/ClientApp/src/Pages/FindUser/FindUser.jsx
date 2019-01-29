import React, { Component, Fragment } from 'react';
import  axios  from 'axios';
import { FindUserControls } from '../../components/FindUserControls';
import { MyGrid } from '../../components/Grid';
import { Container } from './components';

export class FindUser extends Component {
  state = {
    gridData: null,
  }

  componentDidMount(){
    axios.get('/data.json').then( res => {
      this.setState({
        gridData: res.data,
      })
    })
  }

  render() {
    const {gridData} = this.state;
    console.log('updated with:', gridData)
    return (
      <Fragment>
        <Container>
          <FindUserControls />
        </Container>
        <Container>
          <MyGrid data={gridData} />
        </Container>
      </Fragment>
    );
  }
}
