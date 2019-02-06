import React, { Fragment } from "react";
import { Label, Input, FormFeedback, FormText } from "reactstrap";
import { FormGroupStyled, Container } from "./components";

export const FindUserControls = ({ inputs }) => {
  const { label } = inputs;

  return (
    <Container>
      {inputs.map(({ label, onChange, name }, index) => (
        <FormGroupStyled key={index}>
          <Label for={name}>{label}</Label>
          <Input name={name} onChange={(e)=>onChange(e)} />
          <FormFeedback>
            User Name value must be at least 3 characters for a maximum of 50
            characters.
          </FormFeedback>
          <FormText>Filter your results by inserting a keyword.</FormText>
        </FormGroupStyled>
      ))}
    </Container>
  );
};
