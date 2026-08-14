import React from "react";
import StyledTextField from "../styledComponents/textFields";

interface IProps {
  label: string;
  value?: string;
  onChange: (value: string) => void;
  placeholder?: string;
  type?: string;
  disabled?: boolean;
  autoComplete?: string;
}

const FormTextField: React.FC<IProps> = (props) => {
  const {
    label,
    value,
    onChange,
    placeholder,
    type = "text",
    disabled = false,
    autoComplete = "off",
  } = props;

  return (
    <StyledTextField
      variant="standard"
      label={label}
      value={value}
      onChange={(e) => onChange(e.target.value)}
      placeholder={placeholder}
      type={type}
      disabled={disabled}
      autoComplete={autoComplete}
    />
  );
};

export default FormTextField;
