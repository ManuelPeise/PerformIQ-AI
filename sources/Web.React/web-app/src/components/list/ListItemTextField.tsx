import React from "react";
import {
  StyledListItem,
  StyledListItemText,
} from "../styledComponents/listComponents";
import StyledTextField from "../styledComponents/textFields";

type TextFieldProps = {
  value: string;
  onChange: (value: string) => void;
  disabled?: boolean;
};

interface IProps {
  primary: string;
  secondary?: string;
  textFieldProps: TextFieldProps;
}

const ListItemTextField: React.FC<IProps> = (props) => {
  const { primary, secondary, textFieldProps } = props;
  return (
    <StyledListItem divider>
      <StyledListItemText primary={primary} secondary={secondary} />
      <StyledTextField
        variant="standard"
        value={textFieldProps.value}
        onChange={(event) => textFieldProps.onChange(event.target.value)}
        disabled={textFieldProps.disabled}
      />
    </StyledListItem>
  );
};

export default ListItemTextField;
