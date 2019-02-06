import React from 'react';
import {LefthandMenuContainer, StyledLink, LefthandMenuWrapper} from "./component";

export class NavMenu extends React.Component {
  render() {
    return (
      <LefthandMenuWrapper>
        <h1>User Admin Portal</h1>
        <LefthandMenuContainer>
            <StyledLink to="/">Users</StyledLink>
            <StyledLink to="/roles">Roles</StyledLink>
            <StyledLink to="/realms">Realms</StyledLink>
        </LefthandMenuContainer> 
      </LefthandMenuWrapper>
    );
  }
}