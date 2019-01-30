import styled from 'styled-components';
import { Link } from "react-router-dom";

export const LefthandMenuWrapper = styled.div`
`

export const LefthandMenuContainer = styled.div`
    margin-top:2rem;
    height: 100%;
    display: flex;
    flex-direction: column;
    justify-content: space-evenly;
`

export const StyledLink = styled(Link)`
    background-color: #eee;
    color: #999;
    padding: 2rem;
    margin: 0.2rem 0;
    font-size: 1.5rem;
    border-radius: 6px;
    &:hover{
        text-decoration: none;
        color: #666;
    }
`