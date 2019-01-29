import React, { Component } from 'react';
import { Col, Grid, Row } from 'react-bootstrap';
import { FindUserControls } from '../../components/FindUserControls';
import { GridComponent } from '../../components/Grid';

export class FindUser extends Component {
  

  render() {
    return (
      <Grid fluid>
        <Row>
           <FindUserControls />
        </Row>
		<Row>
			<GridComponent />
		</Row>
      </Grid>
    );
  }
}
